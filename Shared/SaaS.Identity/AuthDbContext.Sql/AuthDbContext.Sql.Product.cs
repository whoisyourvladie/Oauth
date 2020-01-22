using SaaS.Data.Entities;
using SaaS.Data.Entities.View;
using SaaS.Data.Entities.View.Oauth;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthDbContext
    {
        internal async Task<AssignStatus> ProductAssignAsync(AccountProductPair pair, bool isIgnoreBillingCycle = false)
        {
            var sqlParams = CreateSqlParams(pair);

            sqlParams.Add(isIgnoreBillingCycle.ToSql("isIgnoreBillingCycle"));

            var scalar = (int)await ExecuteScalarAsync("[accounts].[pProductAssign]", sqlParams);

            return (AssignStatus)scalar;
        }
        internal async Task<ViewUnassignProduct> ProductUnassignAsync(AccountProductPair pair)
        {
            var sqlParams = CreateSqlParams(pair);

            return await ExecuteReaderAsync<ViewUnassignProduct>("[accounts].[pProductUnAssign]", sqlParams);
        }        
        internal async Task ProductDeactivateAsync(AccountProductPair pair)
        {
            var sqlParams = new SqlParameter[]
            {
                pair.AccountProductId.ToSql("accountProductId"),
                3.ToSql("activationReason")
            };

            await ExecuteNonQueryAsync("[accounts].[pProductDeactivate]", sqlParams);
        }
        internal async Task ProductNextRebillDateSetAsync(AccountProductPair pair, DateTime? nextRebillDate)
        {
            var sqlParams = new SqlParameter[]
            {
                pair.AccountId.ToSql("accountId"),
                pair.AccountProductId.ToSql("accountProductId"),
                nextRebillDate.ToSql("nextRebillDate")
            };

            await ExecuteNonQueryAsync("[accounts].[pProductSetIsRenewalByAccountId]", sqlParams);
        }
        internal async Task ProductEndDateSetAsync(AccountProductPair pair, DateTime endDate)
        {
            var sqlParams = new SqlParameter[]
            {
                pair.AccountId.ToSql("accountId"),
                pair.AccountProductId.ToSql("accountProductId"),
                endDate.ToSql("expiryDate")
            };

            await ExecuteNonQueryAsync("[accounts].[pProductSetExpiryDateByAccountId]", sqlParams);
        }
        internal async Task ProductIsNewAsync(AccountProductPair pair, bool isNew)
        {
            var sqlParams = CreateSqlParams(pair);

            sqlParams.Add(isNew.ToSql("isNew"));

            await ExecuteNonQueryAsync("[accounts].[pProductSetIsNewByAccountId]", sqlParams);
        }

        internal async Task<ViewUpgradeProduct> UpgradeProductGetAsync(Guid accountProductId)
        {
            var sqlParams = new SqlParameter[] { accountProductId.ToSql("accountProductId") };

            return await ExecuteReaderAsync<ViewUpgradeProduct>("[accounts].[pProductUpgradeGetByAccountProductId]", sqlParams);
        }

        internal async Task<List<ViewAccountProduct>> AccountProductsGetAsync(Guid accountId, Guid? systemId)
        {
            var sqlParams = new SqlParameter[]
            {
                accountId.ToSql("accountId"),
                systemId.ToSql("systemId")
            };

            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = sqlParams.CommandText("[accounts].[pProductGetByAccountId]");
                cmd.Parameters.AddRange(sqlParams);

                await Database.Connection.OpenAsync();

                var reader = await cmd.ExecuteReaderAsync();

                var products = ((IObjectContextAdapter)this).ObjectContext.Translate<ViewAccountProduct>(reader).ToList();
                reader.NextResult();
                var modules = ((IObjectContextAdapter)this).ObjectContext.Translate<ViewAccountProductModule>(reader).ToList();

                Database.Connection.Close();

                foreach (var product in products)
                    product.Modules = modules.Where(e => e.AccountProductId == product.AccountProductId).ToList();

                return products;
            }
        }

        internal async Task<List<ViewAccountMicrotransaction>> AccountMicrotransactionsGetAsync(Guid accountId)
        {
            var sqlParams = CreateSqlParams(accountId);

            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = sqlParams.CommandText("[accounts].[pMicrotransactionGetByAccountId]");
                cmd.Parameters.AddRange(sqlParams);

                await Database.Connection.OpenAsync();

                var reader = await cmd.ExecuteReaderAsync();

                var products = ((IObjectContextAdapter)this).ObjectContext.Translate<ViewAccountMicrotransaction>(reader).ToList();
                reader.NextResult();
                var modules = ((IObjectContextAdapter)this).ObjectContext.Translate<ViewAccountMicrotransactionModule>(reader).ToList();

                Database.Connection.Close();

                foreach (var product in products)
                    product.Modules = modules.Where(e => e.AccountMicrotransactionId == product.AccountMicrotransactionId).ToList();

                return products;
            }
        }

        internal async Task<List<ViewUpclickProduct>> UpclickProductsGetAsync()
        {
            return await ExecuteReaderCollectionAsync<ViewUpclickProduct>("[products].[pProductUpclickGet]", new SqlParameter[] { });
        }

        internal async Task<ViewOwnerProduct> OwnerProductGetAsync(AccountProductPair pair)
        {
            var sqlParams = new SqlParameter[]
            {
                pair.AccountId.ToSql("accountId"),
                pair.AccountProductId.ToSql("accountProductId")
            };
                
            return await ExecuteReaderAsync<ViewOwnerProduct>("[accounts].[pProductGetByOwnerIDAccountProductId]", sqlParams);
        }
        internal async Task<ViewOwnerProduct> OwnerProductDetailsGetAsync(AccountProductPair pair)
        {
            var sqlParams = CreateSqlParams(pair).ToArray();

            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = sqlParams.CommandText("[accounts].[pProductDetailGetByOwnerId]");
                cmd.Parameters.AddRange(sqlParams);

                await Database.Connection.OpenAsync();

                var reader = await cmd.ExecuteReaderAsync();

                var product = ((IObjectContextAdapter)this).ObjectContext.Translate<ViewOwnerProduct>(reader).FirstOrDefault();
                if (!object.Equals(product, null))
                {
                    reader.NextResult();
                    var accounts = ((IObjectContextAdapter)this).ObjectContext.Translate<ViewOwnerAccount>(reader).ToList();
                    reader.NextResult();
                    var accountSystems = ((IObjectContextAdapter)this).ObjectContext.Translate<ViewAccountSystem>(reader).ToList();
                    reader.NextResult();
                    var sessionTokens = ((IObjectContextAdapter)this).ObjectContext.Translate<ViewSessionToken>(reader).ToList();
                    Database.Connection.Close();

                    foreach (var account in accounts)
                    {
                        account.AccountSystems = accountSystems.Where(e => e.AccountId == account.AccountId).ToList();
                        account.SessionTokens = sessionTokens.Where(e => e.AccountId == account.AccountId).ToList();
                    }

                    product.Accounts = accounts;
                }

                return product;
            }
        }
        internal async Task<AllowedCountStatus> ProductAllowedSetAsync(AccountProductPair pair, int allowedCount)
        {
            var sqlParams = CreateSqlParams(pair);

            sqlParams.Add(allowedCount.ToSql("allowedCount"));

            var scalar = (int)await ExecuteScalarAsync("[accounts].[pProductSetAllowedCountByOwnerIDAccountProductId] ", sqlParams);

            return (AllowedCountStatus)scalar;
        }
        internal async Task<List<ViewOwnerProduct>> OwnerProductsGetAsync(Guid accountId)
        {
            return await ExecuteReaderCollectionAsync<ViewOwnerProduct>("[accounts].[pProductGetByOwnerId]", CreateSqlParams(accountId));
        }
        internal async Task OwnerProductInsertAsync(Guid accountId, string productUid, string currency, decimal price, decimal priceUsd, int quantity)
        {
            var sqlParams = new SqlParameter[]
            {
                accountId.ToSql("accountId"),
                productUid.ToSql("productId"),
                quantity.ToSql("quantity"),
                currency.ToSql("currency"),
                price.ToSql("invoicePrice"),
                priceUsd.ToSql("invoicePriceUsd")
            };

            await ExecuteNonQueryAsync("[accounts].[pProductAddByAccountId]", sqlParams);
        }
    }
}