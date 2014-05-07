#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="BasicConnectorPostTest.cs" company="Klarna AB">
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
namespace Klarna.Checkout.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using Klarna.Checkout.HTTP;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// The basic connector POST test.
    /// </summary>
    public class BasicConnectorPostTest : BasicConnectorTestBase
    {
        #region Tests

        /// <summary>
        /// Tests Apply with POST method and status 200 return.
        /// </summary>
        [Test]
        public void ApplyPost200()
        {
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret);

            ResourceMock.SetupProperty(r => r.Location, Url);
            ResourceMock.SetupGet(r => r.ContentType).Returns(ContentType);
            var resourceData = new Dictionary<string, object>() { { "Year", 2012 } };
            ResourceMock.Setup(r => r.Marshal()).Returns(resourceData);

            var request = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request);
            PayLoad = "{\"Year\":2012}";
            ResponseMock.SetupGet(r => r.Data).Returns(PayLoad);
            HttpTransportMock.Setup(t => t.Send(request, PayLoad)).Returns(ResponseMock.Object);

            connector.Apply(HttpMethod.Post, ResourceMock.Object, null);

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request, PayLoad), Times.Once());

            Assert.That(request.Method, Is.EqualTo(HttpMethod.Post.ToString().ToUpper()));
            Assert.That(request.UserAgent, Is.EqualTo(connector.UserAgent.ToString()));
            var authorization = string.Format("Klarna {0}", Digest.Create(string.Concat(PayLoad, Secret)));
            Assert.That(request.Headers["Authorization"], Is.EqualTo(authorization));
            Assert.That(request.Accept, Is.EqualTo(ContentType));
            Assert.That(request.ContentType, Is.EqualTo(ContentType));
        }

        /// <summary>
        /// Tests Apply with POST method and status 200 return.
        /// But that invalid JSON in response throws an exception.
        /// </summary>
        [Test]
        public void ApplyPost200InvalidJson()
        {
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret);

            ResourceMock.SetupProperty(r => r.Location, Url);
            ResourceMock.SetupGet(r => r.ContentType).Returns(ContentType);
            var resourceData = new Dictionary<string, object>() { { "Year", 2012 } };
            ResourceMock.Setup(r => r.Marshal()).Returns(resourceData);

            var request = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request);
            ResponseMock.SetupGet(r => r.Data).Returns("{{{{");
            PayLoad = "{\"Year\":2012}";
            HttpTransportMock.Setup(t => t.Send(request, PayLoad)).Returns(ResponseMock.Object);

            var ex = Assert.Throws<ConnectorException>(
                () => connector.Apply(HttpMethod.Post, ResourceMock.Object, null));

            Assert.That(ex.Message, Is.EqualTo("Bad format on response content."));
        }

        /// <summary>
        /// Tests Apply with POST method and status 201 return.
        /// Verifies that location is updated.
        /// </summary>
        [Test]
        public void ApplyPost201UpdatedLocation()
        {
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret);

            ResourceMock.SetupProperty(r => r.Location, Url);
            ResourceMock.SetupGet(r => r.ContentType).Returns(ContentType);
            var resourceData = new Dictionary<string, object>() { { "Year", 2012 } };
            ResourceMock.Setup(r => r.Marshal()).Returns(resourceData);

            var request = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request);

            PayLoad = "{\"Year\":2012}";
            ResponseMock.SetupGet(r => r.Data).Returns(PayLoad);
            ResponseMock.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.Created);
            const string UpdatedLocation = "http://NewLocation.com";
            ResponseMock.Setup(r => r.Header("Location")).Returns(UpdatedLocation);

            HttpTransportMock.Setup(t => t.Send(request, PayLoad)).Returns(ResponseMock.Object);

            connector.Apply(HttpMethod.Post, ResourceMock.Object, null);

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request, PayLoad), Times.Once());

            Assert.That(request.Method, Is.EqualTo(HttpMethod.Post.ToString().ToUpper()));
            Assert.That(request.UserAgent, Is.EqualTo(connector.UserAgent.ToString()));
            var authorization = string.Format("Klarna {0}", Digest.Create(string.Concat(PayLoad, Secret)));
            Assert.That(request.Headers["Authorization"], Is.EqualTo(authorization));
            Assert.That(request.Accept, Is.EqualTo(ContentType));
            Assert.That(request.ContentType, Is.EqualTo(ContentType));

            Assert.That(ResourceMock.Object.Location.OriginalString, Is.EqualTo(UpdatedLocation));
        }

        /// <summary>
        /// Tests Apply with POST method and status 301 return.
        /// Verifies that no redirect is performed and that location is updated.
        /// </summary>
        [Test]
        public void ApplyPost301EnsureNotRedirected()
        {
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret);
            var newLocation = new Uri("http://NewLocation.com");

            ResourceMock.SetupProperty(r => r.Location, Url);
            ResourceMock.SetupGet(r => r.ContentType).Returns(ContentType);
            var resourceData = new Dictionary<string, object>() { { "Year", 2012 } };
            ResourceMock.Setup(r => r.Marshal()).Returns(resourceData);

            // First request and response
            var request1 = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request1);
            var responseMock1 = new Mock<IHttpResponse>();
            responseMock1.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.MovedPermanently);
            responseMock1.Setup(r => r.Header("Location")).Returns(newLocation.OriginalString);
            PayLoad = "{\"Year\":2012}";
            responseMock1.SetupGet(r => r.Data).Returns(PayLoad);
            HttpTransportMock.Setup(t => t.Send(request1, PayLoad)).Returns(responseMock1.Object);

            // Second request and response
            var request2 = (HttpWebRequest)WebRequest.Create(newLocation);
            HttpTransportMock.Setup(t => t.CreateRequest(newLocation)).Returns(request2);
            var responseMock2 = new Mock<IHttpResponse>();
            responseMock2.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.ServiceUnavailable);
            responseMock2.SetupGet(r => r.Data).Returns(PayLoad);
            HttpTransportMock.Setup(t => t.Send(request2, PayLoad)).Returns(responseMock2.Object);

            connector.Apply(HttpMethod.Post, ResourceMock.Object, null);

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request1, PayLoad), Times.Once());
            HttpTransportMock.Verify(t => t.CreateRequest(newLocation), Times.Never());
            HttpTransportMock.Verify(t => t.Send(request2, PayLoad), Times.Never());

            Assert.That(request1.Method, Is.EqualTo(HttpMethod.Post.ToString().ToUpper()));
            Assert.That(request1.UserAgent, Is.EqualTo(connector.UserAgent.ToString()));
            var authorization = string.Format("Klarna {0}", Digest.Create(string.Concat(PayLoad, Secret)));
            Assert.That(request1.Headers["Authorization"], Is.EqualTo(authorization));
            Assert.That(request1.Accept, Is.EqualTo(ContentType));
            Assert.That(request1.ContentType, Is.EqualTo(ContentType));

            Assert.That(ResourceMock.Object.Location.OriginalString, Is.EqualTo(newLocation.OriginalString));
        }

        /// <summary>
        /// Tests Apply with POST method and status 302 return.
        /// Verifies that no redirect is performed and that location NOT is updated.
        /// </summary>
        [Test]
        public void ApplyPost302EnsureNotRedirected()
        {
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret);
            var newLocation = new Uri("http://NewLocation.com");

            ResourceMock.SetupProperty(r => r.Location, Url);
            ResourceMock.SetupGet(r => r.ContentType).Returns(ContentType);
            var resourceData = new Dictionary<string, object>() { { "Year", 2012 } };
            ResourceMock.Setup(r => r.Marshal()).Returns(resourceData);

            // First request and response
            var request1 = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request1);
            var responseMock1 = new Mock<IHttpResponse>();
            responseMock1.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.Found);
            responseMock1.Setup(r => r.Header("Location")).Returns(newLocation.OriginalString);
            PayLoad = "{\"Year\":2012}";
            responseMock1.SetupGet(r => r.Data).Returns(PayLoad);
            HttpTransportMock.Setup(t => t.Send(request1, PayLoad)).Returns(responseMock1.Object);

            // Second request and response
            var request2 = (HttpWebRequest)WebRequest.Create(newLocation);
            HttpTransportMock.Setup(t => t.CreateRequest(newLocation)).Returns(request2);
            var responseMock2 = new Mock<IHttpResponse>();
            responseMock2.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.ServiceUnavailable);
            responseMock2.SetupGet(r => r.Data).Returns(PayLoad);
            HttpTransportMock.Setup(t => t.Send(request2, PayLoad)).Returns(responseMock2.Object);

            connector.Apply(HttpMethod.Post, ResourceMock.Object, null);

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request1, PayLoad), Times.Once());
            HttpTransportMock.Verify(t => t.CreateRequest(newLocation), Times.Never());
            HttpTransportMock.Verify(t => t.Send(request2, PayLoad), Times.Never());

            Assert.That(request1.Method, Is.EqualTo(HttpMethod.Post.ToString().ToUpper()));
            Assert.That(request1.UserAgent, Is.EqualTo(connector.UserAgent.ToString()));
            var authorization = string.Format("Klarna {0}", Digest.Create(string.Concat(PayLoad, Secret)));
            Assert.That(request1.Headers["Authorization"], Is.EqualTo(authorization));
            Assert.That(request1.Accept, Is.EqualTo(ContentType));
            Assert.That(request1.ContentType, Is.EqualTo(ContentType));

            Assert.That(ResourceMock.Object.Location.OriginalString, Is.EqualTo(Url.OriginalString));
        }

        /// <summary>
        /// Tests Apply with POST method and status 302 return and redirect to status 200.
        /// Verifies redirect, that redirect is with GET and that location NOT is updated.
        /// </summary>
        [Test]
        public void ApplyPost303And200()
        {
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret);
            var newLocation = new Uri("http://NewLocation.com");

            ResourceMock.SetupProperty(r => r.Location, Url);
            ResourceMock.SetupGet(r => r.ContentType).Returns(ContentType);
            var resourceData = new Dictionary<string, object>() { { "Year", 2012 } };
            ResourceMock.Setup(r => r.Marshal()).Returns(resourceData);

            // First request and response
            var request1 = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request1);
            var responseMock1 = new Mock<IHttpResponse>();
            responseMock1.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.SeeOther);
            responseMock1.Setup(r => r.Header("Location")).Returns(newLocation.OriginalString);
            PayLoad = "{\"Year\":2012}";
            responseMock1.SetupGet(r => r.Data).Returns(PayLoad);
            HttpTransportMock.Setup(t => t.Send(request1, PayLoad)).Returns(responseMock1.Object);

            // Second request and response
            var request2 = (HttpWebRequest)WebRequest.Create(newLocation);
            request2.Method = "GET";
            HttpTransportMock.Setup(t => t.CreateRequest(newLocation)).Returns(request2);
            var responseMock2 = new Mock<IHttpResponse>();
            responseMock2.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.OK);
            responseMock2.Setup(r => r.Header("Location")).Returns(Url.OriginalString);
            responseMock2.SetupGet(r => r.Data).Returns(PayLoad);
            HttpTransportMock.Setup(t => t.Send(request2, string.Empty)).Returns(responseMock2.Object);

            connector.Apply(HttpMethod.Post, ResourceMock.Object, null);

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request1, PayLoad), Times.Once());
            HttpTransportMock.Verify(t => t.CreateRequest(newLocation), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request2, string.Empty), Times.Once());

            Assert.That(request1.Method, Is.EqualTo(HttpMethod.Post.ToString().ToUpper()));
            Assert.That(request1.UserAgent, Is.EqualTo(connector.UserAgent.ToString()));
            var authorization = string.Format("Klarna {0}", Digest.Create(string.Concat(PayLoad, Secret)));
            Assert.That(request1.Headers["Authorization"], Is.EqualTo(authorization));
            Assert.That(request1.Accept, Is.EqualTo(ContentType));
            Assert.That(request1.ContentType, Is.EqualTo(ContentType));

            Assert.That(request2.Method, Is.EqualTo(HttpMethod.Get.ToString().ToUpper()));
            Assert.That(request2.UserAgent, Is.EqualTo(connector.UserAgent.ToString()));
            var authorization2 = string.Format("Klarna {0}", Digest.Create(string.Concat(string.Empty, Secret)));
            Assert.That(request2.Headers["Authorization"], Is.EqualTo(authorization2));
            Assert.That(request2.Accept, Is.EqualTo(ContentType));
            Assert.That(request2.ContentType, Is.Null);

            Assert.That(ResourceMock.Object.Location.OriginalString, Is.EqualTo(Url.OriginalString));
        }

        /// <summary>
        /// Tests Apply with POST method and status 303 return and redirect to status 503.
        /// Verifies redirect, that redirect is with GET, that location NOT is updated
        /// and that exception is thrown.
        /// </summary>
        [Test]
        public void ApplyPost303And503()
        {
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret);
            var newLocation = new Uri("http://NewLocation.com");

            ResourceMock.SetupProperty(r => r.Location, Url);
            ResourceMock.SetupGet(r => r.ContentType).Returns(ContentType);
            var resourceData = new Dictionary<string, object>() { { "Year", 2012 } };
            ResourceMock.Setup(r => r.Marshal()).Returns(resourceData);

            // First request and response
            var request1 = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request1);
            var responseMock1 = new Mock<IHttpResponse>();
            responseMock1.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.SeeOther);
            responseMock1.Setup(r => r.Header("Location")).Returns(newLocation.OriginalString);
            PayLoad = "{\"Year\":2012}";
            responseMock1.SetupGet(r => r.Data).Returns(PayLoad);
            HttpTransportMock.Setup(t => t.Send(request1, PayLoad)).Returns(responseMock1.Object);

            // Second request and response
            var request2 = (HttpWebRequest)WebRequest.Create(newLocation);
            request2.Method = "GET";
            HttpTransportMock.Setup(t => t.CreateRequest(newLocation)).Returns(request2);
            var responseMock2 = new Mock<IHttpResponse>();
            responseMock2.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.ServiceUnavailable);
            responseMock2.Setup(r => r.Header("Location")).Returns(Url.OriginalString);
            responseMock2.SetupGet(r => r.Data).Returns(PayLoad);
            HttpTransportMock.Setup(t => t.Send(request2, string.Empty)).Returns(responseMock2.Object);

            var ex = Assert.Throws<ConnectorException>(
                () => connector.Apply(HttpMethod.Post, ResourceMock.Object, null));

            var code = (HttpStatusCode)ex.Data["HttpStatusCode"];
            Assert.That(code, Is.Not.Null);
            Assert.That((int)code, Is.EqualTo(503));
            Assert.That(ex.Message, Is.EqualTo("ServiceUnavailable"));

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request1, PayLoad), Times.Once());
            HttpTransportMock.Verify(t => t.CreateRequest(newLocation), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request2, string.Empty), Times.Once());

            Assert.That(request1.Method, Is.EqualTo(HttpMethod.Post.ToString().ToUpper()));
            Assert.That(request1.UserAgent, Is.EqualTo(connector.UserAgent.ToString()));
            var authorization = string.Format("Klarna {0}", Digest.Create(string.Concat(PayLoad, Secret)));
            Assert.That(request1.Headers["Authorization"], Is.EqualTo(authorization));
            Assert.That(request1.Accept, Is.EqualTo(ContentType));
            Assert.That(request1.ContentType, Is.EqualTo(ContentType));

            Assert.That(request2.Method, Is.EqualTo(HttpMethod.Get.ToString().ToUpper()));
            Assert.That(request2.UserAgent, Is.EqualTo(connector.UserAgent.ToString()));
            var authorization2 = string.Format("Klarna {0}", Digest.Create(string.Concat(string.Empty, Secret)));
            Assert.That(request2.Headers["Authorization"], Is.EqualTo(authorization2));
            Assert.That(request2.Accept, Is.EqualTo(ContentType));
            Assert.That(request2.ContentType, Is.Null);

            Assert.That(ResourceMock.Object.Location.OriginalString, Is.EqualTo(Url.OriginalString));
        }

        #endregion
    }
}
