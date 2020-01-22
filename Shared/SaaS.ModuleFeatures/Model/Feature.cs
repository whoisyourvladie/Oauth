using System;

namespace SaaS.ModuleFeatures.Model
{
    internal class Feature
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public Version GetVersion()
        {
            Version version;
            if (!System.Version.TryParse(Version, out version))
                return null;

            return version;
        }
    }
}