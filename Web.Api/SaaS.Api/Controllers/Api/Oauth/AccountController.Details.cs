using AutoMapper;
using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Oauth;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.View;
using SaaS.IPDetect;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AccountController
    {
        private IHttpActionResult GetAccountResult(Account account)
        {
            return Ok(new
            {
                email = account.Email,
                firstName = account.FirstName,
                lastName = account.LastName,
                status = account.GetStatus(),

                //need remove it in future
                isAnonymous = account.IsAnonymous,
                isActivated = account.IsActivated
            });
        }

        private async Task<AccountDetailsViewModel> GetAccountDetails()
        {
            var accountDetails = await _auth.AccountDetailsGetAsync(AccountId);
            if (object.Equals(accountDetails, null))
                return null;

            var accountDetailsViewModel = new AccountDetailsViewModel();
            return Mapper.Map(accountDetails, accountDetailsViewModel);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetByEmail(string email)
        {
            try
            {
                var account = await _auth.AccountGetAsync(email, isIncludeSubEmails: true);
                if (object.Equals(account, null))
                    return AccountNotFound();
                else if (account.IsEmptyPassword())
                {
                    var sessionTokensExternalHistory = await _auth.SessionTokenExternalHistoriesAsync(account.Id);
                    var sessionTokenExternalHistory = sessionTokensExternalHistory.FirstOrDefault(e => !e.IsUnlinked);
                    if (!object.Equals(sessionTokenExternalHistory, null))
                    {
                        return Ok(new
                        {
                            email = account.Email,
                            firstName = account.FirstName,
                            lastName = account.LastName,
                            status = account.GetStatus(),
                            optin = account.Optin,
                            external = sessionTokenExternalHistory.ExternalClientName
                        });
                    }
                }

                return Ok(new
                {
                    email = account.Email,
                    firstName = account.FirstName,
                    lastName = account.LastName,
                    status = account.GetStatus(),
                    optin = account.Optin
                });
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        [HttpGet, Route("info"), SaaSAuthorize]
        public async Task<IHttpActionResult> Info()
        {
            return await CurrentAccountExecuteAsync(async delegate (Account user)
            {
                return await Task.Run(() => { return GetAccountResult(user); });
            });
        }

        [HttpGet, Route("{accountId:guid}/info")]
        public async Task<IHttpActionResult> InfoById(Guid accountId)
        {
            try
            {
                var account = await _auth.AccountGetAsync(accountId);
                if (object.Equals(account, null))
                    return AccountNotFound();

                return GetAccountResult(account);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        [HttpPost, Route("info"), SaaSAuthorize, ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> Info(UserInfoViewModel model)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                account.FirstName = model.FirstName;
                account.LastName = model.LastName;

                var errorResult = GetErrorResult(await _auth.AccountUpdateAsync(account));
                if (!object.Equals(errorResult, null))
                    return errorResult;

                return Ok();
            });
        }

        [HttpGet, Route("details"), SaaSAuthorize]
        public async Task<IHttpActionResult> Details()
        {
            try
            {
                var accountDetails = await GetAccountDetails();
                if (object.Equals(accountDetails, null))
                    return AccountNotFound();

                return Ok(accountDetails);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        [HttpPost, Route("details"), SaaSAuthorize, ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> Details(AccountDetailsViewModel model)
        {
            try
            {
                var accountsDetail = new ViewAccountDetails();
                accountsDetail = Mapper.Map(model, accountsDetail);
                accountsDetail.Id = AccountId;

                accountsDetail.GeoIp = IpAddressDetector.IpAddress;
                await _auth.AccountDetailsSetAsync(accountsDetail);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }
    }
}