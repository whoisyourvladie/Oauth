using System;

namespace SaaS.Data.Entities.View.Accounts
{
    public class ViewAccountMergePending : Entity<Guid?>
    {
        public Guid AccountIdTo { get; set; }
        public Guid AccountIdFrom { get; set; }
        public Guid AccountIdPrimaryEmail { get; set; }
        public string AccountEmailTo { get; set; }
        public string AccountEmailFrom { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}