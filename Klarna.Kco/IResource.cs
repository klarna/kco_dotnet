#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="IResource.cs" company="Klarna AB">
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
    using System.Collections.Generic;

    /// <summary>
    /// The Resource interface.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Gets or sets the URL of the resource.
        /// </summary>
        Uri Location { get; set; }

        /// <summary>
        /// Gets or sets the content type of the resource.
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Update resource with the new data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        void Parse(Dictionary<string, object> data);

        /// <summary>
        /// Basic representation of the resource.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        Dictionary<string, object> Marshal();
    }
}