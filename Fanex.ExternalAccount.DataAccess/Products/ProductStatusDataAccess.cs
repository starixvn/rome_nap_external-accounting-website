namespace Fanex.ExternalAccount.DataAccess.Products
{
    using System.Collections;
    using System.Collections.Generic;
    using Data;
    using DTO.Products;
    using Fanex.Common.Extensions;
    using Fanex.ExternalAccount.Utilities;

    public interface ProductStatusDataAccess
    {
        List<ProductStatusDto> GetProductStatusList(int custId, int roleId, byte settingStatus);
    }

    public class ProductStatusDataAccessImpl : ProductStatusDataAccess
    {
        public List<ProductStatusDto> GetProductStatusList(int custId, int roleId, byte settingStatus)
        {
            var param = new Hashtable
            {
                { "custid", custId },
                { "roleid", roleId },
                { "settingstatus", settingStatus }
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
                        IsDisabledUpline = reader.ToBoolean("IsDisabledUpline"),
                        IsHidden = reader.ToBoolean("IsHidden"),
                        HasDownline = reader.ToBoolean("HasDownline"),
                        IsDisabled = reader.ToBoolean("IsDisabled"),
                        IsDisabledDownline = reader.ToBoolean("IsDisabledDownline"),
                        IsHiddenUpline = reader.ToBoolean("IsHiddenUpline"),
                        GroupId = reader.IsDBNull(reader.GetOrdinal("GroupId")) ? (int?)null : reader.ToInt32("GroupId")
                    });
                }

                return productStatusList;
            }
        }
    }
}