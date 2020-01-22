using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AccountController
    {
        [HttpGet, Route("payment-information"), SaaSAuthorize]
        public async Task<IHttpActionResult> PaymentInformation()
        {
            try
            {
                var creditCards = await UpclickClient.SubscriptionsPaymentInstruments(User.Identity.Name);

                if (!object.Equals(creditCards, null))
                {
                    foreach (var creditCard in creditCards)
                    {
                        string token = UpclickClient.Token("paymentinstrument.edit");
                        Uri uri = new Uri(string.Format("https://billing.upclick.com/{0}/PaymentInstrument/Edit", token));
                        var query = HttpUtility.ParseQueryString(string.Empty);
                        query.Add("id", HttpUtility.UrlDecode(creditCard.PayTokenID));

                        var uriBuilder = new UriBuilder(uri);
                        uriBuilder.Query = query.ToString();

                        creditCard.EditUrl = uriBuilder.Uri.ToString();
                    }
                }

                return Ok(creditCards);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }
    }
}

//var identity = new ClaimsIdentity(User.Identity);
//var properties = new Dictionary<string, string>();

//var ticket = new AuthenticationTicket(identity, new AuthenticationProperties(properties));

////authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(Identity), new AuthenticationProperties { IsPersistent = true });
////var authenticateResult = await authenticationManager.AuthenticateAsync(Startup.OAuthBearerOptions.AuthenticationType);

//var propertedToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);