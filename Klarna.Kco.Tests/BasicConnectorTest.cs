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

        #endregion
    }
}
