using Microsoft.Owin.Security.OAuth;
using SaaS.Data.Entities.Accounts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Api.Oauth.Providers
{
    public partial class SaaSAuthorizationServerProvider
    {
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                switch (property.Key)
                {
                    case ".issued": context.AdditionalResponseParameters.Add("issued", context.Properties.IssuedUtc.Value.UtcDateTime); continue;
                    case ".expires": context.AdditionalResponseParameters.Add("expires", context.Properties.ExpiresUtc.Value.UtcDateTime); continue;

                    case "session":
                    case "token": continue;
                }

                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            var user = context.OwinContext.Get<Account>("user");
            if (!object.Equals(user, null))
            {
                context.AdditionalResponseParameters.Add("email", user.Email);
                context.AdditionalResponseParameters.Add("firstName", user.FirstName ?? string.Empty);
                context.AdditionalResponseParameters.Add("lastName", user.LastName ?? string.Empty);
                context.AdditionalResponseParameters.Add("status", user.GetStatus());
            }

            return Task.FromResult<object>(null);
        }
    }
}