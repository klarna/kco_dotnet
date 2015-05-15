#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="Checkout.cs" company="Klarna AB">
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
namespace Klarna.Kco.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using Klarna.Checkout;
    using Newtonsoft.Json.Linq;

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
            var session = new Dictionary<string, object>();

            try
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

                Uri resourceUri = null;

                // Retrieve location from session object.
                if (session.ContainsKey("klarna_checkout"))
                {
                    resourceUri = session["klarna_checkout"] as Uri;
                }

                if (resourceUri != null)
                {
                    try
                    {
                        order = new Order(connector, resourceUri);

                        order.Fetch();

                        // Reset cart
                        var data = new Dictionary<string, object> { { "cart", cart } };

                        order.Update(data);
                    }
                    catch (Exception)
                    {
                        // Reset session
                        order = null;
                        session["klarna_checkout"] = null;
                    }
                }

                if (order == null)
                {
                    // Start a new session
                    var merchant = new Dictionary<string, object>
                        {
                            { "id", Eid },
                            { "terms_uri", "http://example.com/terms.aspx" },
                            {
                                "checkout_uri",
                                "https://example.com/checkout.aspx"
                            },
                            {
                                "confirmation_uri",
                                "https://example.com/thankyou.aspx" +
                                "?sid=123&klarna_order={checkout.order.uri}"
                            },
                            //// You cannot receive push notification on a
                            //// non publicly available uri.
                            {
                                "push_uri",
                                "https://example.com/push.aspx" +
                                "?sid=123&klarna_order={checkout.order.uri}"
                            }
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
                    order.Fetch();
                }

                // Store location of checkout session is session object.
                session["klarna_checkout"] = order.Location;

                // Display checkout
                var gui = order.GetValue("gui") as JObject;
                var snippet = gui["snippet"];

                // DESKTOP: Width of containing block shall be at least 750px
                // MOBILE: Width of containing block shall be 100% of browser
                // window (No padding or margin)
                // Use following in ASP.NET.
                // Response.Write(string.Format("<div>{0}</div>", snippet));
            }
            catch (ConnectorException ex)
            {
                var webException = ex.InnerException as WebException;
                if (webException != null)
                {
                    // Here you can check for timeouts, and other connection related errors.
                    // webException.Response could contain the response object.
                }
                else
                {
                    // In case there wasn't a WebException where you could get the response
                    // (e.g. a protocol error, bad digest, etc) you might still be able to
                    // get a hold of the response object.
                    // ex.Data["Response"] as IHttpResponse
                }

                // Additional data might be available in ex.Data.
                if (ex.Data.Contains("internal_message"))
                {
                    // For instance, Content-Type application/vnd.klarna.error-v1+json has "internal_message".
                    var internalMessage = (string)ex.Data["internal_message"];
                    Debug.WriteLine(internalMessage);
                }

                throw;
            }
            catch (Exception)
            {
                // Something else went wrong, e.g. invalid arguments passed to the order object.
                throw;
            }
        }
    }
}