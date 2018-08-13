// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace GenericServices.AspNetCore
{
    /// <summary>
    /// This is used to return a message in the response
    /// </summary>
    public class WebApiMessageAndResult<T>
    {
        /// <summary>
        /// This is used to create a Message-plus-results response from GenericServices
        /// </summary>
        /// <param name="status"></param>
        /// <param name="results"></param>
        public WebApiMessageAndResult(GenericServices.IStatusGeneric status, T results)
        {
            Message = status.Message;
            Results = results;
        }

        /// <summary>
        /// This is used to create a Message-plus-results  response from GenericBizRunner
        /// </summary>
        /// <param name="status"></param>
        public WebApiMessageAndResult(GenericBizRunner.IStatusGeneric status, T results)
        {
            Message = status.Message;
            Results = results;
        }

        /// <summary>
        /// Contains the message taken from the status
        /// </summary>
        public string Message { get;}

        /// <summary>
        /// The data sent by the Web API
        /// </summary>
        public T Results { get; }
    }
}