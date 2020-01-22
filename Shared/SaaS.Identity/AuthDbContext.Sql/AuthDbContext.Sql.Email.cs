using SaaS.Data.Entities;
using SaaS.Data.Entities.Accounts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthDbContext
    {
        internal async Task EmailInsertAsync(Guid accountId, Status status, string emailCustomParam, string modelCustomParam)
        {
            var sqlParams = new SqlParameter[]
            {
                accountId.ToSql("accountId"),
                status.ToSql("status"),
                emailCustomParam.ToSql("emailCustomParam"),
                modelCustomParam.ToSql("modelCustomParam")
            };

            await ExecuteNonQueryAsync("[accounts].[pEmailInsert]", sqlParams);
        }

        internal async Task<List<Email>> EmailsGetAsync(Status status, int top)
        {
            var sqlParams = new SqlParameter[]
            {
                status.ToSql("status"),
                top.ToSql("top")
            };

            return await ExecuteReaderCollectionAsync<Email>("[accounts].[pEmailGetByStatusId]", sqlParams);
        }

        internal async Task EmailStatusSetAsync(int id, Status status)
        {
            var sqlParams = new SqlParameter[]
            {
                id.ToSql("id"),
                status.ToSql("status")
            };

            await ExecuteNonQueryAsync("[accounts].[pEmailSetStatusById]", sqlParams);
        }
    }
}