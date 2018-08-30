namespace Klarna.Checkout
{
    using Microsoft.Win32;
    using System;
    using System.Linq;

    public class IISVersionProvider
    {
        public const string DefaultIISVersion = "Unknown_0.0";

        private readonly PlatformID[] windowsPlatforms = new PlatformID[]
        {
            PlatformID.Win32NT,
            PlatformID.Win32S,
            PlatformID.Win32Windows,
            PlatformID.WinCE,
        };

        /// <summary>
        /// Gets IIS version from registry.
        /// </summary>
        /// <param name="os">Operating system information on which program is being run</param>
        /// <returns>Returns string containing IIS version or Unknown_0.0 if it's not being run on any Windows OS</returns>
        public string FindIISVersion(PlatformID platform)
        {
            if (windowsPlatforms.Contains(platform))
            {
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
                                return splitVersionString[1];
                            }
                        }
                    }
                }
            }

            return DefaultIISVersion;
        }
    }
}