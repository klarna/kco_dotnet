#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="RecurringStatusTest.cs" company="Klarna AB">
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
    /// Tests the RecurringStatus class.
    /// </summary>
    [TestFixture]
    public class RecurringStatusTest : ResourceBaseTest
    {
        #region Properties

        /// <summary>
        /// The real resource under testing
        /// </summary>
        private RecurringStatus status;

        /// <summary>
        /// Gets the resource under testing
        /// </summary>
        public override Resource Resource
        {
            get
            {
                return this.status;
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
            this.status = new RecurringStatus(this.MockConnector.Object);
        }

        #endregion

        #region Tests

        /// <summary>
        /// Tests that Fetch works correctly.
        /// </summary>
        [Test]
        public void Fetch()
        {
            this.status.BaseUri = new Uri("http://klarna.com/foo/bar/15");

            var options = new Dictionary<string, object> { { "url", this.status.BaseUri } };
            MockConnector.Setup(c => c.Apply(HttpMethod.Get, this.status, options)).Verifiable();
            this.status.Fetch();

            MockConnector.Verify();
        }

        #endregion
    }
}
