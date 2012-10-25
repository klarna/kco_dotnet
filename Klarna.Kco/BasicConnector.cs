#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="BasicConnector.cs" company="Klarna AB">
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
namespace Klarna.Checkout
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Klarna.Checkout.HTTP;
    using Newtonsoft.Json;

    /// <summary>
    /// The basic connector.
    /// </summary>
    public class BasicConnector : IConnector
    {
        #region Private Fields

        /// <summary>
        /// The IHttpTransport interface implementation.
        /// </summary>
        private readonly IHttpTransport httpTransport;

        /// <summary>
        /// The digest.
        /// </summary>
        private readonly Digest digest;

        /// <summary>
        /// The secret used to sign requests.
        /// </summary>
        private readonly string secret;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicConnector"/> class.
        /// </summary>
        /// <param name="httpTransport">
        /// The http transport.
        /// </param>
        /// <param name="digest">
        /// The digest.
        /// </param>
        /// <param name="secret">
        /// The secret.
        /// </param>
        public BasicConnector(IHttpTransport httpTransport, Digest digest, string secret)
        {
            this.httpTransport = httpTransport;
            this.digest = digest;
            this.secret = secret;

            UserAgent = new UserAgent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the user agent used for User-Agent header.
        /// </summary>
        public UserAgent UserAgent { get; set; }

        #endregion

        #region Implementation of IConnector

        /// <summary>
        /// Applies a HTTP method on a specific resource.
        /// </summary>
        /// <param name="method">
        /// The HTTP method.
        /// </param>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        public void Apply(HttpMethod method, IResource resource, Dictionary<string, object> options)
        {
            Handle(method, resource, options, new List<Uri>());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles a HTTP request.
        /// </summary>
        /// <param name="method">
        /// The HTTP method to use.
        /// </param>
        /// <param name="resource">
        /// The resource to use.
        /// </param>
        /// <param name="options">
        /// The options to use.
        /// </param>
        /// <param name="visitedUrl">
        /// List of visited url.
        /// </param>
        private void Handle(HttpMethod method, IResource resource,
            Dictionary<string, object> options, List<Uri> visitedUrl)
        {
            var url = ResolveUrl(resource, options);

            var payLoad = string.Empty;
            if (method == HttpMethod.Post)
            {
                var resorceData = resource.Marshal();
                payLoad = JsonConvert.SerializeObject(resorceData);
            }

            var request = CreateRequest(resource, method, payLoad, url);

            var response = httpTransport.Send(request, payLoad);

            VerifyResponse(response);

            //// Handle statuses appropriately.
            // return $this->handleResponse($result, $resource, $visited);
        }

        /// <summary>
        /// Resolve the url to use, from options or resource.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The <see cref="Uri"/>.
        /// </returns>
        private Uri ResolveUrl(IResource resource, Dictionary<string, object> options)
        {
            const string UrlKey = "url";
            Uri url;
            if (options != null && options.ContainsKey(UrlKey))
            {
                url = options[UrlKey] as Uri;
            }
            else
            {
                url = resource.Location;
            }

            return url;
        }

        /// <summary>
        /// Creates a request.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <param name="method">
        /// The HTTP method to use.
        /// </param>
        /// <param name="payLoad">
        /// The pay load.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="HttpWebRequest"/>.
        /// </returns>
        private HttpWebRequest CreateRequest(IResource resource,
            HttpMethod method, string payLoad, Uri url)
        {
            // Create the request with correct method to use
            var request = httpTransport.CreateRequest(url);
            request.Method = method.ToString().ToUpper();

            // Set HTTP Headers
            request.UserAgent = UserAgent.ToString();

            var digestString = digest.Create(string.Concat(payLoad, secret));
            var authorization = string.Format("Klarna {0}", digestString);
            request.Headers.Add("Authorization", authorization);

            request.Accept = resource.ContentType;

            if (payLoad.Length > 0)
            {
                request.ContentType = resource.ContentType;
            }

            return request;
        }

        /// <summary>
        /// Method to verify the response.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <exception cref="ConnectorException">
        /// Thrown if response HTTP status code is error codes 4xx or 5xx.
        /// </exception>
        private void VerifyResponse(IHttpResponse response)
        {
            var statusCode = (int)response.StatusCode;
            if (statusCode >= 400 && statusCode <= 599)
            {
                var exception = new ConnectorException();
                exception.Data["HttpStatusCode"] = response.StatusCode;

                throw exception;
            }
        }

        #endregion
    }
}