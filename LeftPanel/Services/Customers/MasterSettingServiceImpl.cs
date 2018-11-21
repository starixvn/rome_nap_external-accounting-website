namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using Core.Authorization;
    using Extensions;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using ViewModels;

    public class MasterSettingServiceImpl : CustomerSettingServiceImpl
    {
        public MasterSettingServiceImpl(
            int userId,
            int roleId,
            string site,
            IEnumerable<ProductSettingElement> productSettingElements = null,
            AuthorizationService authorizationService = null,
            Func<DataRow, DataTable, List<ObjectInfo>> hiddenAndSyncItemsCreator = null)
            : base(userId, roleId, site, productSettingElements, authorizationService, hiddenAndSyncItemsCreator)
        { }

        public override Setting GetSetting(DataSet customerStatusInfoDataset)
        {
            var setting = base.GetSetting(customerStatusInfoDataset);

            var nonProductStatusRow = customerStatusInfoDataset.Tables[0].Rows[0];
            setting.EnableItems.Insert(2, new ObjectInfo("AutoPT", nonProductStatusRow.EnableAutoMasterPT(), "Allow Auto PT"));

            return setting;
        }
    }
}