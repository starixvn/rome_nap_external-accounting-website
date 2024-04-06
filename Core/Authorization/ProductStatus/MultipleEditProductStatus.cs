namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.ExternalAccount.Core.Context;
    using Shared;

    public static class MultipleEditProductStatus
    {
        private const byte ProductGroupType = 2;

        private class Product
        {
            public Product(int id, byte type)
            {
                Id = id;
                Type = type;
            }

            public int Id { get; }

            public byte Type { get; }
        }

        public static IEnumerable<KeyValuePair<int, string>> GetProductList()
        {
            var products = GetProductIds(ExtRequest.Instance.GetGroupId());

            foreach (var product in products)
            {
                yield return new KeyValuePair<int, string>(
                    product.Id,
                    product.Type == ProductGroupType ? CacheRef.GetProductGroupName(product.Id) : CacheRef.GetProductName(product.Id));
            }
        }

        private static IEnumerable<Product> GetProductIds(int groupId)
        {
            var criteria = new GetProductListCriteria(groupId);

            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                    {
                        while (reader.Read())
                        {
                            yield return new Product(Convert.ToInt32(reader["productid"]), Convert.ToByte(reader["product_type"]));
                        }
                    }
                }
            }
        }
    }
}