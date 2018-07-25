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
            actionResult.ShouldNotBeNull();
            if (status.IsValid)
            {
                actionResult.ShouldNotBeNull();
                var result = actionResult as OkObjectResult;
                Assert.NotNull(result);
                result.StatusCode.ShouldEqual(200);
                result.Value.ToString().ShouldEqual("{ Message = "+ status.Message + " }");
            }
            else
            {
                CheckErrorResponse(actionResult as BadRequestObjectResult, status.Errors.Select(x => x.ErrorResult));
            }
        }

        public static void CheckResponse<T>(this ActionResult<T> actionResult, GenericServices.IStatusGeneric status, T results)
        {
            actionResult.ShouldNotBeNull();
            if (status.IsValid)
            {
                if (results == null)
                {
                    var result = actionResult.Result as NotFoundObjectResult;
                    result.ShouldNotBeNull();
                    result.StatusCode.ShouldEqual(404);
                    result.Value.ToString().ShouldEqual("{ Message = " + status.Message + " }");
                }
                else
                {
                    var result = actionResult.Result as OkObjectResult;
                    result.ShouldNotBeNull();
                    result.StatusCode.ShouldEqual(200);
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
            actionResult.ShouldNotBeNull();
            if (!status.HasErrors)
            {
                actionResult.ShouldNotBeNull();
                var result = actionResult as OkObjectResult;
                Assert.NotNull(result);
                result.StatusCode.ShouldEqual(200);
                result.Value.ToString().ShouldEqual("{ Message = " + status.Message + " }");
            }
            else
            {
                CheckErrorResponse(actionResult as BadRequestObjectResult, status.Errors);
            }
        }

        public static void CheckResponse<T>(this ActionResult<T> actionResult, GenericBizRunner.IStatusGeneric status, T results)
        {
            actionResult.ShouldNotBeNull();
            if (!status.HasErrors)
            {
                if (results == null)
                {
                    var result = actionResult.Result as NotFoundObjectResult;
                    result.ShouldNotBeNull();
                    result.StatusCode.ShouldEqual(404);
                    result.Value.ToString().ShouldEqual("{ Message = " + status.Message + " }");
                }
                else
                {
                    var result = actionResult.Result as OkObjectResult;
                    result.ShouldNotBeNull();
                    result.StatusCode.ShouldEqual(200);
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