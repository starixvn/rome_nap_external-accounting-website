namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using RemotingRef;
    using ViewModels;

    public static class MemberModelExtensions
    {
        public static CustomerModel MemberInfo(this CustomerModel model, DataSet ds)
        {
            SetPersonalInfo(model, ds);

            SetMemberSettings(model, ds);

            return model;
        }

        private static void SetPersonalInfo(CustomerModel model, DataSet ds)
        {
            var lineRow = ds.Tables[1].Rows[0];

            model.PersonalInfo.UplineId = new ObjectInfo(lineRow.AgentId().ToString(), "Agent ID");
            model.PersonalInfo.CurrentLevel = new ObjectInfo(lineRow.MemberName(), "Member");

            model.PersonalInfo.Downlines = new List<ObjectInfo>
            {
                new ObjectInfo(lineRow.SuperId().ToString(), lineRow.SuperName(), "Super"),
                new ObjectInfo(lineRow.MasterId().ToString(), lineRow.MasterName(),"Master"),
                new ObjectInfo(lineRow.AgentId().ToString(), lineRow.AgentName(),"Agent")
            };
        }

        private static void SetMemberSettings(CustomerModel model, DataSet ds)
        {
            var table3rdDCS = ds.Tables[4];
            var customerRow = ds.Tables[0].Rows[0];

            model.Setting = new Setting
            {
                EnableItems = EnableItems(customerRow, table3rdDCS),
                SyncItems = SyncItems(customerRow, table3rdDCS),
                EditItems = EditItems(),
                Details = Details()
            };

            model.AddEnableAndDetailProduct(table3rdDCS, 21, "Allbet", eFunctions.AllbetBetting, false);
            model.AddEnableAndDetailProduct(table3rdDCS, 22, "Macau Games", eFunctions.VoidbridgeBetting);
            model.AddEnableAndDetailProduct(table3rdDCS, 23, "NG2", eFunctions.NG2Betting, getFromRemotingRef: true);
            model.AddEnableAndDetailProduct(table3rdDCS, 24, "Fishing World");
            model.AddEnableAndDetailProduct(table3rdDCS, 28, "Lottery", eFunctions.LotteryBetting);
            model.AddEnableAndDetailProduct(table3rdDCS, 26, "Lucky 3");
        }

        private static List<ObjectInfo> EnableItems(DataRow customerRow, DataTable table3rdDCS)
            => new List<ObjectInfo>
            {
                new ObjectInfo("Closed", customerRow.Closed(),"Closed"),
                new ObjectInfo("Suspended",customerRow.Suspended(),"Suspened"),
                new ObjectInfo("Outright", customerRow.EnableOutright(),"Allow Outright"),
                new ObjectInfo("EnableCasino",  customerRow.EnableCasino(),"Enable Casino"),
                new ObjectInfo("EnableNumberGame", customerRow.EnableNumberGame(),"Enable Num Game", "Enable Number Game"),
                new ObjectInfo("EnableLiveCasino", customerRow.EnableLiveCasino(),"Enable Live Casino"),
                new ObjectInfo("EnableVirtualSports", table3rdDCS.GetVirtualSportsStatus(),"Virtual Sports"),
                new ObjectInfo("DoubleComm", customerRow.EnableDoubleComm(),"Double Comm",  "Enable Double Commission"),
                new ObjectInfo("EnableHRFixedOdds",  customerRow.EnableHorseRacingFixedOdds(),"HR Fixed Odds",  "Enable Horse Racing Fixed Odds"),
                new ObjectInfo("EnableESports",customerRow.EnableESports(),"E-Sports",  "Enable E-Sports"),
                new ObjectInfo("EnableColossusBets",  customerRow.EnableColossusBet(),"Colossus Bets", "Enable Colossus Bets" ),
                new ObjectInfo("EnableGoldDeluxe", customerRow.EnableGoldDeluxe(), "Gold Deluxe")
            };

        private static List<ObjectInfo> SyncItems(DataRow customerRow, DataTable table3RdDcs)
            => new List<ObjectInfo>
            {
                new ObjectInfo("SyncCasino", customerRow.IsSyncedCasino(), "Casino Synced"),
                new ObjectInfo("SyncLiveCasino", customerRow.IsSyncedLiveCasino(), "Live Casino Synced"),
                new ObjectInfo("HideAllbet", table3RdDcs.Get3rdProductStatus(21)),
                new ObjectInfo("HideVoidbridge", table3RdDcs.Get3rdProductStatus(22)),
                new ObjectInfo("HideNG2", table3RdDcs.GetNG2ProductStatus()),
                new ObjectInfo("HideFishingWorld", table3RdDcs.Get3rdProductStatus(24)),
                new ObjectInfo("HideLottery", table3RdDcs.Get3rdProductStatus(28)),
                new ObjectInfo("HideLucky3", table3RdDcs.Get3rdProductStatus(26))
            };

        private static List<ObjectInfo> EditItems()
            => new List<ObjectInfo>
            {
                new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                new ObjectInfo { Key = "EnableKeno", Text = "Enable Keno" },
                new ObjectInfo { Key = "EnableCockfighting", Text = "Enable Cockfighting" }
            };

        private static List<ObjectInfo> Details()
            => new List<ObjectInfo>
            {
                new ObjectInfo ("Info", "Edit Info"),
                new ObjectInfo ("PT", "Edit Position Taking"),
                new ObjectInfo ("Racing", "Edit Racing"),
                new ObjectInfo ("Casino", "Edit Casino"),
                new ObjectInfo ("LiveCasino", "Edit Live Casino"),
                new ObjectInfo ("VirtualSports", "Edit Virtual Sports"),
                new ObjectInfo ("Keno", "Edit Keno"),
                new ObjectInfo ("GoldDeluxe", "Edit Gold Deluxe"),
                new ObjectInfo ("ColossusBets", "Edit Colossus Bets"),
                new ObjectInfo ("Sportsbook2", "Edit Sportsbook 2"),
                new ObjectInfo ("Cockfighting", "Edit Cockfighting"),
                new ObjectInfo ("Cricket",  "Edit Cricket" )
            };

        public static CustomerModel GetSuperMasterPtsOnMember(this CustomerModel model)
        {
            model.Setting.SuperPts.Clear();
            model.Setting.MasterPts.Clear();

            foreach (var item in model.Setting.Details)
            {
                if (!IgnoreOrExcludeProducts().Contains(item.Key))
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

        private static IList<string> IgnoreOrExcludeProducts() => new List<string> { "Info", "ColossusBets" };

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