using System;

namespace SaaS.Data.Entities.View.Accounts
{
    public class ViewAccountEmail : Entity<Guid>
    {
        public string Email { get; set; }
    }
}
