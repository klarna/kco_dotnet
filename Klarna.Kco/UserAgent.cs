#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="UserAgent.cs" company="Klarna AB">
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
        private readonly List<KeyValuePair<string, Dictionary<string, string>>> fields;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAgent"/> class.
        /// </summary>
        public UserAgent()
        {
            fields = new List<KeyValuePair<string, Dictionary<string, string>>>();

            AddField("Library", "Klarna.ApiWrapper", "1.0");
            var os = Environment.OSVersion;
            AddField("OS", os.Platform.ToString(), os.Version.ToString());
            AddField("Language", ".Net", Environment.Version.ToString());
            AddField("Webserver", "IIS", this.IisVersion());
        }

        #endregion

        #region Methods

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
                builder.AppendFormat("{0}/{1}_{2} ", field.Key, field.Value["Name"], field.Value["Version"]);
            }

            return builder.ToString().TrimEnd(' ');
        }

        #endregion

        #region Private Methods

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
        private void AddField(string field, string name, string version)
        {
            fields.Add(
                new KeyValuePair<string, Dictionary<string, string>>(
                    field, new Dictionary<string, string>() { { "Name", name }, { "Version", version } }));
        }

        /// <summary>
        /// Gets IIS version.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string IisVersion()
        {
            var iisVersion = "Unknown_0.0";
            using (var iisKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\InetStp\"))
            {
                if (iisKey != null)
                {
                    var versionString = iisKey.GetValue("VersionString") as string;
                    if (versionString != null)
                    {
                        var splitVersionString = versionString.Split(' ');
                        if (splitVersionString.Length > 1)
                        {
                            iisVersion = splitVersionString[1];
                        }
                    }
                }
            }

            return iisVersion;
        }

        #endregion
    }
}
