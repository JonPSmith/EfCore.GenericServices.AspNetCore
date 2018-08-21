// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.Helpers
{
    public static class ResponseCheckers
    {
        public static void CheckResponse(this ActionResult<WebApiMessageOnly> actionResult, GenericServices.IStatusGeneric status)
        {
            actionResult.CheckResponseWithValidCode(status, CreateResponse.OkStatusCode);
        }

        public static void CheckResponseWithValidCode(this ActionResult<WebApiMessageOnly> actionResult, GenericServices.IStatusGeneric status, int validStatusCode)
        {
            actionResult.ShouldNotBeNull();
            var result = actionResult.Result as ObjectResult;
            if (status.IsValid)
            {
                actionResult.ShouldNotBeNull();
                Assert.NotNull(result);
                result.StatusCode.ShouldEqual(validStatusCode);
                var returnClass = result.Value as WebApiMessageOnly;
                returnClass?.Message.ShouldEqual(status.Message);
            }
            else
            {
                CheckErrorResponse(result as BadRequestObjectResult, status.Errors.Select(x => x.ErrorResult));
            }
        }

        public static void CheckResponse<T>(this ActionResult<WebApiMessageAndResult<T>> actionResult, GenericServices.IStatusGeneric status, T results)
        {
            actionResult.CheckResponseWithValidCode<T>(status, results, CreateResponse.OkStatusCode);
        }

        public static void CheckResponseWithValidCode<T>(this ActionResult<WebApiMessageAndResult<T>> actionResult, GenericServices.IStatusGeneric status, T results, 
            int validStatusCode, int nullResultStatusCode = CreateResponse.ResultIsNullStatusCode)
        {
            actionResult.ShouldNotBeNull();
            var result = actionResult.Result as ObjectResult;
            if (status.IsValid)
            {
                result.ShouldNotBeNull();
                result.StatusCode.ShouldEqual(results == null ? nullResultStatusCode : validStatusCode);
                var returnClass = result.Value as WebApiMessageAndResult<T>;
                returnClass?.Message.ShouldEqual(status.Message);
                returnClass?.Results.ShouldEqual(results);
            }
            else
            {
                CheckErrorResponse(result as BadRequestObjectResult, status.Errors.Select(x => x.ErrorResult));
            }
        }

        //----------------------------------------------------
        //Now the GenericBixRunner

        public static void CheckResponse(this ActionResult<WebApiMessageOnly> actionResult, GenericBizRunner.IStatusGeneric status)
        {
            actionResult.CheckResponseWithValidCode(status, CreateResponse.OkStatusCode);
        }

        public static void CheckResponseWithValidCode(this ActionResult<WebApiMessageOnly> actionResult, GenericBizRunner.IStatusGeneric status, 
            int validStatusCode)
        {
            actionResult.ShouldNotBeNull();
            var result = actionResult.Result as ObjectResult;
            if (!status.HasErrors)
            {
                actionResult.ShouldNotBeNull();
                Assert.NotNull(result);
                result.StatusCode.ShouldEqual(validStatusCode);
                var returnClass = result.Value as WebApiMessageOnly;
                returnClass?.Message.ShouldEqual(status.Message);
            }
            else
            {
                CheckErrorResponse(result as BadRequestObjectResult, status.Errors);
            }
        }

        public static void CheckResponse<T>(this ActionResult<WebApiMessageAndResult<T>> actionResult, GenericBizRunner.IStatusGeneric status, T results)
        {
            actionResult.CheckResponseWithValidCode<T>(status, results, CreateResponse.OkStatusCode);
        }

        public static void CheckResponseWithValidCode<T>(this ActionResult<WebApiMessageAndResult<T>> actionResult, GenericBizRunner.IStatusGeneric status, T results, 
            int validStatusCode, int nullResultStatusCode = CreateResponse.ResultIsNullStatusCode)
        {
            actionResult.ShouldNotBeNull();
            var result = actionResult.Result as ObjectResult;
            if (!status.HasErrors)
            {
                Assert.NotNull(result);
                result.ShouldNotBeNull();
                result.StatusCode.ShouldEqual(results == null ? nullResultStatusCode : validStatusCode);
                var returnClass = result.Value as WebApiMessageOnly;
                returnClass?.Message.ShouldEqual(status.Message);
            }
            else
            {
                CheckErrorResponse(result as BadRequestObjectResult, status.Errors);
            }
        }

        //----------------------------------------------------
        //private

        private static void CheckErrorResponse(ObjectResult result, IEnumerable<ValidationResult> validationResults)
        {
            Assert.NotNull(result);
            result.StatusCode.ShouldEqual(CreateResponse.ErrorsStatusCode);
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