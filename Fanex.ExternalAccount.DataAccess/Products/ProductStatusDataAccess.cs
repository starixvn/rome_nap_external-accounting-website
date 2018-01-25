namespace Fanex.ExternalAccount.DataAccess.Products
{
    using System.Collections;
    using System.Collections.Generic;
    using Data;
    using Fanex.ExternalAccount.Utilities;
    using Core.Shared;

    public interface ProductStatusDataAccess
    {
        List<ProductStatusDto> GetProductStatusList(int custId, int roleId);
    }

    public class ProductStatusDataAccessImpl : ProductStatusDataAccess
    {
        public List<ProductStatusDto> GetProductStatusList(int custId, int roleId)
        {
            var param = new Hashtable
            {
                { "custid", custId },
                { "roleid", roleId }
            };
            var productStatusList = new List<ProductStatusDto>();

            // Cannot use dynamic mapping here due to old fanex.data
            using (var db = new ObjectDb("Acc_CustSetting_StatusSettingSelect"))
            {
                var reader = db.ExecuteReader(param);

                while (reader.Read())
                {
                    productStatusList.Add(new ProductStatusDto
                    {
                        ProductId = reader.ToInt32("ProductID"),
                        ProductName = reader.ToString("ProductName"),
                        IsDisabledUpline = reader.ToBoolean("IsDisabledUpline"),
                        IsSyncedUpline = reader.ToBoolean("IsSyncedUpline"),
                        IsHidden = reader.ToBoolean("IsHidden"),
                        HasDownline = reader.ToBoolean("HasDownline"),
                        IsSynced = reader.ToBoolean("IsSync"),
                        IsDisabled = reader.ToBoolean("IsDisabled"),
                        IsDisabledDownline = reader.ToBoolean("IsDisabledDownline"),
                        IsHiddenUpline = reader.ToBoolean("IsHiddenUpline")
                    });
                }

                return productStatusList;
            }
        }
    }
}