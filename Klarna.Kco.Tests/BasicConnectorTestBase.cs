#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="BasicConnectorTestBase.cs" company="Klarna AB">
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
namespace Klarna.Checkout.Tests
{
    using System;
    using System.Net;
    using Klarna.Checkout.HTTP;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// The basic connector test base.
    /// </summary>
    public class BasicConnectorTestBase
    {
        #region Fields

        /// <summary>
        /// The secret.
        /// </summary>
        protected const string Secret = "My Secret";

        /// <summary>
        /// The content type.
        /// </summary>
        protected const string ContentType = "application/vnd.klarna.checkout.aggregated-order-v2+json";

        /// <summary>
        /// The accept type.
        /// </summary>
        protected const string Accept = "application/vnd.klarna.checkout.recurring-order-accepted-v1+json";

        /// <summary>
        /// The url.
        /// </summary>
        protected readonly Uri Url = new Uri("http://klarna.com");

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the pay load.
        /// </summary>
        protected internal string PayLoad { get; set; }

        /// <summary>
        /// Gets or sets the http transport mock.
        /// </summary>
        protected internal Mock<IHttpTransport> HttpTransportMock { get; set; }

        /// <summary>
        /// Gets or sets the resource mock.
        /// </summary>
        protected internal Mock<IResource> ResourceMock { get; set; }

        /// <summary>
        /// Gets or sets the digest.
        /// </summary>
        protected internal Digest Digest { get; set; }

        /// <summary>
        /// Gets or sets the response mock.
        /// </summary>
        protected internal Mock<IHttpResponse> ResponseMock { get; set; }

        #endregion

        #region Set Up

        /// <summary>
        /// The set up before each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            PayLoad = string.Empty;
            HttpTransportMock = new Mock<IHttpTransport>();
            ResourceMock = new Mock<IResource>();
            Digest = new Digest();
            ResponseMock = new Mock<IHttpResponse>();
            ResponseMock.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.OK);
            ResponseMock.Setup(r => r.Header("Location")).Returns(Url.OriginalString);
        }

        #endregion
    }
}