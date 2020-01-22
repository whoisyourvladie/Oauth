using System;

namespace SaaS.Data.Entities
{
    public class AccountEntity<T> : Entity<T>
    {
        public Guid AccountId { get; set; }
    }
}