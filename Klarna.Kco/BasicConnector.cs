#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="BasicConnector.cs" company="Klarna AB">
//     Copyright 2014 Klarna AB
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
namespace Klarna.Checkout
{
    using System;
    using System.Collections.Generic;
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

        /// <summary>
        /// Gets the transport used for the HTTP communications.
        /// </summary>
        public IHttpTransport Transport
        {
            get
            {
                return this.httpTransport;
            }
        }

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
        /// <returns>
        /// The <see cref="IHttpResponse"/>.
        /// </returns>
        private IHttpResponse Handle(HttpMethod method, IResource resource,
            Dictionary<string, object> options, List<Uri> visitedUrl)
        {
            var url = GetUrl(resource, options);

            var payLoad = string.Empty;
            if (method == HttpMethod.Post)
            {
                var resorceData = GetData(resource, options);
                payLoad = JsonConvert.SerializeObject(resorceData);
            }

            IHttpResponse response = null;
            var request = CreateRequest(resource, method, payLoad, url);
            try
            {
                response = httpTransport.Send(request, payLoad);

                return HandleResponse(response, method, resource, visitedUrl);
            }
            catch (WebException ex)
            {
                var message = ex.Message;
                if (ex.Response != null)
                {
                    response = new HttpResponse((HttpWebResponse)ex.Response);
                    message = response.StatusCode.ToString();
                }

                throw CreateException(message, response, ex);
            }
        }

        /// <summary>
        /// Gets the url to use, from options or resource.
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
        private Uri GetUrl(IResource resource, Dictionary<string, object> options)
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
        /// Gets data to use, from options or resource.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The data.
        /// </returns>
        private object GetData(IResource resource, Dictionary<string, object> options)
        {
            const string DataKey = "data";
            Dictionary<string, object> data;
            if (options != null && options.ContainsKey(DataKey))
            {
                data = options[DataKey] as Dictionary<string, object>;
            }
            else
            {
                data = resource.Marshal();
            }

            return data;
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

            request.Accept = (resource.Accept != null) ? resource.Accept : resource.ContentType;

            if (payLoad.Length > 0)
            {
                request.ContentType = resource.ContentType;
            }

            return request;
        }

        /// <summary>
        /// Handle response based on status.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="method">
        /// The HTTP method.
        /// </param>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <param name="visitedUrl">
        /// List of visited locations.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpResponse"/>.
        /// </returns>
        private IHttpResponse HandleResponse(IHttpResponse response, HttpMethod method,
            IResource resource, List<Uri> visitedUrl)
        {
            VerifyResponse(response);

            var location = response.Header("Location");
            Uri url = null;
            if (!string.IsNullOrEmpty(location))
            {
                url = new Uri(location);
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK: // 200 - Update Data on resource.
                    try
                    {
                        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Data);
                        resource.Parse(data);
                    }
                    catch (Exception ex)
                    {
                        throw new ConnectorException("Bad format on response content.", ex);
                    }

                    break;

                case HttpStatusCode.Created: // 201 - Update location.
                    resource.Location = url;
                    break;

                case HttpStatusCode.MovedPermanently: // 301 - Update location and redirect if method is GET.
                    resource.Location = url;
                    if (method == HttpMethod.Get)
                    {
                        return MakeRedirect(resource, visitedUrl, url);
                    }

                    break;

                case HttpStatusCode.Found: // 302 - Redirect if method is GET.
                    if (method == HttpMethod.Get)
                    {
                        return MakeRedirect(resource, visitedUrl, url);
                    }

                    break;

                case HttpStatusCode.SeeOther: // 303 - Redirect with GET, even if request is POST.
                    return MakeRedirect(resource, visitedUrl, url);
            }

            return response;
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
            if (statusCode < 400 || statusCode > 599)
            {
                return;
            }

            throw CreateException(response.StatusCode.ToString(), response, null);
        }

        /// <summary>
        /// Creates a connector exception and populates it.
        /// </summary>
        /// <param name="message">
        /// The exception message.
        /// </param>
        /// <param name="response">
        /// The response object that caused this exception.
        /// </param>
        /// <param name="ex">
        /// The inner exception.
        /// </param>
        /// <returns>
        /// A ConnectorException with the response data assigned.
        /// </returns>
        private ConnectorException CreateException(string message, IHttpResponse response, WebException ex)
        {
            var exception = new ConnectorException(message, ex);

            if (response == null)
            {
                return exception;
            }

            var contentType = response.Header("Content-Type");
            if (!string.IsNullOrEmpty(contentType)
                && (contentType.EndsWith("+json") || contentType.Equals("application/json")))
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Data);
                    foreach (KeyValuePair<string, object> kvp in data)
                    {
                        exception.Data[kvp.Key] = kvp.Value;
                    }
                }
                catch (Exception)
                {
                    // Ignore the content if parsing it failed.
                }
            }

            exception.Data["HttpStatusCode"] = response.StatusCode; // For backward compatibility.
            exception.Data["Response"] = response;

            return exception;
        }

        /// <summary>
        /// Makes a redirect.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <param name="visitedUrl">
        /// List of visited locations.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpResponse"/>.
        /// </returns>
        /// <exception cref="ConnectorException">
        /// Thrown if infinite redirect loop is detected.
        /// </exception>
        private IHttpResponse MakeRedirect(IResource resource, List<Uri> visitedUrl, Uri url)
        {
            if (visitedUrl.Contains(url))
            {
                throw new ConnectorException("Infinite redirect loop detected.");
            }

            visitedUrl.Add(url);

            var options = new Dictionary<string, object> { { "url", url } };
            return Handle(HttpMethod.Get, resource, options, visitedUrl);
        }

        #endregion
    }
}