#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="BasicConnectorTest.cs" company="Klarna AB">
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
namespace Klarna.Checkout.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text.RegularExpressions;
    using Klarna.Checkout.HTTP;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// The basic connector test.
    /// </summary>
    public class BasicConnectorTest
    {
        #region Private Fields

        /// <summary>
        /// The secret.
        /// </summary>
        private const string Secret = "My Secret";

        /// <summary>
        /// The content type.
        /// </summary>
        private const string ContentType =
            "application/vnd.klarna.checkout.aggregated-order-v2+json";

        /// <summary>
        /// The HTTP error codes.
        /// </summary>
        private static readonly object[] ErrorCodes =
            {
                new object[] { HttpStatusCode.BadRequest, 400 },
                new object[] { HttpStatusCode.Unauthorized, 401 },
                new object[] { HttpStatusCode.PaymentRequired, 402 },
                new object[] { HttpStatusCode.Forbidden, 403 },
                new object[] { HttpStatusCode.NotFound, 404 },
                new object[] { HttpStatusCode.NotAcceptable, 406 },
                new object[] { HttpStatusCode.Conflict, 409 },
                new object[] { HttpStatusCode.PreconditionFailed, 412 },
                new object[] { HttpStatusCode.UnsupportedMediaType, 415 },
                // 422 (Unprocessable Entity) 
                // 428 (Precondition Required)
                // 429 (Too Many Requests)
                new object[] { HttpStatusCode.InternalServerError, 500 },
                new object[] { HttpStatusCode.BadGateway, 502 },
                new object[] { HttpStatusCode.ServiceUnavailable, 503 }
            };

        /// <summary>
        /// The url.
        /// </summary>
        private readonly Uri url = new Uri("http://klarna.com");

        /// <summary>
        /// The http transport mock.
        /// </summary>
        private Mock<IHttpTransport> httpTransportMock;

        /// <summary>
        /// The resource mock.
        /// </summary>
        private Mock<IResource> resourceMock;

        /// <summary>
        /// The digest.
        /// </summary>
        private Digest digest;

        /// <summary>
        /// The response mock.
        /// </summary>
        private Mock<IHttpResponse> responseMock;

        #endregion

        #region Set up

        /// <summary>
        /// The set up before each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            httpTransportMock = new Mock<IHttpTransport>();
            resourceMock = new Mock<IResource>();
            digest = new Digest();
            responseMock = new Mock<IHttpResponse>();
            responseMock.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.OK);
        }

        #endregion

        #region Tests

        /// <summary>
        /// Tests that the UserAgent property is correct.
        /// </summary>
        [Test]
        public void UserAgent()
        {
            var connector =
                new BasicConnector(httpTransportMock.Object, digest, "aboogie");
            Assert.That(connector.UserAgent, Is.Not.Null);

            connector.UserAgent.AddField("Module", "Magento", "5.0",
                new[] { "LanguagePack/7", "JsLib/2.0" });

            var userAgentString = connector.UserAgent.ToString();
            Assert.That(Regex.IsMatch(userAgentString, @"^.*Library\/[^\ ]+_[^\ ]+.*"), Is.True);
            Assert.That(Regex.IsMatch(userAgentString, @"^.*OS\/[^\ ]+_[^\ ]+.*"), Is.True);
            Assert.That(Regex.IsMatch(userAgentString, @"^.*Language\/[^\ ]+_[^\ ]+.*"), Is.True);
            Assert.That(Regex.IsMatch(userAgentString, @"^.*Webserver\/[^\ ]+_[^\ ]+.*"), Is.True);
            Assert.That(Regex.IsMatch(userAgentString, @"^.*Module\/Magento_5.0 \(LanguagePack\/7 ; JsLib\/2.0\).*"), Is.True);
        }

        /// <summary>
        /// Tests that Apply uses url in resource.
        /// </summary>
        [Test]
        public void ApplyUrlInResource()
        {
            var connector = new BasicConnector(httpTransportMock.Object, digest, Secret);

            resourceMock.SetupProperty(r => r.Location, url);

            var request = (HttpWebRequest)WebRequest.Create(url);
            httpTransportMock.Setup(t => t.CreateRequest(url)).Returns(request);
            var payLoad = string.Empty;
            httpTransportMock.Setup(t => t.Send(request, payLoad)).Returns(responseMock.Object);

            connector.Apply(HttpMethod.Get, resourceMock.Object, null);

            httpTransportMock.Verify(t => t.CreateRequest(url), Times.Once());
        }

        /// <summary>
        /// Tests that Apply uses url in options.
        /// </summary>
        [Test]
        public void ApplyUrlInOptions()
        {
            var connector = new BasicConnector(httpTransportMock.Object, digest, Secret);

            var request = (HttpWebRequest)WebRequest.Create(url);
            httpTransportMock.Setup(t => t.CreateRequest(url)).Returns(request);
            var payLoad = string.Empty;
            httpTransportMock.Setup(t => t.Send(request, payLoad)).Returns(responseMock.Object);

            connector.Apply(HttpMethod.Get, resourceMock.Object,
                new Dictionary<string, object> { { "url", url } });

            httpTransportMock.Verify(t => t.CreateRequest(url), Times.Once());
        }

        /// <summary>
        /// Tests Apply with GET method.
        /// </summary>
        [Test]
        public void ApplyGet()
        {
            var connector = new BasicConnector(httpTransportMock.Object, digest, Secret);

            resourceMock.SetupProperty(r => r.Location, url);
            resourceMock.SetupGet(r => r.ContentType).Returns(ContentType);

            var request = (HttpWebRequest)WebRequest.Create(url);
            httpTransportMock.Setup(t => t.CreateRequest(url)).Returns(request);
            var payLoad = string.Empty;
            httpTransportMock.Setup(t => t.Send(request, payLoad)).Returns(responseMock.Object);

            connector.Apply(HttpMethod.Get, resourceMock.Object, null);

            httpTransportMock.Verify(t => t.CreateRequest(url), Times.Once());
            httpTransportMock.Verify(t => t.Send(request, payLoad), Times.Once());

            Assert.That(request.Method, Is.EqualTo(HttpMethod.Get.ToString().ToUpper()));
            Assert.That(request.UserAgent, Is.EqualTo(connector.UserAgent.ToString()));
            var authorization =
                string.Format("Klarna {0}", digest.Create(string.Concat(payLoad, Secret)));
            Assert.That(request.Headers["Authorization"], Is.EqualTo(authorization));
            Assert.That(request.Accept, Is.EqualTo(ContentType));
            Assert.That(request.ContentType, Is.Null);
        }

        /// <summary>
        /// Tests Apply with GET method, with status code that is expected to throw an
        /// exception.
        /// </summary>
        /// <param name="statusCode">
        /// The HTTP status code.
        /// </param>
        /// <param name="expectedCode">
        /// The expected code.
        /// </param>
        [Test, TestCaseSource("ErrorCodes")]
        public void ApplyGetError(HttpStatusCode statusCode, int expectedCode)
        {
            var connector = new BasicConnector(httpTransportMock.Object, digest, Secret);

            resourceMock.SetupProperty(r => r.Location, url);
            resourceMock.SetupGet(r => r.ContentType).Returns(ContentType);

            var request = (HttpWebRequest)WebRequest.Create(url);
            httpTransportMock.Setup(t => t.CreateRequest(url)).Returns(request);
            var payLoad = string.Empty;
            responseMock.SetupGet(r => r.StatusCode).Returns(statusCode);
            httpTransportMock.Setup(t => t.Send(request, payLoad)).Returns(responseMock.Object);

            var ex = Assert.Throws<ConnectorException>(
                () => connector.Apply(HttpMethod.Get, this.resourceMock.Object, null));

            var code = (HttpStatusCode)ex.Data["HttpStatusCode"];
            Assert.That(code, Is.Not.Null);
            Assert.That((int)code, Is.EqualTo(expectedCode));
        }

        /// <summary>
        /// Tests Apply with POST method.
        /// </summary>
        [Test]
        public void ApplyPost()
        {
            var connector = new BasicConnector(httpTransportMock.Object, digest, Secret);

            resourceMock.SetupProperty(r => r.Location, url);
            resourceMock.SetupGet(r => r.ContentType).Returns(ContentType);
            var resourceData = new Dictionary<string, object>() { { "Year", 2012 } };
            resourceMock.Setup(r => r.Marshal()).Returns(resourceData);

            var request = (HttpWebRequest)WebRequest.Create(url);
            httpTransportMock.Setup(t => t.CreateRequest(url)).Returns(request);
            const string PayLoad = "{\"Year\":2012}";
            httpTransportMock.Setup(t => t.Send(request, PayLoad)).Returns(responseMock.Object);

            connector.Apply(HttpMethod.Post, resourceMock.Object, null);

            httpTransportMock.Verify(t => t.CreateRequest(url), Times.Once());
            httpTransportMock.Verify(t => t.Send(request, PayLoad), Times.Once());

            Assert.That(request.Method, Is.EqualTo(HttpMethod.Post.ToString().ToUpper()));
            Assert.That(request.UserAgent, Is.EqualTo(connector.UserAgent.ToString()));
            var authorization =
                string.Format("Klarna {0}", digest.Create(string.Concat(PayLoad, Secret)));
            Assert.That(request.Headers["Authorization"], Is.EqualTo(authorization));
            Assert.That(request.Accept, Is.EqualTo(ContentType));
            Assert.That(request.ContentType, Is.EqualTo(ContentType));
        }

        /// <summary>
        /// Tests Apply with POST method, with status code that is expected to throw an
        /// exception.
        /// </summary>
        /// <param name="statusCode">
        /// The HTTP status code.
        /// </param>
        /// <param name="expectedCode">
        /// The expected code.
        /// </param>
        [Test, TestCaseSource("ErrorCodes")]
        public void ApplyPostError(HttpStatusCode statusCode, int expectedCode)
        {
            var connector = new BasicConnector(httpTransportMock.Object, digest, Secret);

            resourceMock.SetupProperty(r => r.Location, url);
            resourceMock.SetupGet(r => r.ContentType).Returns(ContentType);
            var resourceData = new Dictionary<string, object>() { { "Year", 2012 } };
            resourceMock.Setup(r => r.Marshal()).Returns(resourceData);

            var request = (HttpWebRequest)WebRequest.Create(url);
            httpTransportMock.Setup(t => t.CreateRequest(url)).Returns(request);
            const string PayLoad = "{\"Year\":2012}";
            responseMock.SetupGet(r => r.StatusCode).Returns(statusCode);
            httpTransportMock.Setup(t => t.Send(request, PayLoad)).Returns(responseMock.Object);

            var ex = Assert.Throws<ConnectorException>(
                () => connector.Apply(HttpMethod.Post, this.resourceMock.Object, null));

            var code = (HttpStatusCode)ex.Data["HttpStatusCode"];
            Assert.That(code, Is.Not.Null);
            Assert.That((int)code, Is.EqualTo(expectedCode));
        }

        #endregion
    }
}
