namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using Core.Authorization;
    using Extensions;
    using Fanex.ExternalAccount.Core.Context;
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
            IEnumerable<ProductSettingElement> productSettingElements = null,
            IAuthorizationService authorizationService = null,
            CurrentSession currentSession = null,
            Func<DataTable, List<ObjectInfo>> hiddenAndSyncItemsCreator = null)
            : base(userId, roleId, productSettingElements, authorizationService, currentSession, hiddenAndSyncItemsCreator)
        { }

        public override Setting GetSetting(DataSet customerStatusInfoDataset)
        {
            var setting = base.GetSetting(customerStatusInfoDataset);

            var nonProductStatusRow = customerStatusInfoDataset.Tables[0].Rows[0];
#pragma warning disable S109 // Magic numbers should not be used
            setting.EnableItems.Insert(2, new ObjectInfo("AutoPT", nonProductStatusRow.EnableAutoMasterPT(), "Allow Auto PT"));
#pragma warning restore S109 // Magic numbers should not be used

            return setting;
        }
    }
}