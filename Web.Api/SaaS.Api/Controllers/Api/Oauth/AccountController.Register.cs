using AutoMapper;
using Microsoft.Owin.Security;
using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Oauth;
using SaaS.Api.Oauth;
using SaaS.Api.Oauth.Providers;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.Oauth;
using SaaS.Data.Entities.View;
using SaaS.IPDetect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AccountController
    {
        private async Task<IHttpActionResult> _register(RegisterViewModel model, Account account, ViewAccountDetails accountDetails, string stackTrace)
        {
            if (!object.Equals(account, null))
            {
                await _auth.AccountVisitorIdSetAsync(account, model.VisitorId);

                if (model.IsBusiness())
                {
                    await _auth.AccountMaskAsBusinessAsync(account);

                    //make sure that user will get b2b 30 trial
                    await _auth.AccountActivateAsync(account);
                }

                if (account.IsBusiness && string.Equals(model.Source, "sodapdf.com-get-trial", StringComparison.InvariantCultureIgnoreCase))
                    await NotificationManager.BusinessDownload(account);

                if (account.IsEmptyPassword())
                {
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        await _auth.AccountPasswordSetAsync(account, model.Password);
                        await NotificationManager.AccountCreationComplete(account);
                    }
                    accountDetails.GeoIp = IpAddressDetector.IpAddress;
                    accountDetails.Id = account.Id;
                    await _auth.AccountDetailsSetAsync(accountDetails);

                    return Ok();
                }

                return AccountExists();
            }
            account = new Account(model.Email, model.FirstName, model.LastName);
            account.IsBusiness = model.IsBusiness();

            var result = await _auth.AccountCreateAsync(account, model.Password);
            var errorResult = GetErrorResult(result);

            if (!object.Equals(errorResult, null))
                return errorResult;

            accountDetails.GeoIp = IpAddressDetector.IpAddress;
            accountDetails.Id = account.Id;
            accountDetails.Source = model.Source;
            accountDetails.WebForm = FormIdBuilder.Build(model.FormId);

            await _auth.AccountOptinSetAsync(account, model.Optin);
            await _auth.AccountDetailsSetAsync(accountDetails);
            await _auth.AccountVisitorIdSetAsync(account, model.VisitorId);

            OauthLogger.CreateAccountWarn(Request, account, accountDetails, stackTrace);

            if ("sodapdf.com-esign-lite".Equals(model.Source, StringComparison.InvariantCultureIgnoreCase))
                return Ok(); //

            if (account.IsBusiness && "sodapdf.com-get-trial".Equals(model.Source, StringComparison.InvariantCultureIgnoreCase))
                await NotificationManager.BusinessDownloadNewAccount(account);
            else
            {
                if (!string.IsNullOrEmpty(model.Password))
                {
#if PdfForge
                    if (!object.Equals(accountDetails, null) && "covermount".Equals(accountDetails.Build, StringComparison.InvariantCultureIgnoreCase))
                    {
                        await NotificationManager.EmailConfirmationCovermount(account);
                        return Ok();
                    }
#endif

                    await NotificationManager.EmailConfirmation(account);
                }
                else
                {
                    if ("sodapdf.com-opdfs".Equals(model.Source, StringComparison.InvariantCultureIgnoreCase) ||
                        "sodapdf.com-opdfs-send-to-email".Equals(model.Source, StringComparison.InvariantCultureIgnoreCase))
                        await NotificationManager.MicrotransactionCreatePassword(account);
                }
            }

            return Ok();
        }

        private async Task<IHttpActionResult> _registerLegal(RegisterViewModel model, Account account, ViewAccountDetails accountDetails)
        {
            if (!object.Equals(account, null))
            {
                await _auth.AccountVisitorIdSetAsync(account, model.VisitorId);

                if (account.IsActivated)
                    await NotificationManager.LegacyActivationSignInNotification(account);
                else
                    await NotificationManager.LegacyCreatePasswordReminder(account);

                return AccountExists();
            }

            account = new Account(model.Email, model.FirstName, model.LastName);
            account.IsBusiness = model.IsBusiness();

            var result = await _auth.AccountCreateAsync(account, model.Password);
            var errorResult = GetErrorResult(result);

            if (!object.Equals(errorResult, null))
                return errorResult;

            accountDetails.GeoIp = IpAddressDetector.IpAddress;
            accountDetails.Id = account.Id;
            accountDetails.Source = model.Source;
            accountDetails.WebForm = FormIdBuilder.Build(model.FormId);

            await _auth.AccountOptinSetAsync(account, model.Optin);
            await _auth.AccountDetailsSetAsync(accountDetails);
            await _auth.AccountVisitorIdSetAsync(account, model.VisitorId);

            await NotificationManager.LegacyActivationCreatePassword(account);

            OauthLogger.CreateAccountWarn(Request, account, accountDetails, "registerLegal");

            return Ok();
        }


        private void _b2blead(RegisterViewModel model)
        {
            if (object.Equals(model, null) || !_b2bleadSourceCollection.Contains(model.Source))
                return;

            try
            {
                using (MailMessage message = new MailMessage())
                {
                    message.To.Add(_appSettings.B2BLead.Email);
                    message.Subject = string.Format("{0} - {1} {2}", model.Source, model.FirstName, model.LastName);

                    message.SubjectEncoding = Encoding.UTF8;

                    StringBuilder body = new StringBuilder();

                    _b2bleadBuilder(body, "Email", model.Email);
                    body.AppendFormat("Name: {0} {1}", model.FirstName, model.LastName); body.AppendLine();

                    _b2bleadBuilder(body, "Company", model.Company);
                    _b2bleadBuilder(body, "Occupation", model.Occupation);
                    _b2bleadBuilder(body, "Phone", model.Phone);
                    _b2bleadBuilder(body, "Language", model.LanguageISO2);
                    _b2bleadBuilder(body, "Country", model.CountryISO2);

                    _b2bleadBuilder(body, "State", model.State);
                    _b2bleadBuilder(body, "City", model.City);
                    _b2bleadBuilder(body, "Address1", model.Address1);
                    _b2bleadBuilder(body, "Address2", model.Address2);
                    _b2bleadBuilder(body, "PostalCode", model.PostalCode);

                    _b2bleadBuilder(body, "Industry", model.Industry);
                    _b2bleadBuilder(body, "JobRole", model.JobRole);
                    _b2bleadBuilder(body, "Licenses", model.Licenses);
                    _b2bleadBuilder(body, "Product", model.Product);

                    message.Body = body.ToString();
                    message.BodyEncoding = Encoding.UTF8;

                    using (var client = new SmtpClient())
                        client.Send(message);
                }
            }
            catch { }
        }

        private void _b2bleadBuilder(StringBuilder builder, string key, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            builder.AppendFormat("{0}: {1}", key, value);
            builder.AppendLine();
        }


        [HttpPost, Route("register"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> Register(RegisterViewModel model)
        {
            try
            {
                var accountsDetail = new ViewAccountDetails();
                accountsDetail = Mapper.Map(model, accountsDetail);

                var acceptLanguage = Request.Headers.AcceptLanguage.FirstOrDefault();
                if (!object.Equals(acceptLanguage, null) && !string.IsNullOrEmpty(acceptLanguage.Value))
                    accountsDetail.LanguageISO2 = acceptLanguage.Value.Substring(0, 2).ToLower();

                _b2blead(model);

                if (!object.Equals(model.Params, null))
                {
                    accountsDetail.Cmp = model.Params.Cmp;
                    accountsDetail.Partner = model.Params.Partner;
                    accountsDetail.Build = model.Params.Build;
                    accountsDetail.Uid = model.Params.GetUid();
                }

                var account = await _auth.AccountGetAsync(model.Email);

                if (object.Equals(account, null))
                {
                    account = await _auth.AccountGetAsync(model.Email, isIncludeSubEmails: true);
                    if (!object.Equals(account, null))
                        return AccountExists();
                }
               else if (account.IsEmptyPassword())
                {
                    var sessionTokensExternalHistory = await _auth.SessionTokenExternalHistoriesAsync(account.Id);
                    var sessionTokenExternalHistory = sessionTokensExternalHistory.FirstOrDefault(e => !e.IsUnlinked);
                    if (!object.Equals(sessionTokenExternalHistory, null))
                        return ErrorContent("invalid_grant", string.Format("You should sign in with {0}.", sessionTokenExternalHistory.ExternalClientName),System.Net.HttpStatusCode.Conflict);
                        
                }

                if ("soda pdf 8".Equals(model.Source, StringComparison.InvariantCultureIgnoreCase))
                    return await _registerLegal(model, account, accountsDetail);

                return await _register(model, account, accountsDetail, "register");
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        [HttpPost, Route("register-anonymous"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> RegisterAnonymous(RegisterAnonymousViewModel model, Guid? visitorId = null)
        {
            try
            {
                var account = await _auth.AccountAnonymousCreateAsync(model.Password);

                if (object.Equals(account, null))
                    return AccountNotFound();

                var client = GetClient(model.client_id);
                if (object.Equals(client, null))
                    return ClientNotFound();

                var identity = new ClaimsIdentity(Startup.OAuthBearerOptions.AuthenticationType);
                var session = Guid.NewGuid();
                var properties = new Dictionary<string, string>
                    {
                        { "session",  session.ToString("N") }
                    };

                identity.AddClaim(ClaimTypes.NameIdentifier, account.Id);
                identity.AddClaim(ClaimTypes.Name, account.Email);
                identity.AddClaims(account, client);

                var ticket = new AuthenticationTicket(identity, new AuthenticationProperties(properties));

                var issued = DateTime.UtcNow;
                var expires = issued.Add(Startup.OAuthServerOptions.AccessTokenExpireTimeSpan);

                var token = new SessionToken
                {
                    Id = session,
                    AccountId = account.Id,
                    ClientId = client.Id,
                    ClientVersion = model.client_version,
                    IssuedUtc = issued,
                    ExpiresUtc = expires
                };

                ticket.Properties.IssuedUtc = issued;
                ticket.Properties.ExpiresUtc = expires;

                token.ProtectedTicket = Startup.OAuthServerOptions.AccessTokenFormat.Protect(ticket);

                var scopeWorker = SaaSScopeWorkerFactory.Create("webeditor", _auth, _authProduct);

                await scopeWorker.SessionTokenInsertAsync(token, null);
                await _auth.AccountVisitorIdSetAsync(account, visitorId);

                return Ok(new
                {
                    access_token = token.ProtectedTicket,
                    token_type = ticket.Identity.AuthenticationType,
                    expires_in = (int)(ticket.Properties.ExpiresUtc - ticket.Properties.IssuedUtc).Value.TotalSeconds,
                    refresh_token = token.Id.ToString("N"),
                    issued = ticket.Properties.IssuedUtc,
                    expires = ticket.Properties.ExpiresUtc,
                    email = account.Email,
                    firstName = account.FirstName,
                    lastName = account.LastName,
                    status = account.GetStatus()
                });
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }
    }
}