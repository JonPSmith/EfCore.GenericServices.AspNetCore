// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.Helpers
{
    public static class ResponseCheckers
    {
        public static void CheckResponse(this IActionResult actionResult, GenericServices.IStatusGeneric status)
        {
            actionResult.CheckResponseWithValidCode(status, 200);
        }

        public static void CheckResponseWithValidCode(this IActionResult actionResult, GenericServices.IStatusGeneric status, int validStatusCode)
        {
            actionResult.ShouldNotBeNull();
            if (status.IsValid)
            {
                actionResult.ShouldNotBeNull();
                var result = actionResult as ObjectResult;
                Assert.NotNull(result);
                result.StatusCode.ShouldEqual(validStatusCode);
                result.Value.ToString().ShouldEqual("{ Message = " + status.Message + " }");
            }
            else
            {
                CheckErrorResponse(actionResult as BadRequestObjectResult, status.Errors.Select(x => x.ErrorResult));
            }
        }

        public static void CheckResponse<T>(this ActionResult<T> actionResult, GenericServices.IStatusGeneric status, T results)
        {
            actionResult.CheckResponseWithValidCode<T>(status, results, 200);
        }

        public static void CheckResponseWithValidCode<T>(this ActionResult<T> actionResult, GenericServices.IStatusGeneric status, T results, int validStatusCode)
        {
            actionResult.ShouldNotBeNull();
            if (status.IsValid)
            {
                var result = actionResult.Result as ObjectResult;
                result.ShouldNotBeNull();
                if (results == null)
                {
                    result.StatusCode.ShouldEqual(404);
                    result.Value.ToString().ShouldEqual("{ Message = " + status.Message + " }");
                }
                else
                {
                    result.StatusCode.ShouldEqual(validStatusCode);
                    result.Value.ToString().ShouldEqual("{ Message = " + status.Message + ", results = " + results + " }");
                }
            }
            else
            {
                CheckErrorResponse(actionResult.Result as ObjectResult, status.Errors.Select(x => x.ErrorResult));
            }
        }

        public static void CheckResponse(this IActionResult actionResult, GenericBizRunner.IStatusGeneric status)
        {
            actionResult.CheckResponseWithValidCode(status, 200);
        }

        public static void CheckResponseWithValidCode(this IActionResult actionResult, GenericBizRunner.IStatusGeneric status, int validStatusCode)
        {
            actionResult.ShouldNotBeNull();
            if (!status.HasErrors)
            {
                actionResult.ShouldNotBeNull();
                var result = actionResult as ObjectResult;
                Assert.NotNull(result);
                result.StatusCode.ShouldEqual(validStatusCode);
                result.Value.ToString().ShouldEqual("{ Message = " + status.Message + " }");
            }
            else
            {
                CheckErrorResponse(actionResult as BadRequestObjectResult, status.Errors);
            }
        }

        public static void CheckResponse<T>(this ActionResult<T> actionResult, GenericBizRunner.IStatusGeneric status, T results)
        {
            actionResult.CheckResponseWithValidCode<T>(status, results, 200);
        }

        public static void CheckResponseWithValidCode<T>(this ActionResult<T> actionResult, GenericBizRunner.IStatusGeneric status, T results, int validStatusCode)
        {
            actionResult.ShouldNotBeNull();
            var result = actionResult.Result as ObjectResult;
            result.ShouldNotBeNull();
            if (!status.HasErrors)
            {
                if (results == null)
                {
                    result.StatusCode.ShouldEqual(404);
                    result.Value.ToString().ShouldEqual("{ Message = " + status.Message + " }");
                }
                else
                {
                    result.StatusCode.ShouldEqual(validStatusCode);
                    result.Value.ToString().ShouldEqual("{ Message = " + status.Message + ", results = " + results + " }");
                }
            }
            else
            {
                CheckErrorResponse(actionResult.Result as ObjectResult, status.Errors);
            }
        }

        //----------------------------------------------------
        //private

        private static void CheckErrorResponse(ObjectResult result, IEnumerable<ValidationResult> validationResults)
        {
            Assert.NotNull(result);
            result.StatusCode.ShouldEqual(400);
            var expectedDict = FormExpectedErrorResponse(validationResults);
            var dict = (Dictionary<string, object>)result.Value;
            dict.Count.ShouldEqual(expectedDict.Keys.Count);
            foreach (var propertyName in expectedDict.Keys)
            {
                dict.ContainsKey(propertyName).ShouldBeTrue($"missing entry for property {propertyName}");
                var dictErrors = ((string[])dict[propertyName]);
                dictErrors.Length.ShouldEqual(expectedDict[propertyName].Count);
                for (int i = 0; i < dictErrors.Length; i++)
                {
                    dictErrors[i].ShouldEqual(expectedDict[propertyName][i]);
                }
            }
        }

        private static Dictionary<string, List<string>> FormExpectedErrorResponse(IEnumerable<ValidationResult> validationResults)
        {
            var dict = new Dictionary<string, List<string>>();
            foreach (var validationResult in validationResults)
            {
                if (validationResult.MemberNames.Any())
                    foreach (var propertyName in validationResult.MemberNames)
                    {
                        dict.AddToDictionary(propertyName, validationResult.ErrorMessage);
                    }
                else
                {
                    dict.AddToDictionary("", validationResult.ErrorMessage);
                }
            }

            return dict;
        }

        private static void AddToDictionary(this Dictionary<string, List<string>> dict, 
            string propertyName, string errorMessage)
        {
            if (!dict.ContainsKey(propertyName))
                dict[propertyName] = new List<string>();
            dict[propertyName].Add(errorMessage);
        }

    }
}