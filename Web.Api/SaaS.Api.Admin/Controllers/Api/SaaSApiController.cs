using Microsoft.AspNet.Identity;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.Admin.Oauth;
using SaaS.Identity;
using SaaS.Notification;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Upclick.Api.Client;
using AuthAdminRepository = SaaS.Identity.Admin.AuthRepository;
using AuthRepository = SaaS.Identity.AuthRepository;
using IAuthAdminRepository = SaaS.Identity.Admin.IAuthRepository;
using IAuthRepository = SaaS.Identity.IAuthRepository;
using UserIdentityAdmin = SaaS.Identity.Admin.User;

namespace SaaS.Api.Admin.Controllers.Api
{
    public class SaaSApiController : ApiController
    {

        protected readonly IAuthRepository _auth;
        protected readonly IAuthAdminRepository _authAdmin;
        protected readonly IAuthProductRepository _authProduct;
        //**********
        protected readonly IAuthAdminRepository _gdpr;

        protected static readonly IAppSettings _appSettings = new AppSettings();

        public SaaSApiController()
        {
            _auth = new AuthRepository();
            _auth.SetDataProtectorProvider(Startup.DataProtectionProvider);

            _authAdmin = new AuthAdminRepository();
            _authAdmin.SetDataProtectorProvider(Startup.DataProtectionProvider);

            _authProduct = new AuthProductRepository();
            _gdpr = new AuthAdminRepository("udb");
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
                    ResetPassword = _appSettings.Oauth.ResetPassword
                });

                return _notificationManager;
            }
        }

        protected override void Dispose(bool disposing)
        {
            _auth.Dispose();
            _authAdmin.Dispose();

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

        protected IHttpActionResult ErrorContent(Exception exc)
        {
            return ErrorContent("invalid_request", exc.Message);
        }

        protected IHttpActionResult UserNotFound()
        {
            return ErrorContent("invalid_grant", "User is not exists.", HttpStatusCode.NotFound);
        }
        protected IHttpActionResult AccountNotFound()
        {
            return ErrorContent("invalid_grant", "Customer is not exists.", HttpStatusCode.NotFound);
        }

        protected IHttpActionResult UserExists()
        {
            return ErrorContent("invalid_grant", "User already exists.", HttpStatusCode.Conflict);
        }
        protected IHttpActionResult AccountExists()
        {
            return ErrorContent("invalid_grant", "Customer already exists.", HttpStatusCode.Conflict);
        }

        protected async Task LogInsertAsync(string log, LogActionTypeEnum logActionType, Guid? accountId = null)
        {
            await _authAdmin.LogInsertAsync(UserId, accountId, log, logActionType);
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

        public Guid UserId
        {
            get { return Guid.Parse(User.Identity.GetUserId()); }
        }

        protected delegate Task<IHttpActionResult> UserExecuteHandlerAsync(UserIdentityAdmin user);
        protected async Task<IHttpActionResult> CurrentUserExecuteAsync(UserExecuteHandlerAsync handler)
        {
            try
            {
                var user = await _authAdmin.UserGetAsync(UserId);
                if (object.Equals(user, null))
                    return UserNotFound();

                return await handler(user);
            }
            catch (Exception exc) { return ErrorContent(exc); }
        }

        protected delegate Task<IHttpActionResult> AccountExecuteHandlerAsync(Account account);
        protected async Task<IHttpActionResult> CurrentAccountExecuteAsync(AccountExecuteHandlerAsync handler, Guid accountId)
        {
            try
            {
                var account = await _auth.AccountGetAsync(accountId);
                if (object.Equals(account, null))
                    return AccountNotFound();

                return await handler(account);
            }
            catch (Exception exc) { return ErrorContent(exc); }
        }
        protected async Task<IHttpActionResult> CurrentAccountExecuteAsync(AccountExecuteHandlerAsync handler, string email, bool isIncludeSubEmails = false)
        {
            try
            {
                var account = await _auth.AccountGetAsync(email, isIncludeSubEmails: isIncludeSubEmails);
                if (object.Equals(account, null))
                    return AccountNotFound();

                return await handler(account);
            }
            catch (Exception exc) { return ErrorContent(exc); }
        }
    }
}