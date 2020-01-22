using SaaS.Api.Admin.Models.Oauth;
using SaaS.Data.Entities.Admin.Oauth;
using SaaS.Data.Entities.Admin.View.Oauth;
using SaaS.Identity.Admin;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Admin.Controllers.Api
{
    [RoutePrefix("api/user"), Authorize]
    public class UserController : SaaSApiController
    {
        [HttpGet, Authorize(Roles = "admin")]
        public async Task<IHttpActionResult> Index()
        {
            try
            {
                return Ok(await _authAdmin.ViewUsersGetAsync());
            }
            catch (Exception exc) { return ErrorContent(exc); }
        }

        [HttpPost, Route("insert"), Authorize(Roles = "admin")]
        public async Task<IHttpActionResult> Insert(ViewUser model)
        {
            try
            {
                var user = await _authAdmin.UserGetAsync(model.Login);

                if (!object.Equals(user, null))
                    return UserExists();

                await _authAdmin.ViewUserInsertAsync(model);

                var log = string.Format("User '{0}'({1}) has been created.", model.Login, model.Role);

                await LogInsertAsync(log, LogActionTypeEnum.UserCreate);

                return Ok();
            }
            catch (Exception exc) { return ErrorContent(exc); }
        }

        [HttpPost, Route("update"), Authorize(Roles = "admin")]
        public async Task<IHttpActionResult> Update(ViewUser model)
        {
            try
            {
                var user = await _authAdmin.UserGetAsync(model.Id);

                if (object.Equals(user, null))
                    return UserNotFound();

                var loginUser = await _authAdmin.UserGetAsync(model.Login);

                if (!object.Equals(loginUser, null) && loginUser.Id != user.Id)
                    return UserExists();

                await _authAdmin.ViewUserSetAsync(model);

                var log = string.Format("User '{0}'({1}) has been updated.", model.Login, model.Role);

                await LogInsertAsync(log, LogActionTypeEnum.UserEdit);

                return Ok();
            }
            catch (Exception exc) { return ErrorContent(exc); }
        }

        private async Task<IHttpActionResult> UserSetActive(Guid userId, bool isActive)
        {
            try
            {
                var user = await _authAdmin.ViewUserGetAsync(userId);

                if (object.Equals(user, null))
                    return UserNotFound();

                await _authAdmin.UserSetActiveAsync(userId, isActive);

                var log = string.Format("User '{0}'({1}) has been {2}.", user.Login, user.Role, isActive ? "activated" : "deactivated");

                await LogInsertAsync(log, isActive ? LogActionTypeEnum.UserActivate : LogActionTypeEnum.UserDeactivate);

                return Ok(await _authAdmin.ViewUserGetAsync(userId));
            }
            catch (Exception exc)
            {
                return ErrorContent(exc);
            }
        }

        [HttpPost, Route("{userId:guid}/activate"), Authorize(Roles = "admin")]
        public async Task<IHttpActionResult> Activate(Guid userId)
        {
            return await UserSetActive(userId, true);
        }

        [HttpPost, Route("{userId:guid}/deactivate"), Authorize(Roles = "admin")]
        public async Task<IHttpActionResult> Deactivate(Guid userId)
        {
            return await UserSetActive(userId, false);
        }

        [HttpPost, Route("change-password")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            return await CurrentUserExecuteAsync(async delegate (User user)
            {
                var result = await _authAdmin.UserChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);

                IHttpActionResult errorResult = GetErrorResult(result);

                if (!object.Equals(errorResult, null))
                    return errorResult;

                var log = string.Format("User '{0}' has updated his password. ", user.Login);

                await LogInsertAsync(log, LogActionTypeEnum.UserEditPassword);

                return Ok();
            });
        }
    }
}
