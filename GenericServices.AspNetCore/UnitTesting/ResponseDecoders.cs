// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GenericServices.AspNetCore.UnitTesting
{
    /// <summary>
    /// This contains extention methods to decode the response so that you can unit test/integeration test your Web API code
    /// </summary>
    public static class ResponseDecoders
    {
        /// <summary>
        /// This extracts the status code from the ActionResult{T}
        /// </summary>
        /// <param name="actionResult"></param>
        /// <returns>an int - throws exceptions if status is null</returns>
        public static int GetStatusCode<T>(this ActionResult<T> actionResult)
        {
            var objResult = (actionResult.Result as ObjectResult);
            if (objResult == null)
                throw new NullReferenceException("Could not cast the response to ObjectResult");
            return objResult.StatusCode ?? throw new NullReferenceException("Status Code as null");
        }

        /// <summary>
        /// This converts the <see cref="ActionResult{WebApiMessageOnly}"/> created by <see cref="CreateResponse"/> into a GenericServices.IStatusGeneric
        /// </summary>
        /// <param name="actionResult"></param>
        /// <returns>a status which is similar to the original status (errors might not be in the exact same form)</returns>
        public static IStatusGeneric CopyToStatus(this ActionResult<WebApiMessageOnly> actionResult)
        {
            var testStatus = new StatusGenericHandler();
            var objResult = (actionResult.Result as ObjectResult);
            if (objResult == null)
                throw new NullReferenceException("Could not cast the response to ObjectResult");
            var errorPart = objResult as BadRequestObjectResult;
            if (errorPart != null)
            {
                //It has errors, so copy errors to status
                testStatus.AddValidationResults(ExtractErrors(errorPart));
                return testStatus;
            }

            var decodedValue = objResult.Value as WebApiMessageOnly;
            if (decodedValue == null)
                throw new NullReferenceException($"Could not cast the response value to WebApiMessageOnly");
            testStatus.Message = decodedValue.Message;
            return testStatus;
        }

        /// <summary>
        /// This converts the <see cref="ActionResult{T}"/> used in a create action into a GenericServices.IStatusGeneric
        /// </summary>
        /// <param name="actionResult"></param>
        /// <param name="routeName"></param>
        /// <param name="routeValues"></param>
        /// <param name="dto"></param>
        /// <returns>a status which is similar to the original status (errors might not be in the exact same form)</returns>
        public static IStatusGeneric CheckCreateResponse<T>(this ActionResult<T> actionResult, string routeName, object routeValues, T dto)
            where T : class
        {
            var testStatus = new StatusGenericHandler();
            var objResult = (actionResult.Result as ObjectResult);
            if (objResult == null)
                throw new NullReferenceException("Could not cast the response to ObjectResult");
            var errorPart = objResult as BadRequestObjectResult;
            if (errorPart != null)
            {
                testStatus.Message = "Errors: " + string.Join("\n", ExtractErrors(errorPart));
                return testStatus;
            }

            var createdAtRouteResult = objResult as CreatedAtRouteResult;
            if (createdAtRouteResult == null)
                throw new NullReferenceException($"Could not cast the response value to CreatedAtRouteResult");
            if (createdAtRouteResult.RouteName != routeName)
                testStatus.AddError($"RouteName: expected {routeName}, found: {createdAtRouteResult.RouteName}");
            if (createdAtRouteResult.Value as T != dto)
                testStatus.AddError($"DTO: the returned DTO instance does not match the test DTO: expected {typeof(T).Name}, found: {createdAtRouteResult.Value.GetType().Name}");
            testStatus.CombineStatuses(CompareRouteValues(createdAtRouteResult.RouteValues, routeValues));

            return testStatus;
        }

        /// <summary>
        /// This converts the <see cref="ActionResult{WebApiMessageAndResult{T}}"/> created by <see cref="CreateResponse"/> into a GenericServices.IStatusGeneric
        /// </summary>
        /// <param name="actionResult"></param>
        /// <returns>a status which is similar to the original status (errors might not be in the exact same form)</returns>
        public static IStatusGeneric<T> CopyToStatus<T>(this ActionResult<WebApiMessageAndResult<T>> actionResult)
        {
            var testStatus = new StatusGenericHandler<T>();
            var objResult = (actionResult.Result as ObjectResult);
            if (objResult == null)
                throw new NullReferenceException("Could not cast the response to ObjectResult");
            var errorPart = objResult as BadRequestObjectResult;
            if (errorPart != null)
            {
                //It has errors, so copy errors to status
                testStatus.AddValidationResults(ExtractErrors(errorPart));
                return testStatus;
            }

            var decodedValue = objResult.Value as WebApiMessageAndResult<T>;
            if (decodedValue == null)
                throw new NullReferenceException($"Could not cast the response value to WebApiMessageAndResult<{typeof(T).Name}>");
            testStatus.Message = decodedValue.Message;
            testStatus.SetResult(decodedValue.Results);

            return testStatus;
        }

        //----------------------------------------------------
        //private

        private static IStatusGeneric CompareRouteValues(RouteValueDictionary foundValueRouteValues, object expectedRouteValues)
        {
            var expectedRouteValueDict = expectedRouteValues.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => new {key = x.Name, value = x.GetValue(expectedRouteValues)})
                .ToDictionary(x => x.key, y => y.value);

            var status = new StatusGenericHandler();
            if (string.Join(", ", expectedRouteValueDict.Keys) != string.Join(", ", foundValueRouteValues.Keys) )
                return status.AddError(
                    $"RouteValues: Different named properties: expected = {string.Join(",", expectedRouteValueDict.Keys)}, found = {string.Join(", ", foundValueRouteValues.Keys)}");
            foreach (var propName in expectedRouteValueDict.Keys)
            {
                var expectedValue = expectedRouteValueDict[propName];
                var foundValue = foundValueRouteValues[propName];
                if (expectedValue.GetType() != foundValue.GetType())
                    status.AddError(
                        $"RouteValues->{propName}, different type: expected = {expectedValue.GetType().Name}, found = {foundValue.GetType().Name}");
                else if (!Equals(expectedValue, foundValue))
                    status.AddError(
                        $"RouteValues->{propName}, different values: expected = {expectedValue}, found = {foundValue}");
            }

            return status;
        }

        private static IEnumerable<ValidationResult> ExtractErrors(BadRequestObjectResult result)
        {
            var errors = new List<ValidationResult>();
            var dict = (Dictionary<string, object>)result.Value;
            foreach (var propertyName in dict.Keys)
            {
                var dictErrors = ((string[])dict[propertyName]);
                errors.AddRange(dictErrors.Select(t => new ValidationResult(t, new[] {propertyName})));
            }

            return errors;
        }


    }
}