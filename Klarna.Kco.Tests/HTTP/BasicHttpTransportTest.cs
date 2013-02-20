#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="BasicHttpTransportTest.cs" company="Klarna AB">
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
namespace Klarna.Checkout.Tests.HTTP
{
    using System;
    using System.Net;

    using Klarna.Checkout.HTTP;

    using NUnit.Framework;

    /// <summary>
    /// Tests the BasicHttpTransport class.
    /// </summary>
    public class BasicHttpTransportTest
    {
        #region Private Fields

        /// <summary>
        /// The HTTP error codes.
        /// </summary>
        private static readonly object[] ErrorCodes =
            {
                400, 401, 402, 403, 404, 406, 409, 412, 415, 422, 428, 429, 500, 
                502, 503
            };

        #endregion

        #region Tests

        /// <summary>
        /// Test Timeout.
        /// </summary>
        [Test]
        public void Timeout()
        {
            var transport = new BasicHttpTransport();
            Assert.That(transport.Timeout, Is.EqualTo(10000));

            transport.Timeout = 50000;
            Assert.That(transport.Timeout, Is.EqualTo(50000));
        }

        /// <summary>
        /// Tests CreateRequest.
        /// </summary>
        [Test]
        public void CreateRequest()
        {
            var transport = new BasicHttpTransport();
            var request = transport.CreateRequest(new Uri("http://klarna.com"));
            Assert.That(request, Is.TypeOf<HttpWebRequest>());
            Assert.That(request.AllowAutoRedirect, Is.False);
            Assert.That(request.Timeout, Is.EqualTo(10000));
        }

        /// <summary>
        /// Test Send returning error code.
        /// </summary>
        /// <remarks>
        /// Performed towards real server.
        /// </remarks>
        /// <param name="code">
        /// The error code.
        /// </param>
        [Test, TestCaseSource("ErrorCodes")]
        [Ignore("Don't hit the network")]
        public void SendReturningErrorCode(int code)
        {
            var transport = new BasicHttpTransport();
            var uri = string.Format("http://httpbin.org/status/{0}", code);
            var request = transport.CreateRequest(new Uri(uri));
            request.Method = "GET";
            var response = transport.Send(request, string.Empty);

            Assert.That((int)response.StatusCode, Is.EqualTo(code));
        }

        /// <summary>
        /// Test that header name is case insensitive when accessing a header.
        /// </summary>
        [Test]
        [Ignore("Don't hit the network")]
        public void HeaderNameCaseInsensitive()
        {
            var transport = new BasicHttpTransport();
            var uri = new Uri("http://httpbin.org/status/200");
            var request = transport.CreateRequest(uri);
            request.Method = "GET";
            var response = transport.Send(request, string.Empty);

            var expected = response.Header("Content-Type");
            var lowerCase = response.Header("content-type");
            Assert.That(lowerCase, Is.EqualTo(expected));

            var mixedCase = response.Header("cOnTeNt-TyPe");
            Assert.That(mixedCase, Is.EqualTo(expected));
        }

        #endregion
    }
}
