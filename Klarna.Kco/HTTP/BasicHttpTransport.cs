#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="BasicHttpTransport.cs" company="Klarna AB">
//     Copyright 2012 Klarna AB
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
//         http://www.apache.org/licenses/LICENSE-2.0
//     Unless required by applicable law or agreed to in writing, software
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
// </copyright>
// <author>Klarna Support: support@klarna.com</author>
// <link>http://integration.klarna.com/</link>
// ----------------------------------------------------------------------------
#endregion
namespace Klarna.Checkout.HTTP
{
    using System;
    using System.Net;

    /// <summary>
    /// The basic http transport.
    /// </summary>
    public class BasicHttpTransport : IHttpTransport
    {
        #region Implementation of IHttpTransport

        /// <summary>
        /// Gets or sets the number of seconds before the connection times out.
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Creates a HttpWebRequest object.
        /// </summary>
        /// <param name="url">
        /// The request URL.
        /// </param>
        /// <returns>
        /// The <see cref="HttpWebRequest"/>.
        /// </returns>
        public HttpWebRequest CreateRequest(Uri url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            return request;
        }

        /// <summary>
        /// Performs a HTTP request.
        /// </summary>
        /// <param name="request">
        /// The HTTP request to send.
        /// </param>
        /// <param name="payload">
        /// The payload to send if this is a POST.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpResponse"/>.
        /// </returns>
        public IHttpResponse Send(HttpWebRequest request, string payload)
        {
            if (request.Method == "POST")
            {
                // payLoad
            }

            return null;
        }

        #endregion
    }
}