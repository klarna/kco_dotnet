#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="Fetch.cs" company="Klarna AB">
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
// <link>http://integration.klarna.com/</link>
// ----------------------------------------------------------------------------
#endregion
namespace Klarna.Kco.Examples
{
    using System;
    using System.Collections.Generic;
    using Klarna.Checkout;

    /// <summary>
    /// The fetch checkout example.
    /// </summary>
    public class Fetch
    {
        /// <summary>
        /// The example.
        /// </summary>
        public void Example()
        {
            Uri resourceUri = new Uri("https://checkout.testdrive.klarna.com/checkout/orders/ABC123");

            const string SharedSecret = "sharedSecret";
            var connector = Connector.Create(SharedSecret, Connector.TestBaseUri);

            Order order = new Order(connector, resourceUri);

            order.Fetch();
        }
    }
}