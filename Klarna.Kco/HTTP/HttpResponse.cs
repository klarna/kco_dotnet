#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="HttpResponse.cs" company="Klarna AB">
//     Copyright 2012 Klarna AB
//  
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
//  
//         http://www.apache.org/licenses/LICENSE-2.0
//  
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
    using System.IO;
    using System.Net;

    /// <summary>
    /// The http response.
    /// </summary>
    public class HttpResponse : IHttpResponse
    {
        #region Private Fields

        /// <summary>
        /// The response headers.
        /// </summary>
        private readonly WebHeaderCollection headers;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponse"/> class.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        internal HttpResponse(HttpWebResponse response)
        {
            StatusCode = response.StatusCode;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                Data = reader.ReadToEnd();
            }

            headers = response.Headers;
        }

        #endregion

        #region Implementation of IHttpResponse

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Gets the response data.
        /// </summary>
        public string Data { get; private set; }

        /// <summary>
        /// Gets the specified HTTP header.
        /// </summary>
        /// <param name="name">
        /// The name of the header.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Header(string name)
        {
            return headers[name];
        }

        #endregion
    }
}