using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using SaaS.Data.Entities.Admin.Oauth;
using SaaS.Data.Entities.Admin.View;
using SaaS.Data.Entities.Admin.View.Oauth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Identity.Admin
{
    public interface IAuthRepository : IDisposable
    {
        void SetDataProtectorProvider(IDataProtectionProvider dataProtectorProvider);

        /*******************************************User****************************************************/
        Task<User> UserGetAsync(Guid userId);
        Task<User> UserGetAsync(string login);
        Task UserSetActiveAsync(Guid userId, bool isActive);

        Task<IdentityResult> UserChangePasswordAsync(Guid userId, string oldPassword, string newPassword);

        Task<ViewUser> ViewUserGetAsync(Guid userId);
        Task<List<ViewUser>> ViewUsersGetAsync();
        Task ViewUserSetAsync(ViewUser user);
        Task ViewUserInsertAsync(ViewUser user);
        Task<string> AccountGDPRDeleteAsync(string email);
        /*******************************************Log*****************************************************/
        Task<List<ViewLog>> LogsGetAsync(DateTime from, DateTime to, Guid? userId, string log, LogActionTypeEnum? logActionType);
        Task LogInsertAsync(Guid userId, Guid? accountId, string log, LogActionTypeEnum logActionType);

        Task<List<LogActionType>> LogActionTypesGetAsync();

        /*******************************************SessionToken********************************************/
        Task<SessionToken> SessionTokenGetAsync(Guid id);
        Task SessionTokenInsertAsync(SessionToken token);
    }
}
