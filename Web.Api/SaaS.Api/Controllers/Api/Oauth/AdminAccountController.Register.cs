using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Oauth;
using SaaS.Data.Entities.Accounts;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AdminAccountController
    {
        [HttpPost, Route("register"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> Register(AuthNameViewModel model)
        {
            try
            {
                var user = await _auth.AccountGetAsync(model.Email, isIncludeSubEmails: true);

                if (!object.Equals(user, null))
                    return AccountExists();

                user = new Account(model.Email, model.FirstName, model.LastName);

                var result = await _auth.AccountCreateAsync(user);

                var errorResult = GetErrorResult(result);

                if (!object.Equals(errorResult, null))
                    return errorResult;
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }
    }
}