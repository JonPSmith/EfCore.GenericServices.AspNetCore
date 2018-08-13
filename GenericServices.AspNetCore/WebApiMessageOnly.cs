// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace GenericServices.AspNetCore
{
    /// <summary>
    /// This is used to return a message in the response
    /// </summary>
    public class WebApiMessageOnly
    {
        /// <summary>
        /// This is used to create a Message-only response from GenericServices
        /// </summary>
        /// <param name="status"></param>
        public WebApiMessageOnly(GenericServices.IStatusGeneric status)
        {
            Message = status.Message;
        }

        /// <summary>
        /// This is used to create a Message-only response from GenericBizRunner
        /// </summary>
        /// <param name="status"></param>
        public WebApiMessageOnly(GenericBizRunner.IStatusGeneric status)
        {
            Message = status.Message;
        }

        /// <summary>
        /// Contains the message taken from the status
        /// </summary>
        public string Message { get;}
    }
}