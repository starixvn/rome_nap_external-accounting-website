namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Service.Products;
    using Fanex.ExternalAccount.Utilities;
    using Shared;

    public static class MultipleEditProductStatus
    {
        public static IEnumerable<KeyValuePair<int, string>> GetProductList()
        {
            var specialProductIds = Config.SpecialProductIds();
            

            IProductService productService = new ProductService();
            var adminDoesNotHavePermissionOnSpecialProduct = productService.AdminDoesNotHavePermissionOnSpecialProduct(CurrentSession.Instance.CustId);

            var productIds = GetProductIds(ExtRequest.Instance.GetGroupId());

            foreach (var id in productIds)
            {
                if (specialProductIds.ContainsKey(id) && adminDoesNotHavePermissionOnSpecialProduct)
                {
                    continue;
                }

                yield return new KeyValuePair<int, string>(id, CacheRef.GetProductName(id));
            }
        }

        private static IEnumerable<int> GetProductIds(int groupId)
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
                            yield return Convert.ToInt32(reader["productid"]);
                        }
                    }
                }
            }
        }
    }
}