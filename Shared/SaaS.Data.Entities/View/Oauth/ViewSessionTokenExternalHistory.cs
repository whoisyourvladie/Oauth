using System;

namespace SaaS.Data.Entities.View.Oauth
{
    public class ViewSessionTokenExternalHistory : Entity<Guid>
    {
        public string ExternalClientName { get; set; }
        public string ExternalAccountId { get; set; }
        public string Email { get; set; }
        public bool IsUnlinked { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}