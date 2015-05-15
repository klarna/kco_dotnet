#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="Order.cs" company="Klarna AB">
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

    /// <summary>
    /// The order resource.
    /// </summary>
    public class Order : Resource
    {
        /// <summary>
        /// Relative resource path
        /// </summary>
        private string relativePath = "/checkout/orders";

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="Order" /> class.
        /// </summary>
        /// <param name="connector">
        /// The connector to use.
        /// </param>
        public Order(IConnector connector)
            : base(connector)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order" /> class.
        /// </summary>
        /// <param name="connector">
        /// The connector to use.
        /// </param>
        /// <param name="uri">
        /// The uri of the resource.
        /// </param>
        public Order(IConnector connector, Uri uri)
            : this(connector)
        {
            this.Location = uri;
            this.ContentType = "application/vnd.klarna.checkout.aggregated-order-v2+json";
            this.Accept = this.ContentType;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new order, using the uri in BaseUri.
        /// </summary>
        /// <param name="data">
        /// The order data.
        /// </param>
        public void Create(Dictionary<string, object> data)
        {
            var options = new Dictionary<string, object>
                {
                    { "url", new Uri(this.Connector.BaseUri, this.relativePath) },
                    { "data", data }
                };

            this.Connector.Apply(HttpMethod.Post, this, options);
        }

        /// <summary>
        /// Fetches order data.
        /// </summary>
        public void Fetch()
        {
            var options = new Dictionary<string, object>
                {
                    { "url", this.Location }
                };

            this.Connector.Apply(HttpMethod.Get, this, options);
        }

        /// <summary>
        /// Updates order data.
        /// </summary>
        /// <param name="data">
        /// The updated order data.
        /// </param>
        public void Update(Dictionary<string, object> data)
        {
            var options = new Dictionary<string, object>
                {
                    { "url", this.Location },
                    { "data", data }
                };

            this.Connector.Apply(HttpMethod.Post, this, options);
        }

        #endregion
    }
}