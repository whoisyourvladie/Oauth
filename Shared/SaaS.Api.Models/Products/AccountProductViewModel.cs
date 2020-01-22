using System;

namespace SaaS.Api.Models.Products
{
    public class AccountProductViewModel : ProductViewModel
    {
        public ulong? ExpiresIn
        {
            get
            {
                if (!EndDate.HasValue)
                    return null;

                if (EndDate.Value <= DateTime.UtcNow)
                    return 0;

                return (ulong)(EndDate.Value - DateTime.UtcNow).TotalSeconds;
            }
        }
    }
}
