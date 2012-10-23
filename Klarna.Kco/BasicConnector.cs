#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="BasicConnector.cs" company="Klarna AB">
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
    using System.Collections.Generic;
    using Klarna.Checkout.HTTP;

    /// <summary>
    /// The basic connector.
    /// </summary>
    public class BasicConnector : IConnector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicConnector"/> class.
        /// </summary>
        /// <param name="httpTransport">
        /// The http transport.
        /// </param>
        /// <param name="digest">
        /// The digest.
        /// </param>
        /// <param name="secret">
        /// The secret.
        /// </param>
        public BasicConnector(IHttpTransport httpTransport, Digest digest, string secret)
        {
        }

        #region Implementation of IConnector

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
        public void Apply(HttpMethod method, IResource resource, Dictionary<string, object> options)
        {
        }

        #endregion
    }
}