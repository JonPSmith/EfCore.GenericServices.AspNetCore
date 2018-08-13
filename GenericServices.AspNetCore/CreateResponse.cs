// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GenericServices.AspNetCore
{
    /// <summary>
    /// This provides useful methods to turn IStatusGeneric into WebAPI responses
    /// </summary>
    public static class CreateResponse
    {
        private const int OkStatusCode = 200;

        /// <summary>
        /// This will return a HTTP 200 with the status message if Valid,
        /// otherwise it will returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static IActionResult Response(this GenericServices.IStatusGeneric status)
        {
            return status.ResponseWithValidCode(OkStatusCode);
        }

        /// <summary>
        /// If the status has no errors then it will HTTP response with the status code provided in the
        /// validStatusCode property and the message from the status
        /// otherwise it will returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <param name="status"></param>
        /// <param name="validStatusCode">HTTP status code for non-error status</param>
        /// <returns></returns>
        public static IActionResult ResponseWithValidCode(this GenericServices.IStatusGeneric status, int validStatusCode)
        {
            if (status.IsValid)
                return new ObjectResult(new WebApiMessageOnly(status)) { StatusCode = validStatusCode };

            //it has errors
            return CreateBadRequestObjectResult(status.Errors.Select(x => x.ErrorResult));
        }

        /// <summary>
        /// This will return a result value, with the status Message
        /// 1. If there are no errors and the results is not null it will return a HTTP 200 response
        ///    plus a json containing the message from the status and the results object
        /// 2. If there are no errors but result is  null it will return a HTTP 404 (NotFound) with the status Message
        /// 3. If there are errors it returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="status"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public static ActionResult<T> Response<T>(this GenericServices.IStatusGeneric status, T results) 
        {
            return status.ResponseWithValidCode(results, OkStatusCode);
        }

        /// <summary>
        /// This will return a result value, with the status Message
        /// 1. If there are no errors and the result is not null it will return a HTTP response with the status code provided
        ///    in the validStatusCode property, plus a json containing the message from the status and the results object
        /// 2. If there are no errors but result is  null it will return a HTTP 404 (NotFound) with the status Message
        /// 3. If there are errors it returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="status"></param>
        /// <param name="results"></param>
        /// <param name="validStatusCode">The status code to return when the status has no errors and the result is not null</param>
        /// <param name="nullResultStatusCode">Optional, default is 404: The status code to return if the there ar no errors, but the result is null</param>
        /// <returns></returns>
        public static ActionResult<T> ResponseWithValidCode<T>(this GenericServices.IStatusGeneric status, T results, 
            int validStatusCode, int nullResultStatusCode = 404)
        {
            if (status.IsValid)
                return new ObjectResult(new WebApiMessageAndResult<T>( status, results ))
                        { StatusCode = results == null ? nullResultStatusCode : validStatusCode };

            //it has errors
            return CreateBadRequestObjectResult(status.Errors.Select(x => x.ErrorResult));
        }

        //-----------------------------------------------------
        // -- Now the GenericBizRunner versions

        /// <summary>
        /// This will return a HTTP 200 with the status message if Valid,
        /// otherwise it will returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static IActionResult Response(this GenericBizRunner.IStatusGeneric status)
        {
            return status.ResponseWithValidCode(OkStatusCode);
        }

        /// <summary>
        /// If the status has no errors then it will HTTP response with the status code provided in the
        /// validStatusCode property and the message from the status
        /// otherwise it will returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <param name="status"></param>
        /// <param name="validStatusCode">HTTP status code for non-error status</param>
        /// <returns></returns>
        public static IActionResult ResponseWithValidCode(this GenericBizRunner.IStatusGeneric status, int validStatusCode)
        {
            if (!status.HasErrors)
                return new ObjectResult(new WebApiMessageOnly(status)) { StatusCode = validStatusCode };

            //it has errors
            return CreateBadRequestObjectResult(status.Errors);
        }

        /// <summary>
        /// This will return a result value, with the status Message
        /// 1. If there are no errors and the result is not null it will return a HTTP 200 response
        ///    plus a json containing the message from the status and the results object
        /// 2. If there are no errors but result is  null it will return a HTTP 404 (NotFound) with the status Message
        /// 3. If there are errors it returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="status"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public static ActionResult<T> Response<T>(this GenericBizRunner.IStatusGeneric status, T results)
        {
            return status.ResponseWithValidCode(results, OkStatusCode);
        }

        /// <summary>
        /// This will return a result value, with the status Message
        /// 1. If there are no errors and the result is not null it will return a HTTP response with the status code provided
        ///    in the validStatusCode property, plus a json containing the message from the status and the results object
        /// 2. If there are no errors but result is  null it will return a HTTP 404 (NotFound) with the status Message
        /// 3. If there are errors it returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="status"></param>
        /// <param name="results"></param>
        /// <param name="validStatusCode">The status code to return when the status has no errors and the result is not null</param>
        /// <param name="nullResultStatusCode">Optional, default is 404: The status code to return if the there ar no errors, but the result is null</param>
        /// <returns></returns>
        public static ActionResult<T> ResponseWithValidCode<T>(this GenericBizRunner.IStatusGeneric status, T results,
            int validStatusCode, int nullResultStatusCode = 404)
        {
            if (!status.HasErrors)
                return new ObjectResult(new WebApiMessageAndResult<T>(status, results))
                    { StatusCode = results == null ? nullResultStatusCode : validStatusCode };

            //it has errors
            return CreateBadRequestObjectResult(status.Errors);
        }

        //---------------------------------------------------
        //private 

        private static BadRequestObjectResult CreateBadRequestObjectResult(IEnumerable<ValidationResult> validationResults)
        {
            //I copy the errors to a ModelState as BadRequestObjectResult has a version that turns the ModelStae into its standard error format
            var modelState = new ModelStateDictionary();
            foreach (var validationResult in validationResults)
            {
                if (validationResult.MemberNames.Any())
                    foreach (var propertyName in validationResult.MemberNames)
                    {
                        modelState.AddModelError(propertyName, validationResult.ErrorMessage);
                    }
                else
                {
                    modelState.AddModelError("", validationResult.ErrorMessage);
                }
            }
            return new BadRequestObjectResult(modelState);
        }
    }
}