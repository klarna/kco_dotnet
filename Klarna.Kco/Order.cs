#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="Order.cs" company="Klarna AB">
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
namespace Klarna.Checkout
{
    using System;

    /// <summary>
    /// The order resource.
    /// </summary>
    public class Order : IResource
    {
        #region Private Fields

        /// <summary>
        /// The connector.
        /// </summary>
        private IConnector connector;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        /// <param name="connector">
        /// The connector.
        /// </param>
        public Order(IConnector connector)
        {
            this.connector = connector;
        }

        #endregion

        #region Implementation of IResource

        /// <summary>
        /// Gets or sets the URL of the resource.
        /// </summary>
        public Uri Location { get; set; }

        /// <summary>
        /// Gets the content type of the resource.
        /// </summary>
        public string ContentType
        {
            get { return "application/vnd.klarna.checkout.aggregated-order-v1+json"; }
        }

        /// <summary>
        /// Update resource with the new data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void Parse(object data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Basic representation of the resource.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Marshal()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}