// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using StatusGeneric;

namespace GenericServices.AspNetCore
{
    /// <summary>
    /// This is used to return a message in the response
    /// </summary>
    public class WebApiMessageOnly
    {

        /// <summary>
        /// This is used to create a Message-only response from new GenericServices
        /// </summary>
        /// <param name="status"></param>
        public WebApiMessageOnly(IStatusGeneric status)
        {
            Message = status.Message;
        }

        /// <summary>
        /// Contains the message taken from the status
        /// </summary>
        public string Message { get;}
    }
}