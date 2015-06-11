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
    using System.Net;
    using System.Diagnostics;

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
            const string Eid = "0";
            const string SharedSecret = "sharedSecret";

            var connector = Connector.Create(SharedSecret, Connector.TestBaseUri);

            Order order = new Order(connector);

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

            var merchant = new Dictionary<string, object>
                {
                    { "id", Eid },
                    { "terms_uri", "http://example.com/terms.aspx" },
                    { "checkout_uri", "https://example.com/checkout.aspx" },
                    {
                        "confirmation_uri",
                        "https://example.com/thankyou.aspx" +
                            "?klarna_order_id={checkout.order.id}"
                    },
                    //// You cannot receive push notification on a non publicly
                    //// available uri.
                    {
                        "push_uri",
                        "https://example.com/push.aspx" +
                            "?klarna_order_id={checkout.order.id}" }
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

            //data.Add("recurring", true);

            try
            {
                order.Create(data);
                order.Fetch();

                string orderID = order.GetValue("id") as string;

                Debug.WriteLine("Order ID: " + orderID);
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