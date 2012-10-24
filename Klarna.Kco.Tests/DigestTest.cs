#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="DigestTest.cs" company="Klarna AB">
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
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using NUnit.Framework;

    /// <summary>
    /// Tests the Digest class.
    /// </summary>
    [TestFixture]
    public class DigestTest
    {
        /// <summary>
        /// Tests that creation of digest string is correct.
        /// </summary>
        [Test]
        public void CreateDigest()
        {
            var article = new Dictionary<string, object>()
                {
                    { "artno", "id_1" }, 
                    { "name", "product" }, 
                    { "price", 12345 }, 
                    { "vat", 25 }, 
                    { "qty", 1 }
                };

            var goodsList = new List<Dictionary<string, object>>() { article };

            var data = new Dictionary<string, object>()
                {
                    { "eid", 1245 }, 
                    { "goods_list", goodsList }, 
                    { "currency", "SEK" }, 
                    { "country", "SWE" }, 
                    { "language", "SV" }
                };

            var json = JsonConvert.SerializeObject(data);

            var digest = new Digest();
            var actual = digest.Create(string.Concat(json, "mySecret"));
            const string Expected = "MO/6KvzsY2y+F+/SexH7Hyg16gFpsPDx5A2PtLZd0Zs=";

            Assert.That(actual, Is.EqualTo(Expected));
        }
    }
}
