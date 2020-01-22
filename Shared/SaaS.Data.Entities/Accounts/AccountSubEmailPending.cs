using System;

namespace SaaS.Data.Entities.Accounts
{
    public class AccountSubEmailPending : AccountEntity<Guid>
    {
        public string Email { get; set; }

        public DateTime CreateDate { get; set; }
    }
}