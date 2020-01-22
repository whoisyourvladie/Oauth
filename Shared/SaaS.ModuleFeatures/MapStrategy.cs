using SaaS.ModuleFeatures.Model;
using System;
using System.Collections.Generic;

namespace SaaS.ModuleFeatures
{
    public enum MapStrategy { Subscription, PerpetualLicense }

    internal abstract class MapStrategyBase
    {
        protected void Normalize(ref Version license)
        {
            license = license ?? new Version();
            license = new Version(license.Major < 0 ? 0 : license.Major,
                                  license.Minor < 0 ? 0 : license.Minor,
                                  license.Build < 0 ? 0 : license.Build,
                                  license.Revision < 0 ? 0 : license.Revision);
        }

        internal abstract MapModule[] GetModules(Root root, Version license);
    }
    internal class MapStrategySubscription : MapStrategyBase
    {
        internal override MapModule[] GetModules(Root root, Version license)
        {
            Normalize(ref license);

            var mapModules = new List<MapModule>(root.Modules.Length);
            foreach (var module in root.Modules)
            {
                var features = new List<MapFeature>(module.Features.Length);

                foreach (var feature in module.Features)
                    features.Add(new MapFeature(feature.Name, feature.GetVersion()));

                mapModules.Add(new MapModule(module.Name, features.ToArray()));
            }

            return mapModules.ToArray();
        }
    }
    internal class MapStrategyPerpetualLicense : MapStrategyBase
    {
        internal override MapModule[] GetModules(Root root, Version license)
        {
            if (object.Equals(root.Modules, null))
                return null;

            Normalize(ref license);

            var latestVersion = root.GetLatestVersion();
            var mapModules = new List<MapModule>(root.Modules.Length);
            foreach (var module in root.Modules)
            {
                if (object.Equals(module.Features, null))
                    continue;

                var features = new List<MapFeature>(module.Features.Length);

                foreach (var feature in module.Features)
                {
                    var featureVersion = feature.GetVersion();
                    var mapFeature = new MapFeature(feature.Name, featureVersion);

                    featureVersion = featureVersion ?? new Version();
                    mapFeature.IsHidden = featureVersion.Major >= license.Major &&
                                          featureVersion.Minor > license.Minor &&
                                          featureVersion.Major == latestVersion.Major;

                    if (!mapFeature.IsHidden)
                        mapFeature.IsUpgradable = featureVersion > license;

                    features.Add(mapFeature);
                }

                mapModules.Add(new MapModule(module.Name, features.ToArray()));
            }

            return mapModules.ToArray();
        }
    }

    internal abstract class MapStrategyCreator
    {
        internal abstract MapStrategyBase Factory();

        internal static MapStrategyCreator Creator(MapStrategy strategy)
        {
            switch (strategy)
            {
                case MapStrategy.PerpetualLicense:
                    return new MapStrategyPerpetualLicenseCreator();

                default:
                    return new MapStrategySubscriptionCreator();
            }
        }
    }
    internal class MapStrategySubscriptionCreator : MapStrategyCreator
    {
        internal override MapStrategyBase Factory()
        {
            return new MapStrategySubscription { };
        }
    }
    internal class MapStrategyPerpetualLicenseCreator : MapStrategyCreator
    {
        internal override MapStrategyBase Factory()
        {
            return new MapStrategyPerpetualLicense { };
        }
    }
}
