using System;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Data.Entities.Admin.Oauth
{
    public class SessionToken: Entity<Guid>
    {
        public DateTime IssuedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }

        [Required]
        public string ProtectedTicket { get; set; }
    }
}
