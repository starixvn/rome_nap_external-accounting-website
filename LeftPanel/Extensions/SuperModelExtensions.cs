namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using CustomerSetting.Security.Authenticate;
    using ViewModels;
    using static eFunctions;

    public static class SuperModelExtensions
    {
        public static CustomerModel SuperInfo(this CustomerModel model, DataSet ds, int actorId)
        {
            SetSuperBalanceInfo(model, ds);

            SetSuperPersonalInfo(model, ds);

            SetSuperSettings(model, ds, actorId);

            return model;
        }

        private static void SetSuperPersonalInfo(CustomerModel model, DataSet ds)
        {
            var lineRow = ds.Tables[1].Rows[0];

            model.PersonalInfo.Downlines = new List<ObjectInfo>
            {
                new ObjectInfo(lineRow.SuperId().ToString(), lineRow.SuperName(), "Super")
            };

            model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = lineRow.SuperName(), Text = "Super" };
        }

        private static void SetSuperBalanceInfo(CustomerModel model, DataSet ds)
        {
            var cashRow = ds.Tables[2].Rows[0];

            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalSuperBalance(),
                Text = "Total Balance"
            });

            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalSuperYesterdayCash(),
                Text = "Yesterday Balance"
            });

            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.CustomerOutstanding(),
                Text = "Master Outstanding"
            });
        }

        private static void SetSuperSettings(CustomerModel model, DataSet ds, int userId)
        {
            var tableCustomer = ds.Tables[0];
            var table3rdDCS = ds.Tables[4];
            var customerRow = tableCustomer.Rows[0];

            model.Setting = new Setting
            {
                EnableItems = EnableItems(customerRow, table3rdDCS),
                SyncItems = SyncItems(customerRow, table3rdDCS),
                EditItems = EditItems(),
                Details = Details()
            };

            model.AddProductSetting(table3rdDCS, 21, "Allbet", false);
            model.AddProductSetting(table3rdDCS, 22, "Macau Games");
            model.AddProductSetting(table3rdDCS, 23, "NG2", getFromRemotingRef: true);
            model.AddProductSetting(table3rdDCS, 24, "Fishing World");
            model.AddProductSetting(table3rdDCS, 28, "Lottery");
            model.AddProductSetting(table3rdDCS, 26, "Lucky 3");

            AddBettingLimitForSuper(model, userId);
        }

        private static List<ObjectInfo> EnableItems(DataRow customerRow, DataTable table3rdDCS)
            => new List<ObjectInfo>
            {
                new ObjectInfo("Closed", customerRow.Closed(), "Closed"),
                new ObjectInfo("Suspended", customerRow.Suspended(), "Suspened"),
                new ObjectInfo("Outright", customerRow.EnableOutright(), "Allow Outright"),
                new ObjectInfo("EnableCasino", customerRow.EnableCasino(), "Enable Casino"),
                new ObjectInfo("EnableNumberGame", customerRow.EnableNumberGame(), "Enable Num Game", "Enable Number Game"),
                new ObjectInfo("EnableLiveCasino", customerRow.EnableLiveCasino(), "Enable Live Casino"),
                new ObjectInfo("EnableVirtualSports", table3rdDCS.GetVirtualSportsStatus(), "Virtual Sports"),
                new ObjectInfo("DoubleComm", customerRow.EnableDoubleComm(), "Double Comm", "Enable Double Commission"),
                new ObjectInfo("EnableHRFixedOdds", customerRow.EnableHorseRacingFixedOdds(), "HR Fixed Odds", "Enable Horse Racing Fixed Odds"),
                new ObjectInfo("EnableESports", customerRow.EnableESports(), "E-Sports", "Enable E-Sports"),
                new ObjectInfo("EnableColossusBets", customerRow.EnableColossusBet(), "Colossus Bets", "Enable Colossus Bets"),
                new ObjectInfo("EnableGoldDeluxe", customerRow.EnableGoldDeluxe(), "Gold Deluxe")
            };

        private static List<ObjectInfo> EditItems()
        {
            return new List<ObjectInfo>
            {
                new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                new ObjectInfo { Key = "EnableKeno", Text = "Enable Keno" },
                new ObjectInfo { Key = "EnableCockfighting", Text = "Enable Cockfighting" }
            };
        }

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

        private static List<ObjectInfo> Details()
            => new List<ObjectInfo>
            {
                new ObjectInfo("Info", "Edit Info"),
                new ObjectInfo("PT", "Edit Position Taking"), // Todo: noted
                new ObjectInfo("AddNew", "Add New"), // Todo: noted
                new ObjectInfo("Transfer", "Edit Transfer Condition"),
                new ObjectInfo("Racing", "Edit Racing"),
                new ObjectInfo("Casino", "Edit Casino"),
                new ObjectInfo("LiveCasino", "Edit Live Casino"),
                new ObjectInfo("VirtualSports", "Edit Virtual Sports"),
                new ObjectInfo("Keno", "Edit Keno"),
                new ObjectInfo("GoldDeluxe", "Edit Gold Deluxe"),
                new ObjectInfo("ColossusBets", "Edit Colossus Bets"),
                new ObjectInfo("Sportsbook2", "Edit Sportsbook 2"),
                new ObjectInfo("Cockfighting", "Edit Cockfighting"),
                new ObjectInfo("Cricket", "Edit Cricket")
            };

        private static readonly List<string> BetLimitSettingProductKeys = new List<string>
        {
            CasinoBetting,
            GoldDeluxeBetting,
            KenoBetting,
            LiveCasinoBetting,
            RacingBetting,
            SportsbookBetting,
            VirtualSportsBetting,
            AllbetBetting,
            VoidbridgeBetting,
            NG2Betting,
            LotteryBetting
        };

        private static void AddBettingLimitForSuper(CustomerModel model, int actorId)
        {
            var showBetSetting = BetLimitSettingProductKeys.Any(productKey =>
                 CustomerModelExtensions.IsAuthorized(actorId, productKey, AuthenticateAction.AuthenticationType.Read));

            if (showBetSetting)
            {
                model.Setting.Details.Add(new ObjectInfo("BetSetting", "Edit Betting Limit"));
            }
        }
    }
}