using Newtonsoft.Json;
using System;

namespace SaaS.ModuleFeatures
{
    public class MapFeature
    {
        public MapFeature(string name, Version version)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; set; }
        public Version Version { get; set; }

        [JsonIgnore]
        public bool IsHidden { get; set; }

        [JsonIgnore]
        public bool IsUpgradable { get; set; }

        [JsonIgnore]
        public bool IsActivated { get; set; }

        public ulong Status
        {
            get
            {
                ulong status = 0;

                status |= (ulong)(IsHidden ? MapFeatureStatus.IsHidden : 0);
                status |= (ulong)(IsUpgradable ? MapFeatureStatus.IsUpgradable : 0);
                status |= (ulong)(IsActivated ? MapFeatureStatus.IsActivated : 0);

                return status;
            }
        }

        [Flags]
        public enum MapFeatureStatus
        {
            None = 0,
            IsHidden = 1,
            IsUpgradable = 2,
            IsActivated = 4
        }

        internal void Activate()
        {
            if (IsHidden || IsUpgradable)
                return;

            IsActivated = true;
        }
    }
}
