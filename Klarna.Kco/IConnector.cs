#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="IConnector.cs" company="Klarna AB">
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
    using System.Collections.Generic;
    using Klarna.Checkout.HTTP;

    /// <summary>
    /// The Connector interface.
    /// </summary>
    public interface IConnector
    {
        /// <summary>
        /// Gets or sets the user agent used for User-Agent header.
        /// </summary>
        UserAgent UserAgent { get; set; }

        /// <summary>
        /// Gets the transport used for the HTTP communications.
        /// </summary>
        IHttpTransport Transport { get; }

        /// <summary>
        /// Applies a HTTP method on a specific resource.
        /// </summary>
        /// <param name="method">
        /// The HTTP method.
        /// </param>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        void Apply(HttpMethod method, IResource resource, Dictionary<string, object> options);
    }
}