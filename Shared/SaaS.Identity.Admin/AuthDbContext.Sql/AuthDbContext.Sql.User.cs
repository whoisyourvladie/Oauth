using SaaS.Common.Extensions;
using SaaS.Data.Entities.Admin.View.Oauth;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SaaS.Identity.Admin
{
    public partial class AuthDbContext
    {
        internal bool PasswordIsEqual(string source, string password)
        {
            return string.Equals(source, password.GetMD5Hash(), StringComparison.InvariantCultureIgnoreCase);
        }

        internal async Task<User> UserGetAsync(Guid userId)
        {
            var sqlParams = new SqlParameter[] { userId.ToSql("id") };

            return await ExecuteReaderAsync<User>("[oauth].[pUserGetById]", sqlParams);
        }
        internal async Task<User> UserGetAsync(string login)
        {
            var sqlParams = new SqlParameter[] { login.ToSql("login") };

            return await ExecuteReaderAsync<User>("[oauth].[pUserGetByLogin]", sqlParams);
        }
        internal async Task UserSetActiveAsync(Guid userId, bool isActive)
        {
            var sqlParams = new SqlParameter[]
            {
                userId.ToSql("id"),
                isActive.ToSql("isActive")
            };

            await ExecuteNonQueryAsync("[oauth].[pUserSetActive]", sqlParams);
        }

        internal async Task<ViewUser> ViewUserGetAsync(Guid userId)
        {
            var sqlParams = new SqlParameter[] { userId.ToSql("id") };

            return await ExecuteReaderAsync<ViewUser>("[oauth].[vUserGetById]", sqlParams);
        }
        internal async Task<ViewUser> ViewUserGetAsync(string login, string password)
        {
            var sqlParams = new SqlParameter[] { login.ToSql("login") };

            var user = await ExecuteReaderAsync<ViewUser>("[oauth].[vUserGetByLogin]", sqlParams);

            if (!object.Equals(user, null) && !PasswordIsEqual(user.Password, password))
                user = null;

            return user;
        }
        internal async Task<List<ViewUser>> ViewUsersGetAsync()
        {
            return await ExecuteReaderCollectionAsync<ViewUser>("[oauth].[vUserGet]", new SqlParameter[] { });
        }

        internal async Task ViewUserSetAsync(ViewUser user)
        {
            var password = string.IsNullOrEmpty(user.Password) ? null : user.Password.Trim().GetMD5Hash();

            var sqlParams = new SqlParameter[]
            {
                user.Id.ToSql("id"),
                user.Login.ToSql("login"),
                password.ToSql("password"),
                user.Role.ToSql("role")
            };

            await ExecuteNonQueryAsync("[oauth].[vUserSet]", sqlParams);
        }
        internal async Task ViewUserInsertAsync(ViewUser user)
        {
            var password = string.IsNullOrEmpty(user.Password) ? null : user.Password.Trim().GetMD5Hash();

            var sqlParams = new SqlParameter[]
            {
                user.Login.ToSql("login"),
                password.ToSql("password"),
                user.Role.ToSql("role")
            };

            await ExecuteNonQueryAsync("[oauth].[vUserInsert]", sqlParams);
        }

        internal async Task<string> AccountGDPRDeleteAsync(string userEmail)
        {
            var error="";
            try {
                var sqlParams = new SqlParameter[]
            {
                1.ToSql("action"),
                1.ToSql("sourceID"),
                6.ToSql("typeID"),
                userEmail.ToSql("userEmail")
            };
                var status = await ExecuteNonQueryAsync("[gdpr].[pQueue]", sqlParams);

                if (status == -1) {
                 
                    var data = await ExecuteNonQueryOutParamAsync("[gdpr].[pQueue_Process]", userEmail);
                    if (data.Length > 0)
                        return "success";

                }
                
            }
            catch (Exception ex) {
                error = ex.Message;
            }

            return error;
        }
    }
}