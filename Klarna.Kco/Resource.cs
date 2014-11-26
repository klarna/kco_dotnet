#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="Resource.cs" company="Klarna AB">
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
// <link>http://developers.klarna.com/</link>
// ----------------------------------------------------------------------------
#endregion
namespace Klarna.Checkout
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The abstract Resource
    /// </summary>
    public abstract class Resource : IResource
    {
        #region Fields

        /// <summary>
        /// The connector.
        /// </summary>
        protected readonly IConnector Connector;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource" /> class.
        /// </summary>
        /// <param name="connector">
        /// The connector to use.
        /// </param>
        public Resource(IConnector connector)
        {
            this.Connector = connector;
            this.ResourceData = new Dictionary<string, object>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the base uri that is used to create resources.
        /// </summary>
        public Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets the uri of the resource.
        /// </summary>
        public Uri Location { get; set; }

        /// <summary>
        /// Gets or sets the content type of the resource.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the accept of the resource.
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        protected Dictionary<string, object> ResourceData { get; set; }

        #endregion

        #region Methods

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
            return this.ResourceData[key];
        }

        /// <summary>
        /// Replace resource with the new data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public virtual void Parse(Dictionary<string, object> data)
        {
            this.ResourceData = data;
        }

        /// <summary>
        /// Basic representation of the resource.
        /// </summary>
        /// <returns>
        /// The <see cref="object" />.
        /// </returns>
        public virtual Dictionary<string, object> Marshal()
        {
            return this.ResourceData;
        }

        #endregion
    }
}
