#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="Push.cs" company="Klarna AB">
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

                var order =
                    new Order { ContentType = "application/vnd.klarna.checkout.aggregated-order-v2+json" };
                var connector = Connector.Create(SharedSecret);

                // Use following in ASP.NET.
                // var checkoutId = Request.QueryString["checkout_uri"];
                // Just a placeholder in this example.
                var checkoutId = "https://klarnacheckout.apiary.io/checkout/orders/12";

                order.Fetch(connector, new Uri(checkoutId));

                if ((string)order.GetValue("status") == "checkout_complete")
                {
                    // At this point make sure the order is created in your
                    // system and send a confirmation email to the customer.
                    //order.SetValue("status", "created");
                    var uniqueId = Guid.NewGuid().ToString("N");
                    //order.SetValue("merchant_reference",
                    //    new Dictionary<string, object> { { "orderid1", uniqueId } });
                    order.Update(connector);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}