using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using SaaS.Common.Extensions;
using SaaS.Data.Entities;
using SaaS.Data.Entities.Oauth;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace SaaS.Api.Oauth.Providers
{
    public partial class SaaSAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId, clientSecret;
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
                context.TryGetFormCredentials(out clientId, out clientSecret);

            var client = ValidateClientAuthentication(context, clientSecret);
            if (object.Equals(client, null))
                return;

            string[] scope = null;
            SystemSignInData systemSignInData = null;
            string token = null;

            var formCollection = await context.Request.ReadFormAsync();
            if (!object.Equals(formCollection, null))
            {
                scope = GetScope(formCollection);
                systemSignInData = GetSystemSignInData(formCollection);
                token = formCollection["token"];

                Guid visitorId;
                if (Guid.TryParse(formCollection["visitorId"], out visitorId))
                    context.OwinContext.Set("visitorId", visitorId);

                var externalClient = OauthManager.GetExternalClient(formCollection["externalClient"]);
                if (externalClient.HasValue)
                    context.OwinContext.Set("externalClient", externalClient);

                Version clientVersion;
                if ((Version.TryParse(formCollection["client_version"], out clientVersion) && clientVersion.Major > 0) ||
                    Version.TryParse(client.Version, out clientVersion))
                    context.OwinContext.Set("clientVersion", clientVersion.ToString());

                if (Guid.TryParse(formCollection["installationID"], out Guid installationID))
                    context.OwinContext.Set("installationID", installationID);
            }

            context.OwinContext.Set("scope", scope);
            context.OwinContext.Set("systemSignInData", systemSignInData);
            context.OwinContext.Set("client", client);
            context.OwinContext.Set("token", token);

            context.Validated();
        }

        private Client ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context, string clientSecret)
        {
            if (object.Equals(context.ClientId, null))
            {
                context.SetError("invalid_clientId", "ClientId should be sent.");
                return null;
            }

            var client = GetClient(context.ClientId);
            if (object.Equals(client, null))
            {
                context.SetError("invalid_clientId", "This client is not registered in the system.");
                return null;
            }

            if (!client.IsActive)
            {
                context.SetError("invalid_client", "Client is inactive.");
                return null;
            }

            if (client.ApplicationType == ApplicationTypes.Desktop)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientSecret", "Client secret should be sent.");
                    return null;
                }

                if (client.Secret != clientSecret.GetHash())
                {
                    context.SetError("invalid_clientSecret", "Client secret is invalid.");
                    return null;
                }
            }

            return client;
        }

        private string[] GetScope(IFormCollection formCollection)
        {
            var _scope = formCollection["scope"];
            if (!string.IsNullOrEmpty(_scope))
                return _scope.ToLower().Split(',');

            return null;
        }

        private SubscriptionSignInData GetSignInData(IFormCollection formCollection)
        {
            int accountProductId;
            bool setAsDefault;

            bool.TryParse(formCollection["setAsDefault"], out setAsDefault);
            if (int.TryParse(formCollection["accountProductId"], out accountProductId))
                return new SubscriptionSignInData { AccountProductId = accountProductId, SetAsDefault = setAsDefault };

            return new SubscriptionSignInData();
        }

        private SystemSignInData GetSystemSignInData(IFormCollection formCollection)
        {
            var systemSignInData = new SystemSignInData
            {
                MotherboardKey = formCollection["motherboardKey"],
                PhysicalMac = formCollection["physicalMac"],
                PcName = formCollection["pcName"] ?? null
            };

            Guid machineKey;
            if (Guid.TryParse(formCollection["machineKey"], out machineKey))
                systemSignInData.MachineKey = machineKey;

            bool isAutogeneratedMachineKey;
            bool.TryParse(formCollection["isAutogeneratedMachineKey"], out isAutogeneratedMachineKey);
            systemSignInData.IsAutogeneratedMachineKey = isAutogeneratedMachineKey;

            return systemSignInData;
        }

        private static Client GetClient(string name)
        {
            Client client = null;

            if (!string.IsNullOrEmpty(name))
                _clients.TryGetValue(name, out client);

            return client;
        }

        //private static ExternalClient GetExternalClient(string name)
        //{
        //    ExternalClient client = null;

        //    if (!string.IsNullOrEmpty(name))
        //        _externalClients.TryGetValue(name, out client);

        //    return client;
        //}
    }
}