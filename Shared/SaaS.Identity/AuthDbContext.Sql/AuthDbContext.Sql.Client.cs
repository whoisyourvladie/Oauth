using SaaS.Data.Entities.Oauth;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SaaS.Identity
{
    public partial class AuthDbContext
    {
        internal List<Client> ClientsGet()
        {
            return ExecuteReaderCollection<Client>("[oauth].[pClient]", new SqlParameter[] { });
        }
    }
}