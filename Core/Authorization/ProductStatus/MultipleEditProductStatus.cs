//namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
//{
//    using System;
//    using System.Collections.Generic;
//    using Fanex.Data;
//    using Fanex.ExternalAccount.Core.Context;
//    using Fanex.ExternalAccount.Core.Shared;
//    using Product = Fanex.ProductStatus.Setting.Configuration.Product;

//    public static class MultipleEditProductStatus
//    {
//        private const byte ProductGroupType = 2;

//        public static IEnumerable<Product> GetProductList()
//        {
//            var criteria = new GetProductListCriteria(ExtRequest.Instance.GetGroupId());

//            if (criteria.IsValid())
//            {
//                using (var db = new ObjectDb(criteria.GetSettingKey()))
//                {
//                    using (var reader = db.ExecuteReader(criteria.GetSpParams()))
//                    {
//                        while (reader.Read())
//                        {
//                            var productType = Convert.ToByte(reader["product_type"]);
//                            var productId = Convert.ToInt32(reader["productid"]);
//                            yield return new Product(
//                                productId,
//                                productType == ProductGroupType ? CacheRef.GetProductGroupName(productId) : CacheRef.GetProductName(productId),
//                                productType == ProductGroupType);
//                        }
//                    }
//                }
//            }
//        }
//    }
//}