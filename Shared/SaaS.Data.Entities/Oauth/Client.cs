namespace SaaS.Data.Entities.Oauth
{
    public class Client : Entity<int>
    {
        public string Secret { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public ApplicationTypes ApplicationType { get; set; }
        public bool IsActive { get; set; }
        public int RefreshTokenLifeTime { get; set; }
    }
}