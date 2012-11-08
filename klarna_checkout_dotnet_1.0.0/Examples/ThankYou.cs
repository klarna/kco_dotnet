#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="ThankYou.cs" company="Klarna AB">
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
    /// The thank you example.
    /// </summary>
    public class ThankYou
    {
        /// <summary>
        /// The example.
        /// </summary>
        public void Example()
        {
            try
            {
                // Shared secret
                const string SharedSecret = "sharedSecret";

                var order = new Order();
                var connector = Connector.Create(SharedSecret);

                // Retrieve location from session object.
                // Use following in ASP.NET.
                // var checkoutId = Session["klarna_checkout"] as Uri;
                // Just a placeholder in this example.
                var checkoutId = "https://klarnacheckout.apiary.io/checkout/orders/12";

                order.Fetch(connector, new Uri(checkoutId));

                if ((string)order.GetValue("status") != "checkout_complete")
                {
                    // Report error

                    // Use following in ASP.NET.
                    // Response.Write("Checkout not completed, redirect to checkout.aspx");
                }

                // Display thank you snippet
                var gui = (Dictionary<string, object>)order.GetValue("gui");
                var snippet = gui["snippet"];

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