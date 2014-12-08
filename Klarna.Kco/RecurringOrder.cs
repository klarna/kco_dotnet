﻿#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="RecurringOrder.cs" company="Klarna AB">
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
    /// The recurring order resource.
    /// </summary>
    public class RecurringOrder : Resource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecurringOrder" /> class.
        /// </summary>
        /// <param name="connector">
        /// The connector to use.
        /// </param>
        public RecurringOrder(IConnector connector)
            : base(connector)
        {
        }

        /// <summary>
        /// Creates a new recurring order, using the uri in BaseUri.
        /// </summary>
        /// <param name="data">
        /// The order data.
        /// </param>
        public void Create(Dictionary<string, object> data)
        {
            var options = new Dictionary<string, object>
                {
                    { "url", this.BaseUri },
                    { "data", data }
                };

            this.Connector.Apply(HttpMethod.Post, this, options);
        }
    }
}