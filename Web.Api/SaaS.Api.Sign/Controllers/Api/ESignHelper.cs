using SaaS.Data.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SaaS.Api.Sign.Controllers.Api
{
    internal static class eSignHelper
    {
        internal static Guid? GetAllowedAccountProductId(List<ViewAccountProduct> products, Guid? accountProductId, out ViewAccountProduct product)
        {
            var sorted = products.OrderByDescending(e => e.PurchaseDate);
            var filter = sorted.Where(e => !e.IsDisabled);

            product =
                filter.FirstOrDefault(e => !e.AllowedEsignCount.HasValue && accountProductId.HasValue && e.AccountProductId == accountProductId.Value) ??
                filter.FirstOrDefault(e => !e.AllowedEsignCount.HasValue && e.IsMinor) ??
                filter.FirstOrDefault(e => e.AllowedEsignCount.HasValue && e.IsMinor && e.AllowedEsignCount > e.UsedEsignCount);

            if (!object.Equals(product, null))
                return product.AccountProductId;

            return null;
        }

        internal static Guid? GetAllowedAccountMicrotransactionId(List<ViewAccountMicrotransaction> microtransactions, out ViewAccountMicrotransaction microtransaction)
        {
            var sorted = microtransactions.OrderByDescending(e => e.PurchaseDate);
            var filter = sorted;

            microtransaction =
                filter.FirstOrDefault(e => !e.AllowedEsignCount.HasValue) ??
                filter.FirstOrDefault(e => e.AllowedEsignCount.HasValue && e.AllowedEsignCount > e.UsedEsignCount);

            if (!object.Equals(microtransaction, null))
                return microtransaction.AccountMicrotransactionId;

            return null;
        }

        internal static void GetAllowedUsedCount(List<ViewAccountProduct> products, List<ViewAccountMicrotransaction> microtransactions, out int? allowed, out int? used)
        {
            allowed = 0;
            used = 0;

            var filterProducts = products.Where(e => !e.IsDisabled && e.AllowedEsignCount.HasValue);
            var filterMicrotransactions = microtransactions.Where(e => e.AllowedEsignCount.HasValue);

            allowed += filterProducts.Sum(product=> product.AllowedEsignCount);
            used += filterProducts.Sum(product => product.UsedEsignCount);

            allowed += filterMicrotransactions.Sum(product => product.AllowedEsignCount);
            used += filterMicrotransactions.Sum(product => product.UsedEsignCount);
        }
    }
}