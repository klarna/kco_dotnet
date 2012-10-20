#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="UserAgentTest.cs" company="Klarna AB">
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
    using System.Text.RegularExpressions;
    using NUnit.Framework;

    /// <summary>
    /// Tests the UserAgent class.
    /// </summary>
    [TestFixture]
    public class UserAgentTest
    {
        #region Private Fields

        /// <summary>
        /// The user agent used in tests.
        /// </summary>
        private UserAgent userAgent;

        #endregion

        #region Set Up

        /// <summary>
        /// The set up before each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            userAgent = new UserAgent();
        }

        #endregion

        #region Tests

        /// <summary>
        /// Tests Default UA string.
        /// </summary>
        [Test]
        public void Default()
        {
            var userAgentString = userAgent.ToString();
            Assert.That(Regex.IsMatch(userAgentString, @"^.*Library\/[^\ ]+_[^\ ]+.*"), Is.True);
            Assert.That(Regex.IsMatch(userAgentString, @"^.*OS\/[^\ ]+_[^\ ]+.*"), Is.True);
            Assert.That(Regex.IsMatch(userAgentString, @"^.*Language\/[^\ ]+_[^\ ]+.*"), Is.True);
            Assert.That(Regex.IsMatch(userAgentString, @"^.*Webserver\/[^\ ]+_[^\ ]+.*"), Is.True);
        }

        #endregion
    }
}