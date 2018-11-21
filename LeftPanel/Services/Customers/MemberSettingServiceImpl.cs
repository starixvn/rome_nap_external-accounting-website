namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Core.Authorization;
    using Models;
    using ViewModels;

    public class MemberSettingServiceImpl : CustomerSettingServiceImpl
    {
        private static readonly IList<string> IgnoredSettingsWhenEditMember =
            new List<string> { "Info", "ColossusBets", "AddNew", "Transfer" };

        public MemberSettingServiceImpl(
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

            foreach (var detailLink in setting.Details)
            {
                if (!IgnoredSettingsWhenEditMember.Contains(detailLink.Key))
                {
                    setting.MasterPts.Add(
                        new ObjectInfo(detailLink.Key, detailLink.Value.ToString().Replace("Edit ", string.Empty)));
                }
            }

            setting.MasterPts.Add(new ObjectInfo("SBPT", "Sportsbook's PT"));

            SetEditSportsbookTextForIndirectUplineEditMember(setting.MasterPts);

            // Set edit links for super same as master edit member
            setting.SuperPts = setting.MasterPts;

            return setting;
        }

        private static void SetEditSportsbookTextForIndirectUplineEditMember(List<ObjectInfo> masterPts)
        {
            var ptItem = masterPts.Find(item => item.Key == "PT");
            if (ptItem != null)
            {
                ptItem.Value = "Sportsbook";
            }
        }
    }
}