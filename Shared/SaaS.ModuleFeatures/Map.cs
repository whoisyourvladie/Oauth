using Newtonsoft.Json;
using SaaS.ModuleFeatures.Model;
using System;
using System.IO;

namespace SaaS.ModuleFeatures
{
    public class Map
    {
        private Root _root;

        internal Map(Root root)
        {
            _root = root;
        }

        public static Map Load(string path)
        {
            var json = File.ReadAllText(path);
            var root = JsonConvert.DeserializeObject<Root>(json);

            return new Map(root);
        }

        public MapModule[] GetModules(MapStrategy strategy, Version license)
        {
            var creator = MapStrategyCreator.Creator(strategy);
            var searchMapStrategy = creator.Factory();

            return searchMapStrategy.GetModules(_root, license);
        }
    }
}