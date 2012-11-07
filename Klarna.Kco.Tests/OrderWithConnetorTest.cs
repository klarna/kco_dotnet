#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="OrderWithConnetorTest.cs" company="Klarna AB">
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
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Tests the Order class with Connector.
    /// </summary>
    [TestFixture]
    public class OrderWithConnetorTest
    {
        #region Private Fields

        /// <summary>
        /// The order.
        /// </summary>
        private Order order;

        /// <summary>
        /// The mocked connector.
        /// </summary>
        private Mock<IConnector> mockConnector;

        #endregion

        #region Set up

        /// <summary>
        /// The set up before each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            order = new Order();
            mockConnector = new Mock<IConnector>();
        }

        #endregion

        #region Tests

        /// <summary>
        /// Tests that Create works correctly.
        /// </summary>
        [Test]
        public void Create()
        {
            const string Key = "foo";
            const string Value = "boo";
            var data = new Dictionary<string, object> { { Key, Value } };
            order = new Order(data);

            var options = new Dictionary<string, object> { { "url", order.BaseUri } };
            mockConnector.Setup(c => c.Apply(HttpMethod.Post, order, options)).Verifiable();
            order.Create(mockConnector.Object);

            mockConnector.Verify();

            Assert.That(order.GetValue(Key), Is.EqualTo(Value));
        }

        /// <summary>
        /// Tests the Create with alternative entry point works correctly.
        /// </summary>
        [Test]
        public void CreateAlternativeEntryPoint()
        {
            const string Key = "foo";
            const string Value = "boo";
            var data = new Dictionary<string, object> { { Key, Value } };
            order = new Order(data) { BaseUri = new Uri("https://checkout.klarna.com/beta/checkout/orders") };

            var options = new Dictionary<string, object> { { "url", order.BaseUri } };
            mockConnector.Setup(c => c.Apply(HttpMethod.Post, order, options)).Verifiable();
            order.Create(mockConnector.Object);

            mockConnector.Verify();

            Assert.That(order.GetValue(Key), Is.EqualTo(Value));
        }

        /// <summary>
        /// Tests that Fetch works correctly.
        /// </summary>
        [Test]
        public void Fetch()
        {
            order.Location = new Uri("http://klarna.com/foo/bar/15");

            var options = new Dictionary<string, object> { { "url", order.Location } };
            mockConnector.Setup(c => c.Apply(HttpMethod.Get, order, options)).Verifiable();
            order.Fetch(mockConnector.Object);

            mockConnector.Verify();
        }

        /// <summary>
        /// Tests that Fetch with uri parameter works correctly.
        /// </summary>
        [Test]
        public void FetchSetLocation()
        {
            var uri = new Uri("http://klarna.com/foo/bar/16");

            var options = new Dictionary<string, object> { { "url", uri } };
            mockConnector.Setup(c => c.Apply(HttpMethod.Get, order, options)).Verifiable();
            order.Fetch(mockConnector.Object, uri);

            mockConnector.Verify();

            Assert.That(order.Location, Is.EqualTo(uri));
        }

        /// <summary>
        /// Tests that Update works correctly.
        /// </summary>
        [Test]
        public void Update()
        {
            order.Location = new Uri("http://klarna.com/foo/bar/15");

            var options = new Dictionary<string, object> { { "url", order.Location } };
            mockConnector.Setup(c => c.Apply(HttpMethod.Post, order, options)).Verifiable();
            order.Update(mockConnector.Object);

            mockConnector.Verify();
        }

        /// <summary>
        /// Tests that Update with uri parameter works correctly.
        /// </summary>
        [Test]
        public void UpdateSetLocation()
        {
            var uri = new Uri("http://klarna.com/foo/bar/16");

            var options = new Dictionary<string, object> { { "url", uri } };
            mockConnector.Setup(c => c.Apply(HttpMethod.Post, order, options)).Verifiable();
            order.Update(mockConnector.Object, uri);

            mockConnector.Verify();

            Assert.That(order.Location, Is.EqualTo(uri));

        }

        #endregion
    }
}
