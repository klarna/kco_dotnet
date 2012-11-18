#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="Checkout.cs" company="Klarna AB">
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
                const string ContentType =
                    "application/vnd.klarna.checkout.aggregated-order-v2+json";

                // Cart
                var cartItems = new List<Dictionary<string, object>>
                        {
                            new Dictionary<string, object>
                                {
                                    { "quantity", 1 }, 
                                    { "reference", "BANAN01" }, 
                                    { "name", "Bananana" }, 
                                    { "unit_price", 450 }, 
                                    { "discount_rate", 0 }, 
                                    { "tax_rate", 2500 }
                                }, 
                            new Dictionary<string, object>
                                {
                                    { "quantity", 1 }, 
                                    { "type", "shipping_fee" }, 
                                    { "reference", "SHIPPING" }, 
                                    { "name", "Shipping Fee" }, 
                                    { "unit_price", 450 }, 
                                    { "discount_rate", 0 }, 
                                    { "tax_rate", 2500 }
                                }
                        };
                var cart = new Dictionary<string, object> { { "items", cartItems } };

                // Merchant ID
                const string Eid = "2";

                const string SharedSecret = "sharedSecret";
                var connector = Connector.Create(SharedSecret);

                Order order = null;

                // Retrieve location from session object.
                var resourceUri = Session["klarna_checkout"] as Uri;
                if (resourceUri != null)
                {
                    try
                    {
                        order = new Order(connector, resourceUri)
                                    {
                                        ContentType = ContentType
                                    };

                        order.Fetch();

                        // Reset cart
                        var data = new Dictionary<string, object> { { "cart", cart } };

                        order.Update(data);
                    }
                    catch (Exception)
                    {
                        // Reset session
                        order = null;
                        Session["klarna_checkout"] = null;
                    }
                }

                if (order == null)
                {
                    // Start a new session

                    var merchant = new Dictionary<string, object>
                        {
                            { "id", Eid }, 
                            { "terms_uri", "http://localhost/terms.html" }, 
                            { "checkout_uri", "http://localhost/checkout.aspx" }, 
                            { "confirmation_uri", "http://localhost/confirmation.aspx" }, 
                            //// You cannot recieve push notification on a non publicly available uri.
                            { "push_uri", "http://localhost/push.aspx" } 
                        };

                    var data =
                        new Dictionary<string, object>
                            {
                                { "purchase_country", "SE" },
                                { "purchase_currency", "SEK" },
                                { "locale", "sv-se" },
                                { "merchant", merchant},
                                { "cart", cart }
                            };

                    order =
                        new Order(connector)
                            {
                                BaseUri = new Uri("https://klarnacheckout.apiary.io/checkout/orders"),
                                ContentType = ContentType
                            };

                    order.Create(data);
                    order.Fetch();
                }

                // Store location of checkout session is session object.
                Session["klarna_checkout"] = order.Location;

                // Display checkout
                var gui = (Dictionary<string, object>)order.GetValue("gui");
                var snippet = gui["snippet"];

                // DESKTOP: Width of containing block shall be at least 750px
                // MOBILE: Width of containing block shall be 100% of browser
                // window (No padding or margin)
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