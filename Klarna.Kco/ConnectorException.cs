#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="ConnectorException.cs" company="Klarna AB">
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
    using System.Runtime.Serialization;

    /// <summary>
    /// The connector exception.
    /// </summary>
    [Serializable]
    public class ConnectorException : Exception
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorException"/> class.
        /// </summary>
        public ConnectorException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public ConnectorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public ConnectorException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected ConnectorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
