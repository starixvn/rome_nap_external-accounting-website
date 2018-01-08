namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Core.Shared;
    using Extensions;
    using ViewModels;

    public class MemberInfoQueryServiceImpl : CustomerInfoQueryBaseService
    {
        private static readonly IList<string> CrossLineIgnoreProducts = new List<string> { "Info", "ColossusBets", "AddNew", "Transfer" };

        internal override void SetLineInfo(PersonalInfo personalInfo, DataRow lineRow)
        {
            personalInfo.UplineId = new ObjectInfo { Value = lineRow.AgentId(), Text = "Agent ID" };
            personalInfo.CurrentLevel = new ObjectInfo { Value = lineRow.MemberName(), Text = "Member" };
            personalInfo.Downlines = GetDownlines(lineRow, Role.Member);
        }

        internal override void SetCustomSettingByLevel(Setting setting, int userId, DataSet dataSet)
        {
            setting.SuperPts.Clear();
            setting.MasterPts.Clear();

            foreach (var item in setting.Details)
            {
                if (!CrossLineIgnoreProducts.Contains(item.Key))
                {
                    setting.MasterPts.Add(new ObjectInfo(item.Key, item.Value.ToString().Replace("Edit ", string.Empty)));
                }
            }

            setting.MasterPts.Add(new ObjectInfo("SBPT", "Sportsbook's PT"));
            SetTextForIndirectUplineEditMember(setting);

            setting.SuperPts = setting.MasterPts;
        }

        private static void SetTextForIndirectUplineEditMember(Setting setting)
        {
            var ptItem = setting.MasterPts.FirstOrDefault(item => item.Key == "PT");
            if (ptItem != null)
            {
                ptItem.Value = "Sportsbook";
            }
        }
    }
}