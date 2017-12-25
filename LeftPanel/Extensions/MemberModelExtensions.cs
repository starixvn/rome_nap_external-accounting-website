namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Core.Shared;
    using ViewModels;

    public static class MemberModelExtensions
    {
        private static readonly IList<string> CrossLineIgnoreProducts = new List<string> { "Info", "ColossusBets", "AddNew", "Transfer" };

        public static CustomerModel MemberInfo(this CustomerModel model, DataSet ds)
        {
            SetPersonalInfo(model, ds);

            SetMemberSettings(model, ds);

            return model;
        }

        private static void SetPersonalInfo(CustomerModel model, DataSet ds)
        {
            var lineRow = ds.Tables[1].Rows[0];

            model.PersonalInfo.UplineId = new ObjectInfo { Value = lineRow.AgentId(), Text = "Agent ID" };
            model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = lineRow.MemberName(), Text = "Member" };
            model.PersonalInfo.Downlines = CustomerModelExtensions.GetDownlines(lineRow, Role.Member);
        }

        private static void SetMemberSettings(CustomerModel model, DataSet ds)
        {
            var table3rdDCS = ds.Tables[4];
            var customerRow = ds.Tables[0].Rows[0];

            model.Setting = new Setting
            {
                EnableItems = CustomerModelExtensions.EnableItems(customerRow, table3rdDCS),
                SyncItems = LeftPanelProductIntegration.CreateSyncItems(customerRow, table3rdDCS),
                EditItems = CustomerModelExtensions.EditItems(),
                Details = CustomerModelExtensions.Details(Role.Member)
            };

            model.AddDefaultMinMaxPTSetting();

            model.AddProductSettings(table3rdDCS);

            model.GetSuperMasterSettingOnMember();
        }

        public static CustomerModel GetSuperMasterSettingOnMember(this CustomerModel model)
        {
            model.Setting.SuperPts.Clear();
            model.Setting.MasterPts.Clear();

            foreach (var item in model.Setting.Details)
            {
                if (!CrossLineIgnoreProducts.Contains(item.Key))
                {
                    model.Setting.MasterPts.Add(new ObjectInfo(item.Key, item.Value.ToString().Replace("Edit ", string.Empty)));
                }
            }

            model.Setting.MasterPts.Add(new ObjectInfo("SBPT", "Sportsbook's PT"));
            SetTextForIndirectUplineEditMember(model);

            model.Setting.SuperPts = model.Setting.MasterPts;

            return model;
        }

        private static void SetTextForIndirectUplineEditMember(CustomerModel model)
        {
            var ptItem = model.Setting.MasterPts.FirstOrDefault(item => item.Key == "PT");
            if (ptItem != null)
            {
                ptItem.Value = "Sportsbook";
            }
        }
    }
}