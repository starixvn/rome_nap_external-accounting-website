namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Product = Fanex.ProductStatus.Setting.Configuration.Product;

    public static class MultipleEditProductStatus
    {
        private const byte ProductGroupType = 2;

        private static IDynamicRepository dynamicRepository = new DynamicRepository();  


        public static IEnumerable<Product> GetProductList()
        {
            var criteria = new GetProductListCriteria(ExtRequest.Instance.GetGroupId());
            var productTypeEntities = dynamicRepository.Fetch<ProductTypeEntity>(criteria).ToList();
            var productList = productTypeEntities.Select(p => {
                return new Product(p.ProductId, 
                    p.ProductType == ProductGroupType ? CacheRef.GetProductGroupName(p.ProductId) : CacheRef.GetProductName(p.ProductId),
                    p.ProductType == ProductGroupType);
            });

            return productList;
        }
    }

    public class ProductTypeEntity
    {
        public int ProductType { get; set; }

        public int ProductId { get; set; }
    }
}