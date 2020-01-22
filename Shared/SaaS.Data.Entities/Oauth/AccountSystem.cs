using System;

namespace SaaS.Data.Entities.Oauth
{
    public class AccountSystem : AccountEntity<Guid>
    {
        public Guid SystemId { get; set; }
        public Guid AccountProductId { get; set; }
    }
}
