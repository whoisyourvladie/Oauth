using System;

namespace SaaS.Data.Entities.Accounts
{
    public class AccountSubEmail : AccountEntity<int>
    {
        public string Email { get; set; }

        public DateTime CreateDate { get; set; }
    }
}