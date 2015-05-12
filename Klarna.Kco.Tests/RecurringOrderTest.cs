#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="RecurringOrderTest.cs" company="Klarna AB">
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
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Tests the RecurringOrder class.
    /// </summary>
    [TestFixture]
    public class RecurringOrderTest : ResourceBaseTest
    {
        #region Properties

        /// <summary>
        /// The real resource under testing
        /// </summary>
        private RecurringOrder recurring;

        /// <summary>
        /// Gets the resource under testing
        /// </summary>
        public override Resource Resource
        {
            get
            {
                return this.recurring;
            }
        }

        #endregion

        #region SetUp

        /// <summary>
        /// The set up before each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.MockConnector.Setup(c => c.BaseUrl).Returns(new Uri("http://test.com"));
            this.recurring = new RecurringOrder(this.MockConnector.Object, "ABC-123");
        }

        #endregion

        #region Tests

        /// <summary>
        /// Tests that Create works correctly.
        /// </summary>
        [Test]
        public void Create()
        {
            var data = new Dictionary<string, object> { { "foo", "boo" } };
            var options = new Dictionary<string, object>
                {
                    { "url", new Uri("http://test.com/checkout/recurring/ABC-123/orders") },
                    { "data", data }
                };
            this.MockConnector.Setup(c => c.Apply(HttpMethod.Post, this.recurring, options)).Verifiable();

            this.recurring.Create(data);

            this.MockConnector.Verify();
        }

        #endregion
    }
}
