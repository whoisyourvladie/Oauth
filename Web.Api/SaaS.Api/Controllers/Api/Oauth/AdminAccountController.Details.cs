using AutoMapper;
using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Oauth;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.View;
using SaaS.IPDetect;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AdminAccountController
    {
        [HttpGet, Route("info")]
        public async Task<IHttpActionResult> Info(string email)
        {
            var user = await _auth.AccountGetAsync(email);

            if (object.Equals(user, null))
                return AccountNotFound();

            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                status = user.GetStatus()
            });
        }

        [HttpPost, Route("info"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> Info(Guid userId, AuthNameViewModel model)
        {
            var user = await _auth.AccountGetAsync(userId);

            if (object.Equals(user, null))
                return AccountNotFound();

            var userWithEmail = await _auth.AccountGetAsync(model.Email);

            if (!object.Equals(userWithEmail, null) && user.Id != userWithEmail.Id)
                return AccountExists();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;

            var result = await _auth.AccountUpdateAsync(user);

            var errorResult = GetErrorResult(result);
            if (!object.Equals(errorResult, null))
                return errorResult;

            return Ok();
        }

        [HttpGet, Route("details")]
        public async Task<IHttpActionResult> Details(Guid userId)
        {
            try
            {
                var user = await _auth.AccountGetAsync(userId);

                if (object.Equals(user, null))
                    return AccountNotFound();

                var accountDetails = await _auth.AccountDetailsGetAsync(user.Id);
                var accountDetailsViewModel = new AccountDetailsViewModel();

                accountDetailsViewModel = Mapper.Map(accountDetails, accountDetailsViewModel);

                return Ok(accountDetailsViewModel);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        [HttpPost, Route("details"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> Details(Guid userId, AccountDetailsViewModel model)
        {
            try
            {
                var account = await _auth.AccountGetAsync(userId);

                if (object.Equals(account, null))
                    return AccountNotFound();

                var accountsDetail = new ViewAccountDetails();
                accountsDetail = Mapper.Map(model, accountsDetail);

                accountsDetail.GeoIp = IpAddressDetector.IpAddress;
                accountsDetail.Id = account.Id;

                await _auth.AccountDetailsSetAsync(accountsDetail);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }
    }
}