using System;

namespace SaaS.ModuleFeatures.Model
{
    internal class Root
    {
        public string LatestVersion { get; set; }

        public Module[] Modules { get; set; }

        public Version GetLatestVersion()
        {
            return System.Version.Parse(LatestVersion);
        }
    }
}