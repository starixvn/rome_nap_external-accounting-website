namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System.Collections.Generic;
    using System.Data;
    using ViewModels;

    public class MemberSettingServiceImpl : SettingServiceImpl
    {
        private static readonly IList<string> IgnoredSettingsWhenEditMember =
            new List<string> { "Info", "ColossusBets", "AddNew", "Transfer" };

        public override Setting GetSetting(
            int actorId,
            string custName,
            int uplineId,
            string groupName,
            DataSet customerInfoDataset)
        {
            var setting = base.GetSetting(actorId, custName, uplineId, groupName, customerInfoDataset);

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