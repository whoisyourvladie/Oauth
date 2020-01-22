using SaaS.Data.Entities.Admin.Oauth;
using SaaS.Data.Entities.Admin.View;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SaaS.Identity.Admin
{
    public partial class AuthDbContext
    {
        internal async Task<List<ViewLog>> LogsGetAsync(DateTime from, DateTime to, Guid? userId, string log, LogActionTypeEnum? logActionTypeEnum)
        {
            var sqlParams = new SqlParameter[]
            {
                from.ToSql("from"),
                to.ToSql("to"),
                userId.ToSql("userId"),
                log.ToSql("log"),
                logActionTypeEnum.ToSql("logActionTypeId")
            };

            return await ExecuteReaderCollectionAsync<ViewLog>("[oauth].[vLogGetBy]", sqlParams);
        }
        internal async Task LogInsertAsync(Guid userId, Guid? accountId, string log, LogActionTypeEnum logActionType)
        {
            var sqlParams = new SqlParameter[] {
                userId.ToSql("userId"),
                accountId.ToSql("accountId"),
                log.ToSql("log"),
                ((int)logActionType).ToSql("logActionTypeId")
            };

            await ExecuteNonQueryAsync("[oauth].[pLogInsert]", sqlParams);
        }

        internal async Task<List<LogActionType>> LogActionTypesGetAsync(DateTime from, DateTime to, Guid? userId, string log)
        {
            var sqlParams = new SqlParameter[]
            {
                from.ToSql("from"),
                to.ToSql("to"),
                userId.ToSql("userId"),
                log.ToSql("log")
            };

            return await ExecuteReaderCollectionAsync<LogActionType>("[oauth].[vLogGetBy]", sqlParams);
        }

        internal async Task<List<LogActionType>> LogActionTypesGetAsync()
        {
            return await ExecuteReaderCollectionAsync<LogActionType>("[oauth].[pLogActionTypeGet]", new SqlParameter[] { });
        }
    }
}