// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

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
        public static IActionResult Response(this IStatusGeneric status)
        {
            if (status.IsValid)
                return new OkObjectResult(new { status.Message });

            //it has errors
            return CreateBadRequestObjectResult(status);
        }

        /// <summary>
        /// This will return a result value, with the status Message
        /// If there are no errors and the result is not null it will return a HTTP 200 with a json result containing the status Message and the results
        /// If there are no errors but result is  null it will return a HTTP 404 the status Message
        /// If there are errors it returns a HTTP 400 with the error information in the standard WebAPI format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="status"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public static ActionResult<T> Response<T>(this IStatusGeneric status, T results) 
        {
            if (status.IsValid)
                return results != null
                    ? (ActionResult<T>)new OkObjectResult(new { status.Message, results })
                    : (ActionResult<T>)new NotFoundObjectResult(new { status.Message });

            //it has errors
            return CreateBadRequestObjectResult(status);
        }

        //---------------------------------------------------
        //private 

        private static BadRequestObjectResult CreateBadRequestObjectResult(IStatusGeneric status)
        {
            //I copy the errors to a ModelState as BadRequestObjectResult has a version that turns the ModelStae into its standard error format
            var modelState = new ModelStateDictionary();
            foreach (var error in status.Errors)
                modelState.AddModelError("", error.ToString());
            return new BadRequestObjectResult(modelState);
        }
    }
}