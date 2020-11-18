namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.ExternalAccount.Core.Context;
    using Shared;

    public static class MultipleEditProductStatus
    {
        public static IEnumerable<KeyValuePair<int, string>> GetProductList()
            => GetProductIds(ExtRequest.Instance.GetGroupId())
            .Select(id => new KeyValuePair<int, string>(id, CacheRef.GetProductName(id)));

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