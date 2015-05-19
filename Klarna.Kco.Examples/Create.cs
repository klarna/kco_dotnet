#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="Create.cs" company="Klarna AB">
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
    /// The create checkout example.
    /// </summary>
    public class Create
    {
        /// <summary>
        /// The example.
        /// </summary>
        public static void Main()
        {
            var items = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                            {
                                { "reference", "123456789" },
                                { "name", "Klarna t-shirt" },
                                { "quantity", 2 },
                                { "unit_price", 12300 },
                                { "discount_rate", 1000 },
                                { "tax_rate", 2500 }
                            },
                        new Dictionary<string, object>
                            {
                                { "type", "shipping_fee" },
                                { "reference", "SHIPPING" },
                                { "name", "Shipping Fee" },
                                { "quantity", 1 },
                                { "unit_price", 4900 },
                                { "tax_rate", 2500 }
                            }
                    };
            var cart = new Dictionary<string, object> { { "items", items } };

            // Merchant ID
            const string Eid = "0";

            const string SharedSecret = "sharedSecret";
            var connector = Connector.Create(SharedSecret, Connector.TestBaseUri);

            Order order = null;

            var merchant = new Dictionary<string, object>
                {
                    { "id", Eid },
                    { "terms_uri", "http://example.com/terms.aspx" },
                    { "checkout_uri", "https://example.com/checkout.aspx" },
                    {
                        "confirmation_uri",
                        "https://example.com/thankyou.aspx?sid=123&klarna_order={checkout.order.uri}"
                    },
                    //// You cannot receive push notification on a non publicly available uri.
                    { "push_uri", "https://example.com/push.aspx?sid=123&klarna_order={checkout.order.uri}" }
                };

            var layout = new Dictionary<string, object>
                {
                    { "layout", "desktop" } // or mobile
                };

            var data = new Dictionary<string, object>
                {
                    { "purchase_country", "SE" },
                    { "purchase_currency", "SEK" },
                    { "locale", "sv-se" },
                    { "merchant", merchant },
                    { "cart", cart },
                    { "gui", layout }
                };

            order = new Order(connector);

            order.Create(data);
        }
    }
}