// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using StatusGeneric;

namespace GenericServices.AspNetCore
{
    /// <summary>
    /// This provides useful methods to turn IStatusGeneric into WebAPI responses
    /// </summary>
    public static class CreateResponse
    {
        public const int OkStatusCode = 200;
        public const int CreatedStatusCode = 201;
        public const int ResultIsNullStatusCode = 204;
        public const int ErrorsStatusCode = 400;

        /// <summary>
        /// This will return a HTTP 200 with the status message if Valid,
        /// otherwise it will returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static ActionResult<WebApiMessageOnly> Response(this IStatusGeneric status)
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
        public static ActionResult<WebApiMessageOnly> ResponseWithValidCode(this IStatusGeneric status, int validStatusCode)
        {
            if (status.IsValid)
                return new ObjectResult(new WebApiMessageOnly(status)) { StatusCode = validStatusCode };

            //it has errors
            return CreateBadRequestObjectResult(status.Errors.Select(x => x.ErrorResult));
        }

        /// <summary>
        /// This allows you to return a CreatedAtRoute result for a Create
        /// </summary>
        /// <param name="status"></param>
        /// <param name="createdRoute"></param>
        /// <returns></returns>
        public static IActionResult Response(this IStatusGeneric status, CreatedAtRouteResult createdRoute)
        {
            if (status.IsValid)
                return createdRoute;

            //it has errors
            return CreateBadRequestObjectResult(status.Errors.Select(x => x.ErrorResult));
        }

        /// <summary>
        /// This allows you to return a CreatedAtRoute result for a Create
        /// </summary>
        /// <param name="status"></param>
        /// <param name="controller"></param>
        /// <param name="routeName">The values needed to work with the HttpGet to return the correct item</param>
        /// <param name="routeValues"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static ActionResult<T> Response<T>(this IStatusGeneric status,
            ControllerBase controller, string routeName, object routeValues, T dto)
        {
            if (status.IsValid)
                return controller.CreatedAtRoute(routeName, routeValues, dto);

            //it has errors
            return CreateBadRequestObjectResult(status.Errors.Select(x => x.ErrorResult));
        }

        /// <summary>
        /// This will return a result value, with the status Message. There are three possibilities:
        /// 1. If there are no errors and the results is not null it will return a HTTP 200 response
        ///    plus a json containing the message from the status and the results object
        /// 2. If there are no errors but result is  null it will return a HTTP 204 (NoContent) with the status Message
        /// 3. If there are errors it returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="status"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public static ActionResult<WebApiMessageAndResult<T>> Response<T>(this IStatusGeneric status, T results) 
        {
            return status.ResponseWithValidCode(results, OkStatusCode);
        }

        /// <summary>
        /// This will return a result value, with the status Message. There are three possibilities:
        /// 1. If there are no errors and the result is not null it will return a HTTP response with the status code provided
        ///    in the validStatusCode property, plus a json containing the message from the status and the results object
        /// 2. If there are no errors but result is  null it will return a HTTP 204 (NoContent) with the status Message
        /// 3. If there are errors it returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="status"></param>
        /// <param name="results"></param>
        /// <param name="validStatusCode">The status code to return when the status has no errors and the result is not null</param>
        /// <param name="nullResultStatusCode">Optional, default is 204: The status code to return if the there ar no errors, but the result is null</param>
        /// <returns></returns>
        public static ActionResult<WebApiMessageAndResult<T>> ResponseWithValidCode<T>(this IStatusGeneric status, T results, 
            int validStatusCode, int nullResultStatusCode = ResultIsNullStatusCode)
        {
            if (status.IsValid)
                return new ObjectResult(new WebApiMessageAndResult<T>( status, results ))
                        { StatusCode = results == null ? nullResultStatusCode : validStatusCode };

            //it has errors
            return CreateBadRequestObjectResult(status.Errors.Select(x => x.ErrorResult));
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