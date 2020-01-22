namespace SaaS.ModuleFeatures
{
    public class MapModule
    {
        public MapModule(string name, MapFeature[] features)
        {
            Name = name;
            Features = features;
        }

        public string Name { get; private set; }
        public MapFeature[] Features { get; private set; }

        public void ActivateFeatures()
        {
            if (object.Equals(Features, null))
                return;

            foreach (var feature in Features)
                feature.Activate();
        }
    }
}
