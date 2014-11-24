#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="OrderTest.cs" company="Klarna AB">
//     Copyright 2012 Klarna AB
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
    using System.Collections.Generic;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Tests the Order class.
    /// </summary>
    [TestFixture]
    public class OrderTest
    {
        #region Private Fields

        /// <summary>
        /// Data used in tests.
        /// </summary>
        private const string Url = "http://klarna.com";

        /// <summary>
        /// Data used in tests.
        /// </summary>
        private const int TheInt = 89;

        /// <summary>
        /// Data used in tests.
        /// </summary>
        private const string TheString = "A string";

        /// <summary>
        /// Data used in tests.
        /// </summary>
        private readonly DateTime theDateTime = new DateTime(2012, 10, 14, 22, 53, 12);

        /// <summary>
        /// The order.
        /// </summary>
        private Order order;

        /// <summary>
        /// The mocked connector.
        /// </summary>
        private Mock<IConnector> mockConnector;

        #endregion

        #region Set Up

        /// <summary>
        /// The set up before each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            mockConnector = new Mock<IConnector>();
            order = new Order(mockConnector.Object);
        }

        #endregion

        #region Tests

        /// <summary>
        /// The construction with connector.
        /// </summary>
        [Test]
        public void ConstructionWithConnector()
        {
            Assert.That(order.BaseUri, Is.Null);
            Assert.That(order.Location, Is.Null);
            Assert.That(order.ContentType, Is.Null);
            Assert.That(order.Accept, Is.Null);
            var data = order.Marshal();
            Assert.That(data, Is.Empty);
        }

        /// <summary>
        /// The construction with resource uri.
        /// </summary>
        [Test]
        public void ConstructionWithResourceUri()
        {
            var uri = new Uri(Url);
            order = new Order(mockConnector.Object, uri);

            Assert.That(order.BaseUri, Is.Null);
            Assert.That(order.Location, Is.EqualTo(uri));
            Assert.That(order.ContentType, Is.Null);
            Assert.That(order.Accept, Is.Null);
            var data = order.Marshal();
            Assert.That(data, Is.Empty);
        }

        /// <summary>
        /// Tests that the content type is correct.
        /// </summary>
        [Test]
        public void ContentType()
        {
            const string ContentType = "application/vnd.klarna.checkout.aggregated-order-v2+json";

            Assert.That(order.ContentType, Is.Null);
            order.ContentType = ContentType;
            Assert.That(order.ContentType, Is.EqualTo(ContentType));
        }

        /// <summary>
        /// Tests that accept is correct.
        /// </summary>
        [Test]
        public void Accept()
        {
            const string Accept = "application/something-else";

            Assert.That(order.Accept, Is.Null);
            order.Accept = Accept;
            Assert.That(order.Accept, Is.EqualTo(Accept));
        }

        /// <summary>
        /// Tests that the location is not initialized.
        /// </summary>
        [Test]
        public void LocationNull()
        {
            Assert.That(order.Location, Is.Null);
        }

        /// <summary>
        /// Tests set/get location.
        /// </summary>
        [Test]
        public void LocationSetGet()
        {
            Assert.That(order.Location, Is.Null);
            order.Location = new Uri(Url);
            Assert.That(order.Location, Is.EqualTo(new Uri(Url)));
        }

        /// <summary>
        /// Tests that parse works correctly.
        /// </summary>
        public void Parse()
        {
            var newData = TestData();
            order.Parse(newData);

            var data = order.Marshal();

            Assert.That(data, Is.EqualTo(newData));
        }

        /// <summary>
        /// Tests that marshal works correctly.
        /// </summary>
        [Test]
        public void Marshal()
        {
            var newData = TestData();
            order.Parse(newData);

            var data = order.Marshal();
            Assert.That(data, Is.TypeOf<Dictionary<string, object>>());
            Assert.That(data["Int"], Is.TypeOf<int>());
            Assert.That((int)data["Int"], Is.EqualTo(TheInt));
            Assert.That(data["String"], Is.TypeOf<string>());
            Assert.That(data["String"], Is.EqualTo(TheString));
            Assert.That(data["DateTime"], Is.TypeOf<DateTime>());
            Assert.That((DateTime)data["DateTime"], Is.EqualTo(theDateTime));
        }

        /// <summary>
        /// Tests set/get values.
        /// </summary>
        [Test]
        public void ValuesGet()
        {
            var data = TestData();
            order.Parse(data);

            var intData = order.GetValue("Int");
            Assert.That(intData, Is.TypeOf<int>());
            Assert.That((int)intData, Is.EqualTo(TheInt));

            var stringData = order.GetValue("String");
            Assert.That(stringData, Is.TypeOf<string>());
            Assert.That(stringData, Is.EqualTo(TheString));

            var dateTimeData = order.GetValue("DateTime");
            Assert.That(dateTimeData, Is.TypeOf<DateTime>());
            Assert.That((DateTime)dateTimeData, Is.EqualTo(theDateTime));
        }

        /// <summary>
        /// Tests that get value with null key or a non-existing key throws an exception.
        /// </summary>
        [Test]
        public void ValuesGetExeption()
        {
            Assert.Throws<ArgumentNullException>(() => order.GetValue(null));
            Assert.Throws<KeyNotFoundException>(() => order.GetValue("NonExistingKey"));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates test data.
        /// </summary>
        /// <returns>
        /// The new test data.
        /// </returns>
        private Dictionary<string, object> TestData()
        {
            return new Dictionary<string, object>
                {
                    { "Int", TheInt },
                    { "String", TheString },
                    { "DateTime", this.theDateTime }
                };
        }

        #endregion
    }
}