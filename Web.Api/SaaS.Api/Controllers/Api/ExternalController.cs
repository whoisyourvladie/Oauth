using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Oauth;
using SaaS.Api.Oauth;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.Oauth;
using SaaS.Data.Entities.View;
using SaaS.IPDetect;
using SaaS.Oauth2.Core;
using SaaS.Oauth2.Services;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace SaaS.Api.Controllers.Api
{
    [RoutePrefix("api/external")]
    public class ExternalController : SaaSApiController
    {
        //https://www.facebook.com/settings?tab=applications
        //https://myaccount.google.com/permissions

        private static ConcurrentDictionary<Guid, ExternalToken> _externaLoginStorage = new ConcurrentDictionary<Guid, ExternalToken>();

        static ExternalController()
        {
            CancellationTokenSource cancelationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var dateTime = DateTime.Now.AddMinutes(-20);
                    var oldTokens = _externaLoginStorage.Values.Where(e => e.CreateDate < dateTime).ToList();
                    foreach (var oldToken in oldTokens)
                    {
                        ExternalToken token;
                        _externaLoginStorage.TryRemove(oldToken.State, out token);
                    }

                    Task.Delay(TimeSpan.FromMinutes(1), cancelationTokenSource.Token).Wait();
                }

            }, cancelationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private Task<ExternalToken> GetExternalToken(Guid state, CancellationToken cancellationToken)
        {
            var task = Task.Run(async delegate
            {
                ExternalToken externalToken = null;

                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    if (object.Equals(externalToken, null))
                        _externaLoginStorage.TryGetValue(state, out externalToken);

                    if (!object.Equals(externalToken, null) && !object.Equals(externalToken.Token, null))
                        return externalToken;

                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                }

                return externalToken;

            }, cancellationToken);

            task.Wait(TimeSpan.FromSeconds(10));

            return task;
        }

        [HttpPost, Route("token")]
        public async Task<IHttpActionResult> Token(SignInViewModel internalSignInViewModel, CancellationToken cancellationToken)
        {
            var task = GetExternalToken(internalSignInViewModel.State, cancellationToken);

            if (task.Status == TaskStatus.WaitingForActivation || object.Equals(task.Result, null) || object.Equals(task.Result.Token, null))
                return ResponseMessage(new HttpResponseMessage(HttpStatusCode.PartialContent));

            var token = task.Result;
            internalSignInViewModel.ExternalClient = token.Provider;

            var service = OauthServiceFactory.CreateService(token.Provider);
            var profile = await service.ProfileAsync(token.Token, cancellationToken);

            var account = await _auth.AccountGetAsync(profile.Email, isIncludeSubEmails: true);

            if (!object.Equals(account, null))
            {
                var sessionTokens = await _auth.SessionTokenExternalHistoriesAsync(account.Id);
                if (sessionTokens.Any(e => e.IsUnlinked &&
                                     string.Equals(token.Provider, e.ExternalClientName, StringComparison.InvariantCultureIgnoreCase) &&
                                     string.Equals(profile.Id, e.ExternalAccountId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return AccountIsDisconnected(token.Provider);
                }
            }

            if (object.Equals(account, null))
            {
                account = new Account(profile.Email, profile.FirstName, profile.LastName);

                var result = await _auth.AccountCreateAsync(account);
                var errorResult = GetErrorResult(result);

                if (!object.Equals(errorResult, null))
                    return errorResult;

                var accountsDetail = new ViewAccountDetails();
                accountsDetail.GeoIp = IpAddressDetector.IpAddress;
                accountsDetail.Id = account.Id;
                accountsDetail.Uid = token.Uid;
                accountsDetail.Cmp = token.Cmp;
                accountsDetail.Source = token.Source;
                accountsDetail.WebForm = token.FormId;
                accountsDetail.Build = token.Build;
                accountsDetail.Partner = token.Partner;
                accountsDetail.IsTrial = token.Trial;

                await _auth.AccountDetailsSetAsync(accountsDetail);
                await NotificationManager.Welcome(account);
            }

            await _auth.AccountOptinSetAsync(account, token.Optin);
            await _auth.AccountActivateAsync(account);

            internalSignInViewModel.SetUser(account);

            await _auth.SessionTokenExternalHistorySetAsync(account.Id, token.Provider, profile.Id, profile.Email);

            return ResponseMessage(await OauthManager.InternalSignIn(internalSignInViewModel, token.VisitorId));
        }

        [HttpPost, Route("connect-account"), SaaSAuthorize]
        public async Task<IHttpActionResult> ConnectAccount([FromUri] Guid state, CancellationToken cancellationToken)
        {
            try
            {
                var task = GetExternalToken(state, cancellationToken);

                if (task.Status == TaskStatus.WaitingForActivation || object.Equals(task.Result, null) || object.Equals(task.Result.Token, null))
                    return ResponseMessage(new HttpResponseMessage(HttpStatusCode.PartialContent));

                var token = task.Result;

                var service = OauthServiceFactory.CreateService(token.Provider);
                var profile = await service.ProfileAsync(token.Token, cancellationToken);

                var user = await _auth.AccountGetAsync(profile.Email, isIncludeSubEmails: true);
                if (!object.Equals(user, null) && user.Id != AccountId)
                    return AccountExists();
                else
                    await _auth.SessionTokenExternalHistoryConnectAccountAsync(AccountId, token.Provider, profile.Id, profile.Email);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }

        [HttpGet, Route("config")]
        [CacheOutput(ClientTimeSpan = 86400, ServerTimeSpan = 86400)]
        public IHttpActionResult Config()
        {
            var google = OauthServiceFactory.CreateService("google");
            var facebook = OauthServiceFactory.CreateService("facebook");
            var microsoft = OauthServiceFactory.CreateService("microsoft");

            return Ok(new ExternalLoginSettingsViewModel(
                new ExternalLoginSettings
                {
                    Window = new ExternalWindowSettings(google.GetWindowSize())
                },
                new ExternalLoginSettings
                {
                    Window = new ExternalWindowSettings(facebook.GetWindowSize())
                },
                new ExternalLoginSettings
                {
                    Window = new ExternalWindowSettings(microsoft.GetWindowSize())
                }));
        }

        [HttpGet, Route("login/{externalClient:regex(^google|facebook|microsoft$)}")]
        public IHttpActionResult ExternaLogin(Guid state, ExternalClient externalClient, string lang = "en",
            string source = null,
            string formId = null,
            string build = null,
            string partner = null,
            Guid? visitorId = null,
            int? uid = null, string cmp = null,
            bool? optin = null,
            bool? trial = null)
        {
            var service = OauthServiceFactory.CreateService(OauthManager.GetExternalClientName(externalClient));

            if (object.Equals(service, null))
                return NotFound();

            var url = new Uri(service.GetAuthenticationUrl(lang));
            var query = HttpUtility.ParseQueryString(url.Query);
            query.Add("state", state.ToString("N"));

            var uriBuilder = new UriBuilder(url);
            uriBuilder.Query = query.ToString();

            var externalToken = new ExternalToken(state, OauthManager.GetExternalClientName(externalClient))
            {
                VisitorId = visitorId,
                Uid = uid,
                Cmp = cmp,
                Optin = optin,
                Build = build,
                Partner = partner,
                Trial = trial
            };

            if (!string.IsNullOrEmpty(source))
                externalToken.Source = string.Format("{0}-{1}", source, externalClient);

            if (!string.IsNullOrEmpty(formId))
                externalToken.FormId = FormIdBuilder.Build(formId, string.Format("-{0}", externalClient));

            _externaLoginStorage.AddOrUpdate(state, externalToken, (key, oldValue) => externalToken);

            return Redirect(uriBuilder.ToString());
        }

        [HttpGet, Route("callback")]
        public async Task<IHttpActionResult> ExternaLoginCallback(Guid state, CancellationToken cancellationToken, string code = null, string error = null)
        {
            if (!string.IsNullOrEmpty(error))
            {
                if ("access_denied".Equals(error, StringComparison.InvariantCultureIgnoreCase))
                    return Error("Your email is required to complete the sign-in process. Error is access_denied.");

                return Error(error);
            }

            ExternalToken externalToken;
            if (!_externaLoginStorage.TryGetValue(state, out externalToken))
                return Error("State is undefined");

            var service = OauthServiceFactory.CreateService(externalToken.Provider);
            var token = await service.TokenAsync(code, cancellationToken);

            if (object.Equals(token, null))
                return Error("Token is undefined");

            var profile = await service.ProfileAsync(token, cancellationToken);
            if (object.Equals(profile, null))
                return ErrorContent("invalid_grant", "Your email is required to complete the sign-in process. Profile is null.");

            if (string.IsNullOrEmpty(profile.Email))
                return ErrorContent("invalid_grant", "Your email is required to complete the sign-in process. Email is empty.");

            externalToken.Token = token;

            return Success();
        }

        [HttpGet, Route("session-token"), SaaSAuthorize]
        public async Task<IHttpActionResult> ExternaSessionToken()
        {
            try
            {
                return Ok(await _auth.SessionTokenExternalHistoriesAsync(AccountId));
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        [HttpPost, Route("session-token"), ValidateNullModel, ValidateModel, SaaSAuthorize]
        public async Task<IHttpActionResult> ExternaSessionTokenState([FromBody] SetPasswordViewModel model)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account user)
            {
                return await Task.Run(async () =>
                {
                    if (!model.IsConnect && user.IsEmptyPassword())
                    {
                        var sessionTokens = await _auth.SessionTokenExternalHistoriesAsync(user.Id);
                        if (!sessionTokens.Any(entity => entity.Id != model.Id && !entity.IsUnlinked))
                        {
                            if (object.Equals(model, null) || string.IsNullOrEmpty(model.NewPassword))
                                return ErrorContent("invalid_request", "Password is required.", httpStatusCode: HttpStatusCode.PreconditionFailed);

                            await _auth.AccountPasswordSetAsync(user, model.NewPassword);
                        }
                    }

                    await _auth.SessionTokenExternalHistorySetStateAsync(model.Id, !model.IsConnect);

                    return Ok();
                });
            });
        }

        private IHttpActionResult Success()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent("<script>window.open('', '_self').close();</script>", Encoding.UTF8, "text/html");
            return ResponseMessage(response);
        }

        private IHttpActionResult Error(string message)
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            response.Content = new StringContent(message, Encoding.UTF8, "text/html");
            return ResponseMessage(response);
        }

        private class ExternalToken
        {
            public Guid State { get; set; }
            public string Provider { get; set; }
            public TokenResult Token { get; set; }
            public DateTime CreateDate { get; set; }

            public Guid? VisitorId { get; set; }
            public int? Uid { get; set; }
            public string Cmp { get; set; }
            public string Source { get; set; }
            public string FormId { get; set; }
            public bool? Optin { get; set; }
            public string Build { get; set; }
            public string Partner { get; set; }
            public bool? Trial { get; set; }

            public ExternalToken(Guid state, string provider)
            {
                State = state;
                Provider = provider;

                CreateDate = DateTime.Now;
            }
        }
    }
}