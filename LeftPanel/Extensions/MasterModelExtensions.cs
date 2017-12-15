    




namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Utilities;
    using ViewModels;

    public static class MasterModelExtensions
    {
        public static CustomerModel MasterInfo(this CustomerModel model, DataSet ds)
        {
            var tableCustomer = ds.Tables[0];
            var tableLine = ds.Tables[1];
            var tableCash = ds.Tables[2];
            var table3rdDCS = ds.Tables[4];
            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = Utils.ToDecimal(tableCash.Rows[0]["masterCash"])
                        + Utils.ToDecimal(tableCash.Rows[0]["agentCash"])
                        + Utils.ToDecimal(tableCash.Rows[0]["customerCash"]),
                Text = "Total Balance"
            });
            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = Utils.ToDecimal(tableCash.Rows[0]["yesterday_masterCash"])
                        + Utils.ToDecimal(tableCash.Rows[0]["yesterday_agentCash"])
                        + Utils.ToDecimal(tableCash.Rows[0]["yesterday_customerCash"]),
                Text = "Yesterday Balance"
            });
            model.BalanceInfo.Balances.Add(new ObjectInfo { Value = Utils.ToDecimal(tableCash.Rows[0]["customerOutstanding"]), Text = "Agent Outstanding" });

            model.PersonalInfo.UplineId = new ObjectInfo { Value = Utils.ToInt32(tableLine.Rows[0]["superID"]), Text = "Super ID" };
            model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["masterName"]), Text = "Master" };
            model.PersonalInfo.Downlines = new List<ObjectInfo>
                    {
                        new ObjectInfo
                        {
                            Key = tableLine.Rows[0].ToInt32("superID").ToString(),
                            Value = Utils.ToString(tableLine.Rows[0]["superName"]),
                            Text = "Super"
                        },
                        new ObjectInfo
                        {
                            Key = tableLine.Rows[0].ToInt32("masterID").ToString(),
                            Value = Utils.ToString(tableLine.Rows[0]["masterName"]),
                            Text = "Master"
                        }
                    };
            model.MasterSettings(table3rdDCS, tableCustomer, tableCash);

            return model;
        }
        
        private static CustomerModel MasterSettings(this CustomerModel model, DataTable table3rdDCS, DataTable tableCustomer, DataTable tableCash = null)
        {
            model.Setting = new Setting
            {
                EnableItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "Closed", Value = Utils.ToBoolean(tableCustomer.Rows[0]["closed"]), Text = "Closed" },
                            new ObjectInfo { Key = "Suspended", Value = Utils.ToBoolean(tableCustomer.Rows[0]["suspended"]), Text = "Suspened" },
                            new ObjectInfo { Key = "AutoPT", Value = !Utils.ToBoolean(tableCash.Rows[0]["disableAutoMPT"]), Text = "Allow Auto PT" },
                            new ObjectInfo { Key = "Outright", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableOutright"]), Text = "Allow Outright" },
                            new ObjectInfo { Key = "EnableCasino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableCasino"]), Text = "Enable Casino" },
                            new ObjectInfo
                            {
                                Key = "EnableNumberGame", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableNumber"]), Text = "Enable Num Game", Tooltip = "Enable Number Game"
                            },
                            new ObjectInfo { Key = "EnableLiveCasino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableLiveCasino"]), Text = "Enable Live Casino" },
                            new ObjectInfo { Key = "EnableVirtualSports", Value = GetVirtualSportsStatus(table3rdDCS), Text = "Virtual Sports" },
                            new ObjectInfo
                            {
                                Key = "DoubleComm", Value = Utils.ToBoolean(tableCustomer.Rows[0]["doubleCommLosing"]), Text = "Double Comm", Tooltip = "Enable Double Commission"
                            },
                            new ObjectInfo
                            {
                                Key = "EnableHRFixedOdds", Value = !tableCustomer.Rows[0].ToBoolean("DisableHRFixedOdds"), Text = "HR Fixed Odds", Tooltip = "Enable Horse Racing Fixed Odds"
                            },
                            new ObjectInfo { Key = "EnableESports", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableESports"]), Text = "E-Sports", Tooltip = "Enable E-Sports" },
                            new ObjectInfo
                            {
                                Key = "EnableColossusBets",
                                Value = !Utils.ToBoolean(tableCustomer.Rows[0]["DisableColossusBet"]),
                                Text = "Colossus Bets",
                                Tooltip = "Enable Colossus Bets"
                            }
                        },
                SyncItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "SyncCasino", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncCasino"]), Text = "Casino Synced" },
                            new ObjectInfo { Key = "SyncLiveCasino", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncLiveCasino"]), Text = "Live Casino Synced" },
                            new ObjectInfo
                            {
                                Key = "HideAllbet",
                                Value = table3rdDCS.Select("ProductID = 21").Length<=0  || (Utils.ToBoolean(table3rdDCS.Select("ProductID = 21").FirstOrDefault()["IsHidden"])
                                || !Utils.ToBoolean(table3rdDCS.Select("ProductID = 21").FirstOrDefault()["IsSync"]))                             },
                            new ObjectInfo
                            {
                                Key = "HideVoidbridge",
                                Value = table3rdDCS.Select("ProductID = 22").Length<=0  || (Utils.ToBoolean(table3rdDCS.Select("ProductID = 22").FirstOrDefault()["IsHidden"])
                                || !Utils.ToBoolean(table3rdDCS.Select("ProductID = 22").FirstOrDefault()["IsSync"]))                             },
                            new ObjectInfo
                            {
                                Key = "HideNG2",
                                Value = table3rdDCS.Select("ProductID = 23").Length<=0  || !Utils.ToBoolean(table3rdDCS.Select("ProductID = 23").FirstOrDefault()["IsSync"])                             },
                            new ObjectInfo
                            {
                                Key = "HideFishingWorld",
                                Value = table3rdDCS.Select("ProductID = 24").Length<=0  || (Utils.ToBoolean(table3rdDCS.Select("ProductID = 24").FirstOrDefault()["IsHidden"])
                                || !Utils.ToBoolean(table3rdDCS.Select("ProductID = 24").FirstOrDefault()["IsSync"]))                             },
                            new ObjectInfo
                            {
                                Key = "HideLottery",
                                Value = table3rdDCS.Select("ProductID = 28").Length<=0  || (Utils.ToBoolean(table3rdDCS.Select("ProductID = 28").FirstOrDefault()["IsHidden"])
                                || !Utils.ToBoolean(table3rdDCS.Select("ProductID = 28").FirstOrDefault()["IsSync"]))                             },
                            new ObjectInfo
                            {
                                Key = "HideLucky3",
                                Value = table3rdDCS.Select("ProductID = 26").Length<=0  || (Utils.ToBoolean(table3rdDCS.Select("ProductID = 26").FirstOrDefault()["IsHidden"])
                                || !Utils.ToBoolean(table3rdDCS.Select("ProductID = 26").FirstOrDefault()["IsSync"]))                             }
                        },
                EditItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                            new ObjectInfo { Key = "EnableKeno", Text = "Enable Keno" },
                            new ObjectInfo { Key = "EnableCockfighting", Text = "Enable Cockfighting" }
                        },
                Details = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "Info", Value = "Edit Info" },
                            new ObjectInfo { Key = "PT", Value = "Edit Position Taking" },
                            new ObjectInfo { Key = "AddNew", Value = "Add New" },
                            new ObjectInfo { Key = "Transfer", Value = "Edit Transfer Condition" },
                            new ObjectInfo { Key = "Racing", Value = "Edit Racing" },
                            new ObjectInfo { Key = "Casino", Value = "Edit Casino" },
                            new ObjectInfo { Key = "LiveCasino", Value = "Edit Live Casino" },
                            new ObjectInfo { Key = "VirtualSports", Value = "Edit Virtual Sports" },
                            new ObjectInfo { Key = "Keno", Value = "Edit Keno" },
                            new ObjectInfo { Key = "GoldDeluxe", Value = "Edit Gold Deluxe" },
                            new ObjectInfo { Key = "ColossusBets", Value = "Edit Colossus Bets" },
                            new ObjectInfo { Key = "Sportsbook2", Value = "Edit Sportsbook 2" },
                            new ObjectInfo { Key = "Cockfighting", Value = "Edit Cockfighting" },
                            new ObjectInfo { Key = "Cricket", Value = "Edit Cricket" }
                        }
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
                Value = !Utils.ToBoolean(tableCustomer.Rows[0]["DisableGoldDeluxe"]),
                Text = "Gold Deluxe"
            });

            return model;
        }

        private static bool GetVirtualSportsStatus(DataTable table3rdDCS)
        {
            DataRow[] drowVirtualSports = table3rdDCS.Select("ProductID = 5");

            return drowVirtualSports.Length > 0 && !Utils.ToBoolean(drowVirtualSports[0]["isDisabled"]);
        }
    }
}
