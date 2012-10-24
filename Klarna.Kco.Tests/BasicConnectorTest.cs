#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="BasicConnectorTest.cs" company="Klarna AB">
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
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using Klarna.Checkout.HTTP;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// The basic connector test.
    /// </summary>
    public class BasicConnectorTest
    {
        #region Private Fields

        /// <summary>
        /// The http transport mock.
        /// </summary>
        private Mock<IHttpTransport> httpTransportMock;

        #endregion

        #region Set up

        /// <summary>
        /// The set up before each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            httpTransportMock = new Mock<IHttpTransport>();
        }

        #endregion

        #region Tests

        /// <summary>
        /// Tests that the UserAgent property is correct.
        /// </summary>
        [Test]
        public void UserAgent()
        {
            var connector = 
                new BasicConnector(httpTransportMock.Object, new Digest(), "aboogie");
            Assert.That(connector.UserAgent, Is.Not.Null);
            var userAgentString = connector.UserAgent.ToString();
            Assert.That(Regex.IsMatch(userAgentString, @"^.*Library\/[^\ ]+_[^\ ]+.*"), Is.True);
            Assert.That(Regex.IsMatch(userAgentString, @"^.*OS\/[^\ ]+_[^\ ]+.*"), Is.True);
            Assert.That(Regex.IsMatch(userAgentString, @"^.*Language\/[^\ ]+_[^\ ]+.*"), Is.True);
            Assert.That(Regex.IsMatch(userAgentString, @"^.*Webserver\/[^\ ]+_[^\ ]+.*"), Is.True);
        }

        #endregion
    }
}
