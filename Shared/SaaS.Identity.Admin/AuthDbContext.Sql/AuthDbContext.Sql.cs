using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaS.Identity.Admin
{
    public partial class AuthDbContext
    {
        internal async Task<T> ExecuteReaderAsync<T>(string commandText, SqlParameter[] sqlParams)
        {
            try
            {
                using (var cmd = Database.Connection.CreateCommand())
                {
                    cmd.CommandText = sqlParams.CommandText(commandText);

                    cmd.Parameters.AddRange(sqlParams);

                    await Database.Connection.OpenAsync();

                    var reader = await cmd.ExecuteReaderAsync();

                    var entity = ((IObjectContextAdapter)this).ObjectContext.Translate<T>(reader).FirstOrDefault();

                    Database.Connection.Close();

                    return entity;
                }
            }
            catch (Exception exc )
            {
                throw exc;
            }
        }
        internal async Task<List<T>> ExecuteReaderCollectionAsync<T>(string commandText, SqlParameter[] sqlParams)
        {
            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = sqlParams.CommandText(commandText);

                cmd.Parameters.AddRange(sqlParams);

                await Database.Connection.OpenAsync();

                var reader = await cmd.ExecuteReaderAsync();

                var list = ((IObjectContextAdapter)this).ObjectContext.Translate<T>(reader).ToList();

                Database.Connection.Close();

                return list;
            }
        }
        internal async Task<int> ExecuteNonQueryAsync(string commandText, SqlParameter[] sqlParams)
        {
            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = sqlParams.CommandText(commandText);

                cmd.Parameters.AddRange(sqlParams);
                
              
                await Database.Connection.OpenAsync();
             
                var reader = await cmd.ExecuteNonQueryAsync();

                Database.Connection.Close();
                cmd.Parameters.Clear();

                return reader;
            }
        }

        internal async Task<String> ExecuteNonQueryOutParamAsync(string commandText, string email)
        {
            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.StoredProcedure;

                IDbDataParameter sourceId = cmd.CreateParameter();
                IDbDataParameter typeId = cmd.CreateParameter();
                IDbDataParameter userEmail = cmd.CreateParameter();
                IDbDataParameter processId = cmd.CreateParameter();

                sourceId.ParameterName = "@sourceID";
                sourceId.Value = 1;
                cmd.Parameters.Add(sourceId);

                typeId.ParameterName = "@typeID";
                typeId.Value = 6;
                cmd.Parameters.Add(typeId);

                userEmail.ParameterName = "@userEmail";
                userEmail.Value = email;
                cmd.Parameters.Add(userEmail);

                //OUT param
                processId.ParameterName = "@processID";
                processId.Direction = ParameterDirection.Output;
                processId.DbType = DbType.String;
                processId.Size = 50;
                cmd.Parameters.Add(processId);

                await Database.Connection.OpenAsync();

                await cmd.ExecuteNonQueryAsync();

                Database.Connection.Close();

                return cmd.Parameters["@processID"].Value.ToString();
            }
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