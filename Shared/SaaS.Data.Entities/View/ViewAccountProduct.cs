using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS.Data.Entities.View
{
    public class ViewAccountProduct : ViewProduct
    {
        [NotMapped]
        public List<ViewAccountProductModule> Modules { get; set; }

        public string ProductVersion { get; set; }

        [NotMapped]
        public ushort? Order { get; set; }
    }

    public static class ViewAccountProductHelper
    {
        public static Version GetProductVersion(this ViewAccountProduct product)
        {
            Version version;
            if (!Version.TryParse(product.ProductVersion, out version))
                return null;

            return version;
        }
    }
}