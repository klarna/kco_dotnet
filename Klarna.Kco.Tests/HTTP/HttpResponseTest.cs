#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="HttpResponseTest.cs" company="Klarna AB">
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
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization.Formatters.Binary;
    using Klarna.Checkout.HTTP;
    using NUnit.Framework;

    /// <summary>
    /// Tests the HttpResponse class.
    /// </summary>
    public class HttpResponseTest
    {
        #region Tests

        /// <summary>
        /// Test that a deserialized exception still holds message and code from
        /// before being serialized.
        /// </summary>
        [Test]
        public void TestSerialization()
        {
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Content-Type", "plain/text");

            HttpResponse response = new HttpResponse(HttpStatusCode.Forbidden, headers, "payload");
            HttpResponse deserialized;

            using (Stream s = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, response);
                s.Position = 0; // Reset stream position
                deserialized = (HttpResponse)formatter.Deserialize(s);
            }

            Assert.That(deserialized.Data, Is.EqualTo(response.Data));
            Assert.That(deserialized.Header("Content-Type"), Is.EqualTo(response.Header("Content-Type")));
            Assert.That(deserialized.StatusCode, Is.EqualTo(response.StatusCode));
        }

        #endregion
    }
}
