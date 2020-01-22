using SaaS.Data.Entities.View;
using System.Collections.Generic;

namespace SaaS.Api.Models.Products
{
    public static class ProductComparer
    {
        public static int Comparer(ViewAccountProduct p1, ViewAccountProduct p2)
        {
            if (object.Equals(p1, null) && object.Equals(p2, null)) return 0;
            if (object.Equals(p1, null)) return -1;
            if (object.Equals(p2, null)) return 1;

            if (p1.IsDisabled && p2.IsDisabled) return ProductEndDateComparer(p1, p2);

            if (p1.IsDisabled) return -1;
            if (p2.IsDisabled) return 1;

            if (!p1.IsActive && p2.IsActive) return -1;
            if (p1.IsActive && !p2.IsActive) return 1;

            if (p1.IsPPC && p2.IsPPC) return ProductEndDateComparer(p1, p2);
            if (p1.IsPPC && !p2.IsPPC) return -1;
            if (!p1.IsPPC && p2.IsPPC) return 1;

            if (p1.IsMinor && p2.IsMinor) return ProductEndDateComparer(p1, p2);
            if (p1.IsMinor && !p2.IsMinor) return -1;
            if (!p1.IsMinor && p2.IsMinor) return 1;

            var count1 = object.Equals(p1.Modules, null) ? 0 : p1.Modules.Count;
            var count2 = object.Equals(p2.Modules, null) ? 0 : p2.Modules.Count;

            if (count1 == count2) return ProductEndDateComparer(p1, p2);

            return count1.CompareTo(count2);
        }
        public static int OrderComparer(ViewAccountProduct p1, ViewAccountProduct p2)
        {
            if (object.Equals(p1, null) && object.Equals(p2, null)) return 0;
            if (object.Equals(p1, null)) return -1;
            if (object.Equals(p2, null)) return 1;

            if (p1.IsDisabled && p2.IsDisabled) return ProductEndDateComparer(p1, p2);

            if (p1.IsDisabled) return -1;
            if (p2.IsDisabled) return 1;

            if (!p1.IsActive && p2.IsActive) return -1;
            if (p1.IsActive && !p2.IsActive) return 1;

            if ((p1.IsRenewal && !p1.NextRebillDate.HasValue) && (p2.IsRenewal && !p2.NextRebillDate.HasValue)) return ProductEndDateComparer(p1, p2);
            if ((p1.IsRenewal && !p1.NextRebillDate.HasValue) && !(p2.IsRenewal && !p2.NextRebillDate.HasValue)) return 1;
            if (!(p1.IsRenewal && !p1.NextRebillDate.HasValue) && (p2.IsRenewal && !p2.NextRebillDate.HasValue)) return -1;

            if (p1.IsMinor && p2.IsMinor) return ProductEndDateComparer(p1, p2);
            if (p1.IsMinor && !p2.IsMinor) return -1;
            if (!p1.IsMinor && p2.IsMinor) return 1;

            if (p1.IsPPC && !p2.IsPPC) return -1;
            if (!p1.IsPPC && p2.IsPPC) return 1;

            var count1 = object.Equals(p1.Modules, null) ? 0 : p1.Modules.Count;
            var count2 = object.Equals(p2.Modules, null) ? 0 : p2.Modules.Count;

            if (count1 == count2) return ProductEndDateComparer(p1, p2);

            return count1.CompareTo(count2);
        }

        public static void ProductOrderer(IEnumerable<ViewAccountProduct> products)
        {
            var source = new List<ViewAccountProduct>(products);
            source.Sort(OrderComparer);
            source.Reverse();

            ushort order = 0;
            source.ForEach(e => { e.Order = order++; });
        }

        public static int ProductEndDateComparer(ViewAccountProduct p1, ViewAccountProduct p2)
        {
            if (p1.EndDate.HasValue && p2.EndDate.HasValue)
                return p1.EndDate.Value.CompareTo(p2.EndDate.Value) * -1;

            return 0;
        }
    }
}
