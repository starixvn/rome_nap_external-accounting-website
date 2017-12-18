namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Utilities;
    using ViewModels;

    public static class AgentModelExtensions
    {
        public static CustomerModel AgentInfo(this CustomerModel model, DataSet ds)
        {
            SetAgentBalanceInfo(model, ds);

            SetAgentPersonalInfo(model, ds);

            model.AgentSettings(ds);

            return model;
        }

        private static void SetAgentPersonalInfo(CustomerModel model, DataSet ds)
        {
            var lineRow = ds.Tables[1].Rows[0];

            model.PersonalInfo.UplineId = new ObjectInfo { Value = lineRow.MasterId(), Text = "Master ID" };
            model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = lineRow.AgentName(), Text = "Agent" };
            model.PersonalInfo.Downlines = new List<ObjectInfo>
            {
                new ObjectInfo(lineRow.SuperId().ToString(), lineRow.SuperName(),  "Super"),
                new ObjectInfo(lineRow.MasterId().ToString(), lineRow.MasterName(),  "Master"),
                new ObjectInfo(lineRow.AgentId().ToString(), lineRow.AgentName(),  "Agent")
            };
        }

        private static void SetAgentBalanceInfo(CustomerModel model, DataSet ds)
        {
            var cashRow = ds.Tables[2].Rows[0];

            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalAgentBalance(),
                Text = "Total Balance"
            });
            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalAgentYesterdayCash(),
                Text = "Yesterday Balance"
            });
            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.MemberOutstanding(),
                Text = "Member Outstanding"
            });
        }

        private static CustomerModel AgentSettings(this CustomerModel model, DataSet ds)
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

            model.AddEnableAndDetailProduct(table3rdDCS, 21, "Allbet", eFunctions.AllbetBetting, false);
            model.AddEnableAndDetailProduct(table3rdDCS, 22, "Macau Games", eFunctions.VoidbridgeBetting);
            model.AddEnableAndDetailProduct(table3rdDCS, 23, "NG2", eFunctions.NG2Betting, true, true);
            model.AddEnableAndDetailProduct(table3rdDCS, 24, "Fishing World");
            model.AddEnableAndDetailProduct(table3rdDCS, 28, "Lottery", eFunctions.LotteryBetting);
            model.AddEnableAndDetailProduct(table3rdDCS, 26, "Lucky 3");

            model.Setting.EnableItems.Add(new ObjectInfo
            {
                Key = "EnableGoldDeluxe",
                Value = !Utils.ToBoolean(customerRow["DisableGoldDeluxe"]),
                Text = "Gold Deluxe"
            });

            return model;
        }

        private static List<ObjectInfo> Details() => new List<ObjectInfo>
        {
            new ObjectInfo("Info","Edit Info"),
            new ObjectInfo("PT","Edit Position Taking"),
            new ObjectInfo("AddNew","Add New"),
            new ObjectInfo("Transfer","Edit Transfer Condition"),
            new ObjectInfo("Racing","Edit Racing"),
            new ObjectInfo("Casino","Edit Casino"),
            new ObjectInfo("LiveCasino","Edit Live Casino"),
            new ObjectInfo("VirtualSports","Edit Virtual Sports"),
            new ObjectInfo("Keno","Edit Keno"),
            new ObjectInfo("GoldDeluxe","Edit Gold Deluxe"),
            new ObjectInfo("ColossusBets","Edit Colossus Bets"),
            new ObjectInfo("Sportsbook2","Edit Sportsbook 2"),
            new ObjectInfo("Cockfighting","Edit Cockfighting"),
            new ObjectInfo("Cricket","Edit Cricket" )
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

        private static List<ObjectInfo> SyncItems(DataRow customerRow, DataTable table3rdDCS)
        {
            return new List<ObjectInfo>
            {
                new ObjectInfo { Key = "SyncCasino", Value = Utils.ToBoolean(customerRow["syncCasino"]), Text = "Casino Synced" },
                new ObjectInfo { Key = "SyncLiveCasino", Value = Utils.ToBoolean(customerRow["syncLiveCasino"]), Text = "Live Casino Synced" },
                new ObjectInfo
                {
                    Key = "HideAllbet",
                    Value = table3rdDCS.Select("ProductID = 21").Length<=0
                            || (Utils.ToBoolean(table3rdDCS.Select("ProductID = 21").FirstOrDefault()?["IsHidden"])
                                || !Utils.ToBoolean(table3rdDCS.Select("ProductID = 21").FirstOrDefault()?["IsSync"]))                             },
                new ObjectInfo
                {
                    Key = "HideVoidbridge",
                    Value = table3rdDCS.Select("ProductID = 22").Length<=0
                            || (Utils.ToBoolean(table3rdDCS.Select("ProductID = 22").FirstOrDefault()?["IsHidden"])
                                || !Utils.ToBoolean(table3rdDCS.Select("ProductID = 22").FirstOrDefault()?["IsSync"]))                             },
                new ObjectInfo
                {
                    Key = "HideNG2",
                    Value = table3rdDCS.Select("ProductID = 23").Length<=0
                            || !Utils.ToBoolean(table3rdDCS.Select("ProductID = 23").FirstOrDefault()?["IsSync"])                             },
                new ObjectInfo
                {
                    Key = "HideFishingWorld",
                    Value = table3rdDCS.Select("ProductID = 24").Length<=0
                            || (Utils.ToBoolean(table3rdDCS.Select("ProductID = 24").FirstOrDefault()?["IsHidden"])
                                || !Utils.ToBoolean(table3rdDCS.Select("ProductID = 24").FirstOrDefault()?["IsSync"]))                             },
                new ObjectInfo
                {
                    Key = "HideLottery",
                    Value = table3rdDCS.Select("ProductID = 28").Length<=0
                            || (Utils.ToBoolean(table3rdDCS.Select("ProductID = 28").FirstOrDefault()?["IsHidden"])
                                || !Utils.ToBoolean(table3rdDCS.Select("ProductID = 28").FirstOrDefault()?["IsSync"]))                             },
                new ObjectInfo
                {
                    Key = "HideLucky3",
                    Value = table3rdDCS.Select("ProductID = 26").Length<=0
                            || (Utils.ToBoolean(table3rdDCS.Select("ProductID = 26").FirstOrDefault()?["IsHidden"])
                                || !Utils.ToBoolean(table3rdDCS.Select("ProductID = 26").FirstOrDefault()?["IsSync"]))                             }
            };
        }

        private static List<ObjectInfo> EnableItems(DataRow customerRow, DataTable table3rdDCS)
        {
            return new List<ObjectInfo>
            {
                new ObjectInfo { Key = "Closed", Value = Utils.ToBoolean(customerRow["closed"]), Text = "Closed" },
                new ObjectInfo { Key = "Suspended", Value = Utils.ToBoolean(customerRow["suspended"]), Text = "Suspened" },
                new ObjectInfo { Key = "Outright", Value = !Utils.ToBoolean(customerRow["disableOutright"]), Text = "Allow Outright" },
                new ObjectInfo { Key = "EnableCasino", Value = !Utils.ToBoolean(customerRow["disableCasino"]), Text = "Enable Casino" },
                new ObjectInfo
                {
                    Key = "EnableNumberGame", Value = !Utils.ToBoolean(customerRow["disableNumber"]), Text = "Enable Num Game", Tooltip = "Enable Number Game"
                },
                new ObjectInfo { Key = "EnableLiveCasino", Value = !Utils.ToBoolean(customerRow["disableLiveCasino"]), Text = "Enable Live Casino" },
                new ObjectInfo { Key = "EnableVirtualSports", Value = GetVirtualSportsStatus(table3rdDCS), Text = "Virtual Sports" },
                new ObjectInfo
                {
                    Key = "DoubleComm", Value = Utils.ToBoolean(customerRow["doubleCommLosing"]), Text = "Double Comm", Tooltip = "Enable Double Commission"
                },
                new ObjectInfo
                {
                    Key = "EnableHRFixedOdds", Value = !customerRow.ToBoolean("DisableHRFixedOdds"), Text = "HR Fixed Odds", Tooltip = "Enable Horse Racing Fixed Odds"
                },
                new ObjectInfo { Key = "EnableESports", Value = !Utils.ToBoolean(customerRow["disableESports"]), Text = "E-Sports", Tooltip = "Enable E-Sports" },
                new ObjectInfo
                {
                    Key = "EnableColossusBets",
                    Value = !Utils.ToBoolean(customerRow["DisableColossusBet"]),
                    Text = "Colossus Bets",
                    Tooltip = "Enable Colossus Bets"
                }
            };
        }

        private static bool GetVirtualSportsStatus(DataTable table3rdDCS)
        {
            DataRow[] drowVirtualSports = table3rdDCS.Select("ProductID = 5");

            return drowVirtualSports.Length > 0 && !Utils.ToBoolean(drowVirtualSports[0]["isDisabled"]);
        }
    }
}