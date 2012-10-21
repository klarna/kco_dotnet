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
    using System;
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

        /// <summary>
        /// Test to add a field without options.
        /// </summary>
        [Test]
        public void AddFieldWithoutOptions()
        {
            userAgent.AddField("JS Lib", "jQuery", "1.8.2");
            var userAgentString = userAgent.ToString();
            Assert.That(Regex.IsMatch(userAgentString, @"^.*JS Lib\/jQuery_1.8.2.*"), Is.True);
        }

        /// <summary>
        /// Test to add a field with options.
        /// </summary>
        [Test]
        public void AddFieldWithOptions()
        {
            var options = new[] { "LanguagePack/7", "JsLib/2.0" };

            userAgent.AddField("Module", "Magento", "5.0", options);
            var userAgentString = userAgent.ToString();
            Assert.That(Regex.IsMatch(userAgentString, @"^.*Module\/Magento_5.0 \(LanguagePack\/7 ; JsLib\/2.0\).*"), Is.True);
        }

        /// <summary>
        /// Tests that redefinition of field throws an exception.
        /// </summary>
        [Test]
        public void CannotRedefineField()
        {
            const string Name = "None";
            const string Version = "0.0";
            Assert.Throws<ArgumentException>(() => this.userAgent.AddField("Library", Name, Version));
            Assert.Throws<ArgumentException>(() => this.userAgent.AddField("OS", Name, Version));
            Assert.Throws<ArgumentException>(() => this.userAgent.AddField("Language", Name, Version));
            Assert.Throws<ArgumentException>(() => this.userAgent.AddField("Webserver", Name, Version));
        }

        #endregion
    }
}