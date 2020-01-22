using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.Admin.Oauth;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Admin.Controllers.Api
{
    public partial class AccountController : SaaSApiController
    {
        [HttpPost, Route("{accountIdTo:guid}/merge")]
        public async Task<IHttpActionResult> AccountMerge(Guid accountIdTo, Guid accountIdFrom, Guid accountIdPrimaryEmail)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                var targetUser = account;
                var targetUserFrom = await _auth.AccountGetAsync(accountIdFrom);
                if (object.Equals(targetUserFrom, null))
                    return AccountNotFound();

                var targetUserPrimaryEmail = accountIdPrimaryEmail == accountIdFrom ? targetUserFrom: targetUser;

                await _auth.AccountMergePendingDeleteAsync(account.Id);
                var pending = await _auth.AccountMergePendingMergeAsync(targetUser.Id, targetUserFrom.Id, targetUserPrimaryEmail.Id);
                await _auth.AccountMergeAsync(pending);

                var log = string.Format("Customer({0}) has been merged with {1}. Primary email is {2}", targetUser.Email, targetUserFrom.Email, targetUserPrimaryEmail.Email);

                await LogInsertAsync(log, LogActionTypeEnum.AccountMerge, account.Id);

                return Ok();

            }, accountIdTo);
        }
    }
}