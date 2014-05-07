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
// <link>http://developers.klarna.com/</link>
// ----------------------------------------------------------------------------
#endregion
namespace Klarna.Checkout.HTTP
{
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization;

    /// <summary>
    /// The http response.
    /// </summary>
    [Serializable]
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

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponse"/> class.
        /// </summary>
        /// <param name="code">The status code of this response.</param>
        /// <param name="headers">The headers associated with this response.</param>
        /// <param name="data">The payload for this response.</param>
        internal HttpResponse(HttpStatusCode code, WebHeaderCollection headers, string data)
        {
            StatusCode = code;
            this.headers = headers;
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponse"/> class.
        /// </summary>
        /// <param name="info">
        /// The object which this HttpResponse will be deserialized from.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        internal HttpResponse(SerializationInfo info, StreamingContext context)
        {
            headers = (WebHeaderCollection)info.GetValue("Headers", typeof(WebHeaderCollection));
            Data = (string)info.GetValue("Data", typeof(string));
            StatusCode = (HttpStatusCode)info.GetValue("StatusCode", typeof(HttpStatusCode));
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

        /// <summary>
        /// Serializes this instance into the specified SerializationInfo object.
        /// </summary>
        /// <param name="info">
        /// The object into which this HttpResponse will be serialized.
        /// </param>
        /// <param name="context">
        /// The destination of the serialization.
        /// </param>
        void System.Runtime.Serialization.ISerializable.GetObjectData(
            SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new System.ArgumentNullException("info");
            }

            info.AddValue("Headers", headers);
            info.AddValue("Data", Data);
            info.AddValue("StatusCode", StatusCode);
        }
    }
}