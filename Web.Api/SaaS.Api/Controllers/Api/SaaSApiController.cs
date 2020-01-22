using Microsoft.AspNet.Identity;
using NLog;
using SaaS.Api.Core;
using SaaS.Api.Models.Oauth;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.Oauth;
using SaaS.Data.Entities.View;
using SaaS.Identity;
using SaaS.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Upclick.Api.Client;

namespace SaaS.Api.Controllers.Api
{
    public class SaaSApiController : ApiController
    {
        private static List<Client> _clients;

        protected readonly INpsRepository _nps;
        protected readonly IAuthRepository _auth;
        protected readonly IAuthProductRepository _authProduct;
        protected static readonly IAppSettings _appSettings = new AppSettings();

        protected static Logger _oautLogLogger = LogManager.GetLogger("oauth-log");

        public SaaSApiController()
        {
            _nps = new NpsRepository();
            _auth = new AuthRepository();
            _auth.SetDataProtectorProvider(Startup.DataProtectionProvider);

            _authProduct = new AuthProductRepository();
        }

        private UpclickClient _upclickClient;
        private NotificationManager _notificationManager;

        protected UpclickClient UpclickClient
        {
            get
            {
                _upclickClient = _upclickClient ?? new UpclickClient(_appSettings.Upclick.MerchantLogin, _appSettings.Upclick.MerchantPassword);

                return _upclickClient;
            }
        }

        protected NotificationManager NotificationManager
        {
            get
            {
                _notificationManager = _notificationManager ?? new NotificationManager(_auth, new NotificationSettings
                {
                    DownloadLink = _appSettings.DownloadLink,
                    CreatePassword = _appSettings.Oauth.CreatePassword,
                    EmailConfirmation = _appSettings.Oauth.EmailConfirmation,
                    MergeConfirmation = _appSettings.Oauth.MergeConfirmation,
                    EmailChangeConfirmation = _appSettings.Oauth.EmailChangeConfirmation,
                    ResetPassword = _appSettings.Oauth.ResetPassword
                });

                return _notificationManager;
            }
        }

        protected Client GetClient(int id)
        {
            _clients = _clients ?? _auth.ClientsGet();

            return _clients.FirstOrDefault(e => e.Id == id);
        }

        protected Client GetClient(string name)
        {
            _clients = _clients ?? _auth.ClientsGet();

            return _clients.FirstOrDefault(e => string.Equals(e.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }

        protected override void Dispose(bool disposing)
        {
            _nps.Dispose();
            _auth.Dispose();
            _authProduct.Dispose();

            base.Dispose(disposing);
        }

        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            string error = null;

            if (object.Equals(result, null))
                return ErrorContent(error, httpStatusCode: HttpStatusCode.InternalServerError);

            if (result.Succeeded)
                return null;

            if (!object.Equals(result.Errors, null))
                error = result.Errors.FirstOrDefault();

            return ErrorContent("invalid_request", error);
        }

        protected IHttpActionResult AccountNotFound()
        {
            return ErrorContent("invalid_grant", "User is not exists.", HttpStatusCode.NotFound);
        }

        protected IHttpActionResult ClientNotFound()
        {
            return ErrorContent("invalid_clientId", "This client is not registered in the system.");
        }

        protected IHttpActionResult AccountExists()
        {
            return ErrorContent("invalid_grant", "User already exists.", HttpStatusCode.Conflict);
        }

        protected IHttpActionResult AccountUnauthorized()
        {
            return ErrorContent("invalid_grant", "Authorization has been denied for this request.", HttpStatusCode.Unauthorized);
        }

        protected IHttpActionResult AccountNotActivated()
        {
            return ErrorContent("invalid_grant", "This account is not activated.", HttpStatusCode.BadRequest);
        }

        protected IHttpActionResult AccountIsDisconnected(string provider)
        {
            return ErrorContent("invalid_grant", string.Format("Your {0} account is disconnected.", provider), HttpStatusCode.BadRequest);
        }

        protected IHttpActionResult ProductNotFound()
        {
            return ErrorContent("invalid_grant", "Product is not exists or is disabled.", HttpStatusCode.NotFound);
        }

        protected IHttpActionResult ErrorContent(string error, string description = null, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            return Content<dynamic>(httpStatusCode, new
            {
                error = error,
                error_description = description
            });
        }

        protected delegate Task<IHttpActionResult> AccountExecuteHandlerAsync(Account account);
        protected delegate Task<IHttpActionResult> OwnerProductExecuteHandlerAsync(Guid accountId, ViewOwnerProduct product);
        protected async Task<IHttpActionResult> CurrentAccountExecuteAsync(AccountExecuteHandlerAsync handler)
        {
            try
            {
                Guid userId;
                if (!Guid.TryParse(User.Identity.GetUserId(), out userId))
                    return AccountNotFound();

                var user = await _auth.AccountGetAsync(userId);
                if (object.Equals(user, null))
                    return AccountNotFound();

                return await handler(user);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }
        protected async Task<IHttpActionResult> AccountExecuteAsync(AccountExecuteHandlerAsync handler, AuthViewModel model)
        {
            try
            {
                var user = await _auth.AccountGetAsync(model.Email);
                if (object.Equals(user, null) || user.IsAnonymous)
                    return AccountNotFound();

                return await handler(user);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }
        protected async Task<IHttpActionResult> OwnerProductExecuteAsync(OwnerProductExecuteHandlerAsync handler, Guid accountProductId, bool skipConflict = false)
        {
            try
            {
                var product = await _authProduct.OwnerProductGetAsync(CreateAccountProductPair(accountProductId));

                if (object.Equals(product, null) || product.IsDisabled)
                    return ProductNotFound();

                if (!skipConflict && !product.IsOwner)
                    return Conflict();

                return await handler(AccountId, product);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        protected Guid AccountId
        {
            get { return Guid.Parse(User.Identity.GetUserId()); }
        }
        protected async Task<Account> GetAccountAsync()
        {
            return await _auth.AccountGetAsync(AccountId);
        }

        protected AccountProductPair CreateAccountProductPair(Guid accountProductId)
        {
            return new AccountProductPair(AccountId, accountProductId);
        }
    }
}
