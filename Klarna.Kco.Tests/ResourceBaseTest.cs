#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="ResourceBaseTest.cs" company="Klarna AB">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// The resource test base.
    /// </summary>
    public abstract class ResourceBaseTest
    {
        #region Fields

        /// <summary>
        /// Data used in tests.
        /// </summary>
        public const string TheUrl = "http://klarna.com";

        /// <summary>
        /// Int data used in tests
        /// </summary>
        public const int TheInt = 89;

        /// <summary>
        /// String data used in tests
        /// </summary>
        public const string TheString = "A string";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the date time used in tests
        /// </summary>
        public DateTime TheDateTime { get; set; }

        /// <summary>
        /// Gets or sets the mocked connector
        /// </summary>
        public Mock<IConnector> MockConnector { get; set; }

        /// <summary>
        /// Gets the resource under testing
        /// </summary>
        public abstract Resource Resource { get; }

        #endregion

        #region SetUp

        /// <summary>
        /// The set up before each test.
        /// </summary>
        [SetUp]
        public void BaseSetUp()
        {
            this.TheDateTime = new DateTime(2012, 10, 14, 22, 53, 12);
            this.MockConnector = new Mock<IConnector>();
        }
     
        #endregion

        #region Methods

        /// <summary>
        /// Creates test data.
        /// </summary>
        /// <returns>
        /// The new test data.
        /// </returns>
        public Dictionary<string, object> TestData()
        {
            return new Dictionary<string, object>
                {
                    { "Int", TheInt },
                    { "String", TheString },
                    { "DateTime", this.TheDateTime }
                };
        }

        #endregion

        #region Tests

        /// <summary>
        /// The construction with connector.
        /// </summary>
        [Test]
        public void ConstructionWithConnector()
        {
            Assert.That(this.Resource.BaseUri, Is.Null);
            Assert.That(this.Resource.Location, Is.Null);
            Assert.That(this.Resource.ContentType, Is.Null);
            Assert.That(this.Resource.Accept, Is.Null);
            Assert.That(this.Resource.Marshal(), Is.Empty);
        }

        /// <summary>
        /// Tests that the content type is correct.
        /// </summary>
        [Test]
        public void ContentType()
        {
            const string ContentType = "application/vnd.klarna-content-v1+json";

            Assert.That(this.Resource.ContentType, Is.Null);
            this.Resource.ContentType = ContentType;
            Assert.That(this.Resource.ContentType, Is.EqualTo(ContentType));
        }

        /// <summary>
        /// Tests that accept is correct.
        /// </summary>
        [Test]
        public void Accept()
        {
            const string Accept = "application/vnd.klarna-accept-v1+json";

            Assert.That(this.Resource.Accept, Is.Null);
            this.Resource.Accept = Accept;
            Assert.That(this.Resource.Accept, Is.EqualTo(Accept));
        }

        /// <summary>
        /// Tests that the location is not initialized.
        /// </summary>
        [Test]
        public void LocationNull()
        {
            Assert.That(this.Resource.Location, Is.Null);
        }

        /// <summary>
        /// Tests set/get location.
        /// </summary>
        [Test]
        public void LocationSetGet()
        {
            Assert.That(this.Resource.Location, Is.Null);
            this.Resource.Location = new Uri(TheUrl);
            Assert.That(this.Resource.Location, Is.EqualTo(new Uri(TheUrl)));
        }

        /// <summary>
        /// Tests that parse works correctly.
        /// </summary>
        public void Parse()
        {
            var newData = TestData();
            this.Resource.Parse(newData);

            var data = this.Resource.Marshal();

            Assert.That(data, Is.EqualTo(newData));
        }

        /// <summary>
        /// Tests that marshal works correctly.
        /// </summary>
        [Test]
        public void Marshal()
        {
            var newData = TestData();
            this.Resource.Parse(newData);

            var data = this.Resource.Marshal();
            Assert.That(data, Is.TypeOf<Dictionary<string, object>>());
            Assert.That(data["Int"], Is.TypeOf<int>());
            Assert.That((int)data["Int"], Is.EqualTo(TheInt));
            Assert.That(data["String"], Is.TypeOf<string>());
            Assert.That(data["String"], Is.EqualTo(TheString));
            Assert.That(data["DateTime"], Is.TypeOf<DateTime>());
            Assert.That((DateTime)data["DateTime"], Is.EqualTo(TheDateTime));
        }

        /// <summary>
        /// Tests set/get values.
        /// </summary>
        [Test]
        public void ValuesGet()
        {
            var data = TestData();
            this.Resource.Parse(data);

            var intData = this.Resource.GetValue("Int");
            Assert.That(intData, Is.TypeOf<int>());
            Assert.That((int)intData, Is.EqualTo(TheInt));

            var stringData = this.Resource.GetValue("String");
            Assert.That(stringData, Is.TypeOf<string>());
            Assert.That(stringData, Is.EqualTo(TheString));

            var dateTimeData = this.Resource.GetValue("DateTime");
            Assert.That(dateTimeData, Is.TypeOf<DateTime>());
            Assert.That((DateTime)dateTimeData, Is.EqualTo(TheDateTime));
        }

        /// <summary>
        /// Tests that get value with null key or a non-existing key throws an exception.
        /// </summary>
        [Test]
        public void ValuesGetExeption()
        {
            Assert.Throws<ArgumentNullException>(() => this.Resource.GetValue(null));
            Assert.Throws<KeyNotFoundException>(() => this.Resource.GetValue("NonExistingKey"));
        }

        #endregion
    }
}
