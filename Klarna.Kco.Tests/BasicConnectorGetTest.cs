#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="BasicConnectorGetTest.cs" company="Klarna AB">
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
    using System.Net;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// The basic connector GET test.
    /// </summary>
    public class BasicConnectorGetTest : BasicConnectorTestBase
    {
        /// <summary>
        /// Tests Apply with GET method and status 200 return.
        /// </summary>
        [Test]
        public void ApplyGet200()
        {
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret);

            ResourceMock.SetupProperty(r => r.Location, Url);
            ResourceMock.SetupGet(r => r.ContentType).Returns(ContentType);

            var request = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request);
            ResponseMock.SetupGet(r => r.Data).Returns(PayLoad);
            HttpTransportMock.Setup(t => t.Send(request, PayLoad)).Returns(ResponseMock.Object);

            connector.Apply(HttpMethod.Get, ResourceMock.Object, null);

            HttpTransportMock.Verify(t => t.CreateRequest(Url), Times.Once());
            HttpTransportMock.Verify(t => t.Send(request, PayLoad), Times.Once());

            Assert.That(request.Method, Is.EqualTo(HttpMethod.Get.ToString().ToUpper()));
            Assert.That(request.UserAgent, Is.EqualTo(connector.UserAgent.ToString()));
            var authorization =
                string.Format("Klarna {0}", Digest.Create(string.Concat(PayLoad, Secret)));
            Assert.That(request.Headers["Authorization"], Is.EqualTo(authorization));
            Assert.That(request.Accept, Is.EqualTo(ContentType));
            Assert.That(request.ContentType, Is.Null);
        }

        /// <summary>
        /// Tests Apply with GET method and status 200 return.
        /// But that invalid JSON in response throws an exception.
        /// </summary>
        [Test]
        public void ApplyGet200InvalidJson()
        {
            var connector = new BasicConnector(HttpTransportMock.Object, Digest, Secret);

            ResourceMock.SetupProperty(r => r.Location, Url);
            ResourceMock.SetupGet(r => r.ContentType).Returns(ContentType);

            var request = (HttpWebRequest)WebRequest.Create(Url);
            HttpTransportMock.Setup(t => t.CreateRequest(Url)).Returns(request);
            ResponseMock.SetupGet(r => r.Data).Returns("{{{{");
            HttpTransportMock.Setup(t => t.Send(request, PayLoad)).Returns(ResponseMock.Object);

            var ex = Assert.Throws<ConnectorException>(
                () => connector.Apply(HttpMethod.Get, ResourceMock.Object, null));

            Assert.That(ex.Message, Is.EqualTo("Bad format on response content."));
        }
    }
}
