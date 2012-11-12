#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="Checkout.cs" company="Klarna AB">
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
namespace Klarna.Kco.Examples
{
    using System;
    using System.Collections.Generic;

    using Klarna.Checkout;

    /// <summary>
    /// The checkout example.
    /// </summary>
    public class Checkout
    {
        /// <summary>
        /// The example.
        /// </summary>
        public void Example()
        {
            // Note! Please remove the code below when used in ASP.NET.
            // Just a placeholder in this example for, the HttpSessionState object, Session.
            var Session = new Dictionary<string, object>();

            try
            {
                // Merchant ID
                const int Eid = 2;

                // Shared secret
                const string SharedSecret = "sharedSecret";

                var order = new Order
                    {
                        BaseUri = new Uri("https://klarnacheckout.apiary.io/checkout/orders"),
                        ContentType = "application/vnd.klarna.checkout.aggregated-order-v2+json"
                    };

                var connector = Connector.Create(SharedSecret);

                // Retrieve location from session object.
                var resourceUri = Session["klarna_checkout"] as Uri;
                if (resourceUri == null)
                {
                    // Start a new session
                    order.SetValue("purchase_country", "SE");
                    order.SetValue("purchase_currency", "SEK");
                    order.SetValue("locale", "sv-se");

                    var merchant = new Dictionary<string, object>
                        {
                            { "id", Eid }, 
                            { "terms_uri", "http://localhost/terms.html" }, 
                            { "checkout_uri", "http://localhost/checkout.aspx" }, 
                            { "confirmation_uri", "http://localhost/thank-you.aspx" }, 
                            //// You cannot recieve push notification on a non publicly available uri.
                            { "push_uri", "http://localhost/push.aspx" } 
                        };
                    order.SetValue("merchant", merchant);

                    var cartItems = new List<Dictionary<string, object>>
                        {
                            new Dictionary<string, object>
                                {
                                    { "reference", "BANAN01" }, 
                                    { "name", "Bananana" }, 
                                    { "unit_price", 450 }, 
                                    { "discount_rate", 0 }, 
                                    { "tax_rate", 2500 }
                                }, 
                            new Dictionary<string, object>
                                {
                                    { "type", "shipping_fee" }, 
                                    { "reference", "SHIPPING" }, 
                                    { "name", "Shipping Fee" }, 
                                    { "unit_price", 450 }, 
                                    { "discount_rate", 0 }, 
                                    { "tax_rate", 2500 }
                                }
                        };
                    order.SetValue("cart", new Dictionary<string, object> { { "items", cartItems } });

                    order.Create(connector);
                    order.Fetch(connector);
                }
                else
                {
                    // Resume session
                    order.Fetch(connector, resourceUri);
                }

                // Store location of checkout session is session object.
                Session["klarna_checkout"] = order.Location;

                // Display checkout
                var gui = (Dictionary<string, object>)order.GetValue("gui");
                var snippet = gui["snippet"];

                // Use following in ASP.NET.
                // Response.Write(string.Format("<div>{0}</div>", snippet));
            }
            catch (Exception ex)
            {
                // Handle exception.
            }
        }
    }
}