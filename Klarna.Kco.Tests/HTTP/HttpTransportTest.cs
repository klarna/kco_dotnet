#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="HttpTransportTest.cs" company="Klarna AB">
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
    using Klarna.Checkout.HTTP;
    using NUnit.Framework;

    /// <summary>
    /// Tests the HttpTransport factory class.
    /// </summary>
    public class HttpTransportTest
    {
        /// <summary>
        /// Tests the factory method.
        /// </summary>
        [Test]
        public void Factory()
        {
            var httptransport = HttpTransport.Create();
            Assert.That(httptransport, Is.InstanceOf<IHttpTransport>());
        }
    }
}
