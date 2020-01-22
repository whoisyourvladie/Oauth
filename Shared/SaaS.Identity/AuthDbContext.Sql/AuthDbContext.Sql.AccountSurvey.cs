using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SaaS.Data.Entities.Accounts;

namespace SaaS.Identity
{
    public partial class AuthDbContext
    {
        internal async Task SurveyAsync(Guid accountId, byte[] data, string lang)
        {
            var action = 1; // INSERT
            var sqlParams = new SqlParameter[]
            {
                accountId.ToSql("accountID"),
                data.ToSql("collectedData"),
                lang.ToSql("lang"),
                action.ToSql("action")
            };

            await ExecuteNonQueryAsync("[accounts].[pAccountQuiz]", sqlParams);
        }

        internal async Task<List<AccountSurvey>> GetAllSurveyAsync()
        {
            var query = "SELECT * FROM [accounts].[accountQuiz]";
            return await ExecuteReaderCollectionAsync<AccountSurvey>(query, new SqlParameter[]{ });
        }
    }
}
