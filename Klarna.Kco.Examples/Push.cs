#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="Push.cs" company="Klarna AB">
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
// <link>http://developers.klarna.com/</link>
// ----------------------------------------------------------------------------
#endregion
// [[examples-push]]
namespace Klarna.Kco.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using Klarna.Checkout;

    /// <summary>
    /// The push example.
    /// </summary>
    public class Push
    {
        /// <summary>
        /// The example.
        /// </summary>
        public void Example()
        {
            try
            {
                const string SharedSecret = "sharedSecret";
                var connector = Connector.Create(SharedSecret);

                // Retrieve location from query string.
                // Use following in ASP.NET.
                // var checkoutId = Request.QueryString["checkout_uri"] as Uri;
                // Just a placeholder in this example.
                Uri checkoutId = new Uri(
                    "https://checkout.testdrive.klarna.com/checkout/orders/12");

                var order = new Order(connector, checkoutId)
                    {
                        ContentType = "application/vnd.klarna.checkout.aggregated-order-v2+json"
                    };

                order.Fetch();

                if ((string)order.GetValue("status") == "checkout_complete")
                {
                    // At this point make sure the order is created in your
                    // system and send a confirmation email to the customer.
                    var uniqueId = Guid.NewGuid().ToString("N");
                    var reference =
                        new Dictionary<string, object>
                            {
                                { "orderid1", uniqueId }
                            };
                    var data =
                        new Dictionary<string, object>
                            {
                                { "status", "created" },
                                { "merchant_reference", reference }
                            };

                    order.Update(data);
                }
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

// [[examples-push]]
