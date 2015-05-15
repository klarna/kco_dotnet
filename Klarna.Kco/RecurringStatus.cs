#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="RecurringStatus.cs" company="Klarna AB">
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
namespace Klarna.Checkout
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The recurring order status resource.
    /// </summary>
    public class RecurringStatus : Resource
    {
        /// <summary>
        /// Relative resource path
        /// </summary>
        private string relativePath = "/checkout/recurring/TOKEN";

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurringStatus" /> class.
        /// </summary>
        /// <param name="connector">
        /// The connector to use.
        /// </param>
        /// <param name="recurringToken">
        /// The recurring token
        /// </param>
        public RecurringStatus(IConnector connector, string recurringToken)
            : base(connector)
        {
            this.Location = new Uri(this.Connector.BaseUri, this.relativePath.Replace("TOKEN", recurringToken));
            this.ContentType = "application/vnd.klarna.checkout.recurring-status-v1+json";
            this.Accept = this.ContentType;
        }

        /// <summary>
        /// Get the payment details status of a recurring order, using the uri in BaseUri
        /// </summary>
        public void Fetch()
        {
            var options = new Dictionary<string, object>
                {
                    { "url", this.Location }
                };

            this.Connector.Apply(HttpMethod.Get, this, options);
        }
    }
}
