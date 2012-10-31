#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="IHttpResponse.cs" company="Klarna AB">
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
namespace Klarna.Checkout.HTTP
{
    using System.Net;

    /// <summary>
    /// The HttpResponse interface.
    /// </summary>
    public interface IHttpResponse
    {
        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the response data.
        /// </summary>
        string Data { get; }

        /// <summary>
        /// Gets the specified HTTP header.
        /// </summary>
        /// <param name="name">
        /// The name of the header.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string Header(string name);
    }
}