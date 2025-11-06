namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    // using Fanex.Data;
    // using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    // Stub implementation for missing ProductStatus library
    public class Product
    {
        public Product(int productId, string productName, byte productType)
        {
            ProductId = productId;
            ProductName = productName;
            ProductType = productType;
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public byte ProductType { get; set; }
    }

    // Stub interface for missing Fanex.Data dependency
    public interface IDynamicRepository
    {
        IEnumerable<T> Fetch<T>(object criteria);
    }

    public class DynamicRepository : IDynamicRepository
    {
        public IEnumerable<T> Fetch<T>(object criteria)
        {
            // Stub implementation - return empty list
            return new List<T>();
        }
    }

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
                    (byte)(p.ProductType == ProductGroupType ? 1 : 0));
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