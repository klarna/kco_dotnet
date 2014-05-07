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
// <link>http://developers.klarna.com/</link>
// ----------------------------------------------------------------------------
#endregion
namespace Klarna.Checkout.HTTP
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// The basic http transport.
    /// </summary>
    public class BasicHttpTransport : IHttpTransport
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicHttpTransport"/> class.
        /// </summary>
        public BasicHttpTransport()
        {
            // Default timeout to 10 seconds
            Timeout = 10000;
        }

        #endregion

        #region Implementation of IHttpTransport

        /// <summary>
        /// Gets or sets the number of milliseconds before the connection times out.
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
            request.AllowAutoRedirect = false;
            request.Timeout = Timeout;
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
            ServicePointManager.ServerCertificateValidationCallback =
                ValidateServerCertificate;

            request.Timeout = Timeout;

            if (request.Method == "POST")
            {
                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                return new HttpResponse(response);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Verifies the remote Secure Sockets Layer (SSL) certificate used
        /// for authentication.
        /// </summary>
        /// <param name="sender">
        /// An object that contains state information for this validation.
        /// </param>
        /// <param name="certificate">
        /// The certificate used to authenticate the remote party.
        /// </param>
        /// <param name="chain">
        /// The chain of certificate authorities associated with the remote
        /// certificate.
        /// </param>
        /// <param name="sslPolicyErrors">
        /// One or more errors associated with the remote certificate.
        /// </param>
        /// <returns>
        /// A Boolean value that determines whether the specified certificate
        /// is accepted for authentication.
        /// </returns>
        private static bool ValidateServerCertificate(
            object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #endregion
    }
}