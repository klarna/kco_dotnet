#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="Connector.cs" company="Klarna AB">
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
    using Klarna.Checkout.HTTP;

    /// <summary>
    /// The connector factory.
    /// </summary>
    public class Connector
    {
        /// <summary>
        /// Gets the base URI for the API.
        /// </summary>
        public static Uri BaseUri
        {
            get
            {
                return new Uri("https://checkout.klarna.com");
            }
        }

        /// <summary>
        /// Gets the test base URI for the API.
        /// </summary>
        public static Uri TestBaseUri
        {
            get
            {
                return new Uri("https://checkout.testdrive.klarna.com");
            }
        }

        /// <summary>
        /// Creates a connector with custom base URI.
        /// </summary>
        /// <param name="secret">
        /// The string used to sign requests.
        /// </param>
        /// <returns>
        /// The <see cref="IConnector"/>.
        /// </returns>
        /// <param name="uri">
        /// The uri for the connector to use.
        /// </param>
        public static IConnector Create(string secret, Uri uri)
        {
            var httpTransport = HttpTransport.Create();
            var digest = new Digest();
            IConnector connector = new BasicConnector(httpTransport, digest, secret, uri);

            return connector;
        }

        /// <summary>
        /// Creates a connector for the base URI.
        /// </summary>
        /// <param name="secret">
        /// The string used to sign requests.
        /// </param>
        /// <returns>
        /// The <see cref="IConnector"/>.
        /// </returns>
        public static IConnector Create(string secret)
        {
            return Create(secret, Connector.BaseUri);
        }
    }
}