using AutoMapper;
using SaaS.Data.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SaaS.Api.Models.Products
{
    public static class ProductConvertor
    {
        public static AccountProductViewModel AccountProductConvertor(ViewAccountProduct product)
        {
            var newProduct = new AccountProductViewModel();

            newProduct = Mapper.Map(product, newProduct);

            foreach (var module in newProduct.Modules.Where(e => "e-sign".Equals(e.Module, StringComparison.InvariantCultureIgnoreCase)))
            {
                module.Allowed = product.AllowedEsignCount;
                module.Used = product.UsedEsignCount;
            }

            return newProduct;
        }
        public static OwnerProductViewModel OwnerAccountProductConvertor(ViewOwnerProduct product)
        {
            var newProduct = new OwnerProductViewModel();

            newProduct = Mapper.Map(product, newProduct);
            newProduct.Modules = new List<AccountProductModuleModel>(); //create fake modules

            if (product.AllowedEsignCount.GetValueOrDefault(int.MaxValue) > 0)
            {
                newProduct.Modules.Add(new AccountProductModuleModel
                {
                    Module = "e-sign",
                    Allowed = product.AllowedEsignCount,
                    Used = product.UsedEsignCount
                });
            }

            return newProduct;
        }

        public static List<OwnerProductViewModel> OwnerAccountProductConvertor(List<ViewOwnerProduct> products)
        {
            return products.ConvertAll(OwnerAccountProductConvertor);
        }
    }
}
