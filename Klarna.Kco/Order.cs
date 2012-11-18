#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="Order.cs" company="Klarna AB">
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
namespace Klarna.Checkout
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The order resource.
    /// </summary>
    public class Order : IResource
    {
        #region Private Fields

        /// <summary>
        /// The data.
        /// </summary>
        private Dictionary<string, object> resourceData;

        /// <summary>
        /// The connector.
        /// </summary>
        private readonly IConnector connector;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="Order" /> class.
        /// </summary>
        /// <param name="connector">
        /// The connector to use.
        /// </param>
        public Order(IConnector connector)
        {
            resourceData = new Dictionary<string, object>();
            this.connector = connector;
            BaseUri = null;
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
            Location = uri;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the base uri that is used to create order resources.
        /// </summary>
        public Uri BaseUri { get; set; }

        #endregion

        #region Implementation of IResource

        /// <summary>
        /// Gets or sets the uri of the resource.
        /// </summary>
        public Uri Location { get; set; }

        /// <summary>
        /// Gets or sets the content type of the resource.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Replace resource with the new data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void Parse(Dictionary<string, object> data)
        {
            resourceData = data;
        }

        /// <summary>
        /// Basic representation of the resource.
        /// </summary>
        /// <returns>
        /// The <see cref="object" />.
        /// </returns>
        public Dictionary<string, object> Marshal()
        {
            return resourceData;
        }

        #endregion

        #region Resource Data Accessors

        /// <summary>
        /// Gets the value of a key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// key is null.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// key does not exist.
        /// </exception>
        /// <returns>
        /// The <see cref="object" />.
        /// </returns>
        public object GetValue(string key)
        {
            return resourceData[key];
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new order, using the uri in BaseUri.
        /// </summary>
        public void Create(Dictionary<string, object> data)
        {
            var options =
                new Dictionary<string, object>
                    {
                        { "url", BaseUri },
                        {"data", data}
                    };
            connector.Apply(HttpMethod.Post, this, options);
        }

        /// <summary>
        /// Fetches order data.
        /// </summary>
        public void Fetch()
        {
            var options =
                new Dictionary<string, object>
                    {
                        { "url", Location }
                    };
            connector.Apply(HttpMethod.Get, this, options);
        }

        /// <summary>
        /// Updates order data.
        /// </summary>
        public void Update(Dictionary<string, object> data)
        {
            var options =
                new Dictionary<string, object>
                    {
                        { "url", Location },
                        {"data", data}
                    };
            connector.Apply(HttpMethod.Post, this, options);
        }

        #endregion
    }
}