using SaaS.Data.Entities.Accounts;
using System.Data.Entity;

namespace SaaS.Identity
{
    public partial class AuthDbContext : DbContext
    {
        public AuthDbContext()
            : base("oauth")
        {
            Database.SetInitializer<AuthDbContext>(null);
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().ToTable("accounts.account");
        }

        public DbSet<Account> Accounts { get; set; }
    }
}