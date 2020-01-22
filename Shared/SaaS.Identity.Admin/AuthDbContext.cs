using System.Data.Entity;

namespace SaaS.Identity.Admin
{
    public partial class AuthDbContext : DbContext
    {
        public AuthDbContext()
            : base("oauthAdmin")
        {
            Database.SetInitializer<AuthDbContext>(null);
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public AuthDbContext(string connectionString)
            : base(connectionString)
        {
            Database.SetInitializer<AuthDbContext>(null);
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("oauth.user");
        }

        public DbSet<User> Users { get; set; }
    }
}