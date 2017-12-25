namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Core.Shared;
    using RemotingRef;
    using ViewModels;

    public static class MemberModelExtensions
    {
        private static readonly IList<string> IgnoreOrExcludeProducts = new List<string> { "Info", "ColossusBets" };

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
                SyncItems = CustomerModelExtensions.SyncItems(customerRow, table3rdDCS),
                EditItems = CustomerModelExtensions.EditItems(),
                Details = CustomerModelExtensions.Details(Role.Member)
            };

            CustomerModelExtensions.AddDCSProductSettings(model, table3rdDCS);
        }

        public static CustomerModel GetSuperMasterSettingOnMember(this CustomerModel model)
        {
            model.Setting.SuperPts.Clear();
            model.Setting.MasterPts.Clear();

            foreach (var item in model.Setting.Details)
            {
                if (!IgnoreOrExcludeProducts.Contains(item.Key))
                {
                    model.Setting.MasterPts.Add(new ObjectInfo(item.Key, item.Value));
                }
            }

            model.Setting.MasterPts.Add(new ObjectInfo("SBPT", "Sportsbook's PT"));

            model.AddDetailProductForCrossLevel("PT", "Sportsbook");
            model.AddDetailProductForCrossLevel("Racing", "Racing");
            model.AddDetailProductForCrossLevel("Casino", "Casino");
            model.AddDetailProductForCrossLevel("LiveCasino", "Live Casino");
            model.AddDetailProductForCrossLevel("VirtualSports", "Virtual Sports");
            model.AddDetailProductForCrossLevel("Keno", "Keno");
            model.AddDetailProductForCrossLevel("GoldDeluxe", "Gold Deluxe");
            model.AddDetailProductForCrossLevel("Sportsbook2", "Sportsbook 2");
            model.AddDetailProductForCrossLevel("Cockfighting", "Cockfighting");
            model.AddDetailProductForCrossLevel("Cricket", "Cricket");
            model.AddDetailProductForCrossLevel("Allbet", "Allbet");
            model.AddDetailProductForCrossLevel("MacauGames", "Macau Games");
            model.AddDetailProductForCrossLevel("NG2", RefData.GetResourceDataByRefIdAndResourceId("Product", "23", "en-US", "NG2"));
            model.AddDetailProductForCrossLevel("FishingWorld", "Fishing World");
            model.AddDetailProductForCrossLevel("Lottery", "Lottery");
            model.AddDetailProductForCrossLevel("Lucky3", "Lucky 3");

            return model;
        }

        private static CustomerModel AddDetailProductForCrossLevel(this CustomerModel model, string productKey, string productName)
        {
            var product = model
                .Setting
                .MasterPts
                .Where(p => p.Key.Equals(productKey))
                .ToList();

            if (product.Count > 0)
            {
                product[0].Value = productName;
            }

            model.Setting.SuperPts = model.Setting.MasterPts;

            return model;
        }
    }
}