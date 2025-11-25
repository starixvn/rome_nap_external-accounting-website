namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using Core.Authorization;
    using Fanex.ExternalAccount.Core.Context;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using ViewModels;

    public class MemberSettingServiceImpl : CustomerSettingServiceImpl
    {
        private static readonly IList<string> IgnoredSettingsWhenEditMember =
            new List<string> { "Info", "AddNew", "Transfer", "ParlayMaxPayout" };

        public MemberSettingServiceImpl(
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

            foreach (var detailLink in setting.Details)
            {
                if (!IgnoredSettingsWhenEditMember.Contains(detailLink.Key))
                {
                    setting.MasterPts.Add(
                        new ObjectInfo(detailLink.Key, detailLink.Value.ToString().Replace("Edit ", string.Empty)));
                }
            }

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