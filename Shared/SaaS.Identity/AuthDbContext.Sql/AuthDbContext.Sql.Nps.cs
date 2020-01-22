using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthDbContext
    {
        internal async Task NpsInsertAsync(string questioner,
            Guid? accountId,
            string clientName, string clientVersion,
            byte rating, byte? ratingUsage, string comment)
        {
            var sqlParams = new SqlParameter[]
            {
                //questioner.ToSql("npsQuestionerName"),
                accountId.ToSql("accountId"),
                clientName.ToSql("clientName"),
                clientVersion.ToSql("clientVersion"),
                rating.ToSql("rating")//,
                //ratingUsage.ToSql("ratingUsage"),
                //comment.ToSql("comment")
            };

            await ExecuteNonQueryAsync("[accounts].[pNpsInsert]", sqlParams);
        }
    }
}