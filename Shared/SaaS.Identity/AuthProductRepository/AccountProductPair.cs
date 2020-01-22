using System;

namespace SaaS.Identity
{
    public struct AccountProductPair
    {
        public Guid AccountId;
        public Guid AccountProductId;

        public AccountProductPair(Guid accountId, Guid accountProductId)
        {
            AccountId = accountId;
            AccountProductId = accountProductId;
        }
    }
}
