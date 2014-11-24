#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="RecurringOrderTest.cs" company="Klarna AB">
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
    /// Tests the RecurringOrder class.
    /// </summary>
    [TestFixture]
    public class RecurringOrderTest
    {
        #region Private Properties

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
        private RecurringOrder recurringOrder;

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
            recurringOrder = new RecurringOrder(mockConnector.Object);
        }

        #endregion

        #region Tests

        /// <summary>
        /// The construction with connector.
        /// </summary>
        [Test]
        public void ConstructionWithConnector()
        {
            Assert.That(recurringOrder.BaseUri, Is.Null);
            Assert.That(recurringOrder.Location, Is.Null);
            Assert.That(recurringOrder.ContentType, Is.Null);
            Assert.That(recurringOrder.Accept, Is.Null);
            var data = recurringOrder.Marshal();
            Assert.That(data, Is.Empty);
        }

        /// <summary>
        /// Tests that the content type is correct.
        /// </summary>
        [Test]
        public void ContentType()
        {
            const string ContentType = "application/vnd.klarna.checkout.recurring-order-v1+json";

            Assert.That(recurringOrder.ContentType, Is.Null);
            recurringOrder.ContentType = ContentType;
            Assert.That(recurringOrder.ContentType, Is.EqualTo(ContentType));
        }

        /// <summary>
        /// Tests that accept is correct.
        /// </summary>
        [Test]
        public void Accept()
        {
            const string Accept = "application/vnd.klarna.checkout.recurring-order-accepted-v1+json";

            Assert.That(recurringOrder.Accept, Is.Null);
            recurringOrder.Accept = Accept;
            Assert.That(recurringOrder.Accept, Is.EqualTo(Accept));
        }

        /// <summary>
        /// Tests that the location is not initialized.
        /// </summary>
        [Test]
        public void LocationNull()
        {
            Assert.That(recurringOrder.Location, Is.Null);
        }

        /// <summary>
        /// Tests set/get location.
        /// </summary>
        [Test]
        public void LocationSetGet()
        {
            Assert.That(recurringOrder.Location, Is.Null);
            recurringOrder.Location = new Uri(Url);
            Assert.That(recurringOrder.Location, Is.EqualTo(new Uri(Url)));
        }

        /// <summary>
        /// Tests that parse works correctly.
        /// </summary>
        public void Parse()
        {
            var newData = TestData();
            recurringOrder.Parse(newData);

            var data = recurringOrder.Marshal();

            Assert.That(data, Is.EqualTo(newData));
        }

        /// <summary>
        /// Tests that marshal works correctly.
        /// </summary>
        [Test]
        public void Marshal()
        {
            var newData = TestData();
            recurringOrder.Parse(newData);

            var data = recurringOrder.Marshal();
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
            recurringOrder.Parse(data);

            var intData = recurringOrder.GetValue("Int");
            Assert.That(intData, Is.TypeOf<int>());
            Assert.That((int)intData, Is.EqualTo(TheInt));

            var stringData = recurringOrder.GetValue("String");
            Assert.That(stringData, Is.TypeOf<string>());
            Assert.That(stringData, Is.EqualTo(TheString));

            var dateTimeData = recurringOrder.GetValue("DateTime");
            Assert.That(dateTimeData, Is.TypeOf<DateTime>());
            Assert.That((DateTime)dateTimeData, Is.EqualTo(theDateTime));
        }

        /// <summary>
        /// Tests that get value with null key or a non-existing key throws an exception.
        /// </summary>
        [Test]
        public void ValuesGetExeption()
        {
            Assert.Throws<ArgumentNullException>(() => recurringOrder.GetValue(null));
            Assert.Throws<KeyNotFoundException>(() => recurringOrder.GetValue("NonExistingKey"));
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
