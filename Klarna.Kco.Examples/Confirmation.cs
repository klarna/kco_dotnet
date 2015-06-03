#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="Confirmation.cs" company="Klarna AB">
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
    /// The confirmation example.
    /// </summary>
    public class Confirmation
    {
        /// <summary>
        /// This example demonstrates the use of the Klarna library to complete
        /// the purchase and display the confirmation page snippet.
        /// </summary>
        public static void Main()
        {
            try
            {
                const string SharedSecret = "sharedSecret";
                // Retrieve location from session object.
                // Use following in ASP.NET.
                // var checkoutId = Session["klarna_order_id"] as string;
                // Just a placeholder in this example.
                const string OrderID = "ABC123";

                var connector = Connector.Create(SharedSecret, Connector.TestBaseUri);

                var order = new Order(connector, OrderID);

                order.Fetch();

                if ((string)order.GetValue("status") == "checkout_incomplete")
                {
                    // Report error

                    // Use following in ASP.NET.
                    // Response.Write("Checkout not completed, redirect to checkout.aspx");
                }

                // Display thank you snippet
                var gui = order.GetValue("gui") as JObject;
                var snippet = gui["snippet"];

                // DESKTOP: Width of containing block shall be at least 750px
                // MOBILE: Width of containing block shall be 100% of browser
                // window (No padding or margin)
                // Use following in ASP.NET.
                // Response.Write(string.Format("<div>{0}</div>", snippet));

                // Clear session object.
                // Session["klarna_order_id"] = null;
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