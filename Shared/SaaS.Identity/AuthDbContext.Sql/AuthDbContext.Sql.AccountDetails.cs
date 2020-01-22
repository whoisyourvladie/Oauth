using SaaS.Data.Entities.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthDbContext
    {
        internal async Task<ViewAccountDetails> AccountDetailsGetAsync(Guid accountId)
        {
            return await ExecuteReaderAsync<ViewAccountDetails>("[accounts].[pAccountDetailGetByAccountID]", accountId);
        }
        internal async Task AccountDetailsSetAsync(ViewAccountDetails accountDetails)
        {
            var sqlParams = new List<SqlParameter>
            {
                accountDetails.Id.ToSql("accountId"),
                accountDetails.FirstName.ToSql("firstName"),
                accountDetails.LastName.ToSql("lastName"),
                accountDetails.Phone.ToSql("phone"),
                accountDetails.PhoneESign.ToSql("phoneESign"),
                accountDetails.Company.ToSql("company"),
                accountDetails.Occupation.ToSql("occupation"),
                accountDetails.CountryISO2.ToSql("countryISO2"),
                accountDetails.LanguageISO2.ToSql("languageISO2"),
                accountDetails.Address1.ToSql("address1"),
                accountDetails.Address2.ToSql("address2"),
                accountDetails.City.ToSql("city"),
                accountDetails.State.ToSql("state"),
                accountDetails.PostalCode.ToSql("postalCode"),
                accountDetails.Build.ToSql("build"),
                accountDetails.Cmp.ToSql("cmp"),
                accountDetails.Uid.ToSql("uid"),
                accountDetails.GeoIp.ToSql("geoIP"),
                accountDetails.Source.ToSql("source"),
                accountDetails.Optin.ToSql("optin"),
                accountDetails.WebForm.ToSql("webForm"),
                accountDetails.Partner.ToSql("partner")
                //accountDetails.InstallationID.ToSql("installationID")
                //accountDetails.IsTrial.ToSql("isTrial")
            };
            //Warning! All transmitted parameters must be implemented on the DB side in this procedure of appropriate product.
#if LuluSoft
            sqlParams.Add(accountDetails.IsTrial.ToSql("trialDays"));
            sqlParams.Add(accountDetails.IsTrial.ToSql("isTrial"));
            sqlParams.Add(accountDetails.InstallationID.ToSql("installationID"));
#endif
            await ExecuteNonQueryAsync("[accounts].[pAccountDetailSetByAccountID]", sqlParams);
        }
        internal async Task<int?> AccountUidGetAsync(Guid accountId)
        {
            var sqlParams = new SqlParameter[]
            {
                accountId.ToSql("accountId")
            };

            object value = await ExecuteScalarAsync("[accounts].[pAccountUidGetByAccountID]", sqlParams);
            
            if (DBNull.Value == value)
                return null;

            return (int)value;
        }
    }
}