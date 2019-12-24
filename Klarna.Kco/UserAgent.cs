#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="UserAgent.cs" company="Klarna AB">
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
    using System.Reflection;
    using System.Text;
    using Microsoft.Win32;

    /// <summary>
    /// The user agent string creation class.
    /// </summary>
    public class UserAgent
    {
        #region Private Fields

        /// <summary>
        /// The fields.
        /// </summary>
        private readonly List<KeyValuePair<string, Dictionary<string, object>>> fields;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAgent"/> class.
        /// Following fields are predefined:
        /// Library, OS, Language and Webserver.
        /// </summary>
        public UserAgent()
        {
            fields = new List<KeyValuePair<string, Dictionary<string, object>>>();
            var iisVersionProvider = new IISVersionProvider();

            AddField("Library", "Klarna.ApiWrapper", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            var os = Environment.OSVersion;
            AddField("OS", os.Platform.ToString(), os.Version.ToString());
            AddField("Language", ".Net", Environment.Version.ToString());
            AddField("Webserver", "IIS", iisVersionProvider.FindIISVersion(os.Platform));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a field to the field collection.
        /// </summary>
        /// <param name="field">
        /// The field name.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if field already exists.
        /// </exception>
        public void AddField(string field, string name, string version)
        {
            AddField(field, name, version, new string[0]);
        }

        /// <summary>
        /// Adds a field to the field collection.
        /// </summary>
        /// <param name="field">
        /// The field name.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if field already exists.
        /// </exception>
        public void AddField(string field, string name, string version, string[] options)
        {
            if (fields.Exists(f => f.Key == field))
            {
                throw new ArgumentException("Field already exists.", field);
            }

            var entries = new Dictionary<string, object> { { "Name", name }, { "Version", version } };
            if (options.Length > 0)
            {
                entries.Add("Options", options);
            }

            fields.Add(new KeyValuePair<string, Dictionary<string, object>>(field, entries));
        }

        /// <summary>
        /// Returns the user agent string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var field in fields)
            {
                var optionsString = new StringBuilder();
                if (field.Value.ContainsKey("Options"))
                {
                    optionsString.Append("(");
                    var options = (string[])field.Value["Options"];
                    var firstOption = true;
                    foreach (var option in options)
                    {
                        if (!firstOption)
                        {
                            optionsString.Append(" ; ");
                        }

                        optionsString.Append(option);
                        firstOption = false;
                    }

                    optionsString.Append(")");
                }

                builder.AppendFormat("{0}/{1}_{2} {3}", field.Key, field.Value["Name"], field.Value["Version"], optionsString);
            }

            return builder.ToString().TrimEnd(' ');
        }

        #endregion
    }
}