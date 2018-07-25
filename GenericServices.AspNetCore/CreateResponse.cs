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
        /// <summary>
        /// This will return a HTTP 200 with the status message if Valid,
        /// otherwise it will returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static IActionResult Response(this GenericServices.IStatusGeneric status)
        {
            if (status.IsValid)
                return new OkObjectResult(new { status.Message });

            //it has errors
            return CreateBadRequestObjectResult(status.Errors.Select(x => x.ErrorResult));
        }

        /// <summary>
        /// This will return a result value, with the status Message
        /// If there are no errors and the result is not null it will return a HTTP 200 with a json result containing the status Message and the results
        /// If there are no errors but result is  null it will return a HTTP 404 (NotFound) with the status Message
        /// If there are errors it returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="status"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public static ActionResult<T> Response<T>(this GenericServices.IStatusGeneric status, T results) 
        {
            if (status.IsValid)
                return results != null
                    ? (ActionResult<T>)new OkObjectResult(new { status.Message, results })
                    : (ActionResult<T>)new NotFoundObjectResult(new { status.Message });

            //it has errors
            return CreateBadRequestObjectResult(status.Errors.Select(x => x.ErrorResult));
        }

        /// <summary>
        /// This will return a HTTP 200 with the status message if Valid,
        /// otherwise it will returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static IActionResult Response(this GenericBizRunner.IStatusGeneric status)
        {
            if (!status.HasErrors)
                return new OkObjectResult(new { status.Message });

            //it has errors
            return CreateBadRequestObjectResult(status.Errors);
        }

        /// <summary>
        /// This will return a result value, with the status Message
        /// If there are no errors and the result is not null it will return a HTTP 200 with a json result containing the status Message and the results
        /// If there are no errors but result is  null it will return a HTTP 404 (NotFound) with the status Message
        /// If there are errors it returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="status"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public static ActionResult<T> Response<T>(this GenericBizRunner.IStatusGeneric status, T results)
        {
            if (!status.HasErrors)
                return results != null
                    ? (ActionResult<T>)new OkObjectResult(new { status.Message, results })
                    : (ActionResult<T>)new NotFoundObjectResult(new { status.Message });

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