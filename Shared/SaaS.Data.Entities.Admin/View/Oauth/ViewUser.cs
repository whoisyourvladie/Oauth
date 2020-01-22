using System;

namespace SaaS.Data.Entities.Admin.View.Oauth
{
    public class ViewUser : Entity<Guid>
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }
    }
}