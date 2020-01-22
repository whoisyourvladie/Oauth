using System.Security.Claims;

namespace SaaS.Api.Client
{
    public class SaaSApiPrinciple : ClaimsPrincipal
    {
        public SaaSApiPrinciple(SaaSApiIdentity identity)
            : base(identity)
        {
            
        }

        public SaaSApiPrinciple(ClaimsPrincipal claimsPrincipal)
            : base(claimsPrincipal)
        {

        }
    }
}
