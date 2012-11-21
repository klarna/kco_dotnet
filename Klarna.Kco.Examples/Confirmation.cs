#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="Confirmation.cs" company="Klarna AB">
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
    /// The confirmation example.
    /// </summary>
    public class Confirmation
    {
        /// <summary>
        /// This example demonstrates the use of the Klarna library to complete
        /// the purchase and display the confirmation page snippet.
        /// </summary>
        public void Example()
        {
            try
            {
                const string SharedSecret = "sharedSecret";
                var connector = Connector.Create(SharedSecret);

                // Retrieve location from session object.
                // Use following in ASP.NET.
                // var checkoutId = Session["klarna_checkout"] as Uri;
                // Just a placeholder in this example.
                var checkoutId = new Uri(
                    "https://checkout.testdrive.klarna.com" +
                    "/checkout/orders/ABC123"
                );
                var order = new Order(connector, checkoutId)
                {
                    ContentType = "application/vnd.klarna.checkout.aggregated-order-v2+json"
                };

                order.Fetch();

                if ((string)order.GetValue("status") == "checkout_incomplete")
                {
                    // Report error

                    // Use following in ASP.NET.
                    // Response.Write("Checkout not completed, redirect to checkout.aspx");
                }

                // Display thank you snippet
                var gui = (Dictionary<string, object>)order.GetValue("gui");
                var snippet = gui["snippet"];

                // DESKTOP: Width of containing block shall be at least 750px
                // MOBILE: Width of containing block shall be 100% of browser
                // window (No padding or margin)
                // Use following in ASP.NET.
                // Response.Write(string.Format("<div>{0}</div>", snippet));

                // Clear session object.
                // Session["klarna_checkout"] = null;
            }
            catch (Exception ex)
            {
            }
        }
    }
}
