#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="RecurringFetch.cs" company="Klarna AB">
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
namespace Klarna.Kco.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Text;
    using Klarna.Checkout;

    /// <summary>
    /// The fetch recurring order status example.
    /// </summary>
    public class RecurringFetch
    {
        /// <summary>
        /// The example.
        /// </summary>
        public void Example()
        {
            /*
             Note! First you must've created a regular aggregated order with the option "recurring" set to true.
             After that order has recieved the status "checkout_complete" you can fetch that resource and retrieve
             the "recurring_token" property which is needed to fetch a recurring order status.
             */

            const string ContentType = "application/vnd.klarna.checkout.recurring-status-v1+json";
            const string SharedSecret = "sharedSecret";

            RecurringStatus status = null;
            var connector = Connector.Create(SharedSecret);

            string uri = "https://checkout.testdrive.klarna.com/checkout/recurring/{0}";
            string recurring_token = "ABC-123";

            try
            {
                status = new RecurringStatus(connector)
                    {
                        BaseUri = new Uri(string.Format(uri, recurring_token)),
                        ContentType = ContentType
                    };

                status.Fetch();

                // Get the payment method details
                var payment_method = status.GetValue("payment_method") as Dictionary<string, object>;

                // Find out what type of payment method, can be either "invoice" or "credit_card".
                string type = payment_method["type"] as string;

                Debug.Print(type);

                // If the type was "credit_card" there will also be a "credit_card_data" property.
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

                if (ex.Data.Contains("reason"))
                {
                    // For instance, Content-Type application/vnd.klarna.checkout.recurring-order-rejected-v1+json
                    // has "reason".
                    var reason = (string)ex.Data["reason"];
                    Debug.WriteLine(reason);
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
