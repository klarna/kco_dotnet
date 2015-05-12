#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="BasicConnectorTest.cs" company="Klarna AB">
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
    using System.Collections.Generic;
    using System.Net;
    using System.Text.RegularExpressions;
    using Klarna.Checkout.HTTP;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;

    /// <summary>
    /// The basic connector test.
    /// </summary>
    public class BasicConnectorTest : BasicConnectorTestBase
    {
        #region Private Fields

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
                new object[] { 422, 422 },
                new object[] { 428, 428 },
                new object[] { 429, 429 },
                new object[] { HttpStatusCode.InternalServerError, 500 },
                new object[] { HttpStatusCode.BadGateway, 502 },
                new object[] { HttpStatusCode.ServiceUnavailable, 503 }
            };

        #endregion

        #region Tests

        /// <summary>
        /// Tests that the UserAgent property is correct.
        /// </summary>
        [Test]
        public void UserAgent()
        {
            var connector =
                new BasicConnector(HttpTransportMock.Object, Digest, "aboogie", "http://test.com");
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
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret, "http://test.com");

            ResourceMock.SetupProperty(r => r.Location, Url);

            var request = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request);
            ResponseMock.SetupGet(r => r.Data).Returns(PayLoad);
            HttpTransportMock.Setup(t => t.Send(request, PayLoad)).Returns(ResponseMock.Object);

            connector.Apply(HttpMethod.Get, ResourceMock.Object, null);

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
        }

        /// <summary>
        /// Tests that Apply uses url in options.
        /// </summary>
        [Test]
        public void ApplyUrlInOptions()
        {
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret, "http://test.com");

            var request = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request);
            ResponseMock.SetupGet(r => r.Data).Returns(PayLoad);
            HttpTransportMock.Setup(t => t.Send(request, PayLoad)).Returns(ResponseMock.Object);

            connector.Apply(HttpMethod.Get, ResourceMock.Object,
                new Dictionary<string, object> { { "url", Url } });

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
        }

        /// <summary>
        /// Tests that Apply uses data in resource.
        /// </summary>
        [Test]
        public void ApplyDataInResource()
        {
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret, "http://test.com");

            ResourceMock.SetupProperty(r => r.Location, Url);

            var request = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request);
            PayLoad = "{\"Year\":2012}";
            ResponseMock.SetupGet(r => r.Data).Returns(PayLoad);
            HttpTransportMock.Setup(t => t.Send(request, PayLoad)).Returns(ResponseMock.Object);

            var resourceData = new Dictionary<string, object>() { { "Year", 2012 } };
            ResourceMock.Setup(r => r.Marshal()).Returns(resourceData);

            var options = new Dictionary<string, object>();
            connector.Apply(HttpMethod.Post, ResourceMock.Object, options);

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request, PayLoad), Times.Once());
        }

        /// <summary>
        /// Tests that Apply uses data in options.
        /// </summary>
        [Test]
        public void ApplyDataInOptions()
        {
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret, "http://test.com");

            ResourceMock.SetupProperty(r => r.Location, Url);

            var request = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request);
            PayLoad = "{\"Year\":2012}";
            ResponseMock.SetupGet(r => r.Data).Returns(PayLoad);
            HttpTransportMock.Setup(t => t.Send(request, PayLoad)).Returns(ResponseMock.Object);

            ResourceMock.Setup(r => r.Marshal()).Returns(new Dictionary<string, object>());

            var resourceData = new Dictionary<string, object>() { { "Year", 2012 } };
            var options = new Dictionary<string, object>() { { "data", resourceData } };
            connector.Apply(HttpMethod.Post, ResourceMock.Object, options);

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request, PayLoad), Times.Once());
        }

        /// <summary>
        /// Tests that Apply uses accept in resource
        /// </summary>
        [Test]
        public void ApplyAcceptInResource()
        {
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret, "http://test.com");

            ResourceMock.SetupProperty(r => r.Location, Url);
            ResourceMock.SetupGet(r => r.ContentType).Returns(ContentType);
            ResourceMock.SetupGet(r => r.Accept).Returns(Accept);

            var request = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request);
            ResponseMock.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.OK);
            ResponseMock.SetupGet(r => r.Data).Returns(PayLoad);
            HttpTransportMock.Setup(t => t.Send(request, PayLoad)).Returns(ResponseMock.Object);

            var options = new Dictionary<string, object>();
            connector.Apply(HttpMethod.Get, ResourceMock.Object, options);

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request, PayLoad), Times.Once());

            Assert.That(request.Accept, Is.EqualTo(Accept));
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
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret, "http://test.com");

            ResourceMock.SetupProperty(r => r.Location, Url);
            ResourceMock.SetupGet(r => r.ContentType).Returns(ContentType);

            var request = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request);
            ResponseMock.SetupGet(r => r.StatusCode).Returns(statusCode);
            HttpTransportMock.Setup(t => t.Send(request, PayLoad)).Returns(ResponseMock.Object);

            var ex = Assert.Throws<ConnectorException>(
                () => connector.Apply(HttpMethod.Get, ResourceMock.Object, null));

            var code = (HttpStatusCode)ex.Data["HttpStatusCode"];
            Assert.That(code, Is.Not.Null);
            Assert.That((int)code, Is.EqualTo(expectedCode));
            Assert.That(ex.Message, Is.EqualTo(code.ToString()));

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request, PayLoad), Times.Once());
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
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret, "http://test.com");

            ResourceMock.SetupProperty(r => r.Location, Url);
            ResourceMock.SetupGet(r => r.ContentType).Returns(ContentType);
            var resourceData = new Dictionary<string, object>() { { "Year", 2012 } };
            ResourceMock.Setup(r => r.Marshal()).Returns(resourceData);

            var request = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request);
            ResponseMock.SetupGet(r => r.StatusCode).Returns(statusCode);
            PayLoad = "{\"Year\":2012}";
            HttpTransportMock.Setup(t => t.Send(request, PayLoad)).Returns(ResponseMock.Object);

            var ex = Assert.Throws<ConnectorException>(
                () => connector.Apply(HttpMethod.Post, ResourceMock.Object, null));

            var code = (HttpStatusCode)ex.Data["HttpStatusCode"];
            Assert.That(code, Is.Not.Null);
            Assert.That((int)code, Is.EqualTo(expectedCode));
            Assert.That(ex.Message, Is.EqualTo(code.ToString()));

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request, PayLoad), Times.Once());
        }

        /// <summary>
        /// Tests Apply with a response error type.
        /// </summary>
        [Test]
        public void ApplyErrorType()
        {
            const HttpStatusCode StatusCode = HttpStatusCode.Unauthorized;
            const string ErrorType = "application/vnd.klarna.error-v1+json";
            const string InternalMessage = "Bad digest";

            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret, "http://test.com");

            ResourceMock.SetupProperty(r => r.Location, Url);
            ResourceMock.SetupGet(r => r.ContentType).Returns(ContentType);
            var resourceData = new Dictionary<string, object>() { { "Year", 2012 } };
            ResourceMock.Setup(r => r.Marshal()).Returns(resourceData);

            var request = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request);

            var errorObject = new Dictionary<string, object>()
            {
                { "http_status_code", (int)StatusCode },
                { "http_status_message", StatusCode.ToString() },
                { "internal_message", InternalMessage }
            };

            var error = JsonConvert.SerializeObject(errorObject);
            ResponseMock.SetupGet(r => r.Data).Returns(error);
            ResponseMock.Setup(r => r.Header("Content-Type")).Returns(ErrorType);
            ResponseMock.SetupGet(r => r.StatusCode).Returns(StatusCode);

            PayLoad = JsonConvert.SerializeObject(resourceData);
            HttpTransportMock.Setup(t => t.Send(request, PayLoad)).Returns(ResponseMock.Object);

            var ex = Assert.Throws<ConnectorException>(
                () => connector.Apply(HttpMethod.Post, ResourceMock.Object, null));

            var code = (HttpStatusCode)ex.Data["HttpStatusCode"];
            Assert.That(code, Is.Not.Null);
            Assert.That((int)code, Is.EqualTo((int)StatusCode));
            Assert.That(ex.Message, Is.EqualTo(code.ToString()));
            Assert.That(ex.Data["internal_message"], Is.EqualTo(InternalMessage));

            var response = (Klarna.Checkout.HTTP.IHttpResponse)ex.Data["Response"];
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.InstanceOf<Klarna.Checkout.HTTP.IHttpResponse>());
            Assert.That(response.StatusCode, Is.EqualTo(StatusCode));
            Assert.That(response.Data, Is.EqualTo(error));
            Assert.That(response.Header("Content-Type"), Is.EqualTo(ErrorType));

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request, PayLoad), Times.Once());
        }

        /// <summary>
        /// Tests Apply that an exception wraps WebException.
        /// </summary>
        [Test]
        public void ApplyWrappedWebException()
        {
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret, "http://test.com");

            ResourceMock.SetupProperty(r => r.Location, Url);
            ResourceMock.SetupGet(r => r.ContentType).Returns(ContentType);
            var resourceData = new Dictionary<string, object>() { { "Year", 2012 } };
            ResourceMock.Setup(r => r.Marshal()).Returns(resourceData);

            var request = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request);

            var webEx = new WebException("Error", null, WebExceptionStatus.Timeout, null);

            PayLoad = JsonConvert.SerializeObject(resourceData);
            HttpTransportMock.Setup(t => t.Send(request, PayLoad)).Throws(webEx);

            var ex = Assert.Throws<ConnectorException>(
                () => connector.Apply(HttpMethod.Post, ResourceMock.Object, null));

            Assert.That(ex.InnerException, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<WebException>());
            Assert.That(ex.InnerException, Is.SameAs(webEx));

            Assert.That(ex.Data["HttpStatusCode"], Is.Null);
            Assert.That(ex.Data["Response"], Is.Null);

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request, PayLoad), Times.Once());
        }

        /// <summary>
        /// Tests that a timeout can be set via the connector.
        /// </summary>
        [Test]
        public void SetTimeoutOnConnector()
        {
            IHttpTransport transport = new BasicHttpTransport();
            IConnector conn = new BasicConnector(transport, Digest, Secret, "http://test.com");
            conn.Transport.Timeout = 120;

            Assert.That(transport.Timeout, Is.EqualTo(120));
        }

        #endregion
    }
}
