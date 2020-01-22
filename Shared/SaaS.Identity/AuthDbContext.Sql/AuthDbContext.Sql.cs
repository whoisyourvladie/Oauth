using SaaS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthDbContext
    {
        private SqlParameter[] CreateSqlParams(Guid accountId)
        {
            return new SqlParameter[] { accountId.ToSql("accountId") };
        }
        private List<SqlParameter> CreateSqlParams(AccountProductPair pair)
        {
            return new List<SqlParameter>()
            {
                pair.AccountProductId.ToSql("accountProductId"),
                pair.AccountId.ToSql("accountId")
            };
        }

        internal async Task<T> ExecuteReaderAsync<T>(string commandText, Guid accountId)
        {
            return await ExecuteReaderAsync<T>(commandText, CreateSqlParams(accountId));
        }
        internal async Task<T> ExecuteReaderAsync<T>(string commandText, IEnumerable<SqlParameter> sqlParams)
        {
            var array = sqlParams.ToArray();
            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = array.CommandText(commandText);

                cmd.Parameters.AddRange(array);

                await Database.Connection.OpenAsync();

                var reader = await cmd.ExecuteReaderAsync();

                var entity = ((IObjectContextAdapter)this).ObjectContext.Translate<T>(reader).FirstOrDefault();

                Database.Connection.Close();

                return entity;
            }
        }
        internal List<T> ExecuteReaderCollection<T>(string commandText, IEnumerable<SqlParameter> sqlParams)
        {
            var array = sqlParams.ToArray();
            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = array.CommandText(commandText);

                cmd.Parameters.AddRange(array);

                Database.Connection.Open();

                var reader = cmd.ExecuteReader();

                var list = ((IObjectContextAdapter)this).ObjectContext.Translate<T>(reader).ToList();

                Database.Connection.Close();

                return list;
            }
        }

        internal async Task<List<T>> ExecuteReaderCollectionAsync<T>(string commandText, Guid accountId)
        {
            return await ExecuteReaderCollectionAsync<T>(commandText, CreateSqlParams(accountId));
        }
        internal async Task<List<T>> ExecuteReaderCollectionAsync<T>(string commandText, IEnumerable<SqlParameter> sqlParams)
        {
            var array = sqlParams.ToArray();
            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = array.CommandText(commandText);

                cmd.Parameters.AddRange(array);

                await Database.Connection.OpenAsync();

                var reader = await cmd.ExecuteReaderAsync();

                var list = ((IObjectContextAdapter)this).ObjectContext.Translate<T>(reader).ToList();

                Database.Connection.Close();

                return list;
            }
        }

        internal int ExecuteNonQuery(string commandText, IEnumerable<SqlParameter> sqlParams)
        {
            var array = sqlParams.ToArray();
            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = array.CommandText(commandText);

                cmd.Parameters.AddRange(array);

                Database.Connection.Open();

                var reader = cmd.ExecuteNonQuery();

                Database.Connection.Close();

                return reader;
            }
        }
        internal async Task<int> ExecuteNonQueryAsync(string commandText, IEnumerable<SqlParameter> sqlParams)
        {
            var array = sqlParams.ToArray();
            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = array.CommandText(commandText);

                cmd.Parameters.AddRange(array);

                await Database.Connection.OpenAsync();

                var reader = await cmd.ExecuteNonQueryAsync();

                Database.Connection.Close();

                return reader;
            }
        }

        internal async Task<object> ExecuteScalarAsync(string commandText, IEnumerable<SqlParameter> sqlParams)
        {
            var array = sqlParams.ToArray();
            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = array.CommandText(commandText);

                cmd.Parameters.AddRange(array);

                await Database.Connection.OpenAsync();

                var reader = await cmd.ExecuteScalarAsync();

                Database.Connection.Close();

                return reader;
            }
        }

        internal bool PasswordIsEqual(string source, string password)
        {
            return string.Equals(source, password.GetMD5Hash(), StringComparison.InvariantCultureIgnoreCase);
        }
    }
    internal static class SqlHelper
    {
        internal static SqlParameter ToSql(this object value, string key)
        {
            return object.Equals(value, null) ? new SqlParameter(key, DBNull.Value) : new SqlParameter(key, value);
        }

        internal static string CommandText(this SqlParameter[] sqlParams, string commandText)
        {
            StringBuilder builder = new StringBuilder(commandText);

            if (!object.Equals(sqlParams, null) && sqlParams.Length > 0)
            {
                foreach (var item in sqlParams)
                    builder.AppendFormat(" @{0}=@{0},", item.ParameterName);

                builder.Length--;
            }

            return builder.ToString();
        }
    }
}
