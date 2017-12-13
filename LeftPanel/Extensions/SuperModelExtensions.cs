namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Core.Context;
    using CustomerSetting.Security.Authenticate;
    using Utilities;
    using ViewModels;

    public static class SuperModelExtensions
    {
        public static CustomerModel SuperInfo(this CustomerModel model, DataSet ds)
        {
            SetBalanceInfo(model, ds);

            SetPersonalInfo(model, ds);

            SetSettingInfo(model, ds);

            return model;
        }

        private static void SetPersonalInfo(CustomerModel model, DataSet ds)
        {
            var tableLine = ds.Tables[1];

            model.PersonalInfo.Downlines = new List<ObjectInfo>
            {
                new ObjectInfo
                {
                    Key = tableLine.Rows[0].ToInt32("superID"),
                    Value = Utils.ToString(tableLine.Rows[0]["superName"]),
                    Text = "Super"
                }
            };

            model.PersonalInfo.CurrentLevel =
                new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["superName"]), Text = "Super" };
        }

        private static void SetBalanceInfo(CustomerModel model, DataSet ds)
        {
            var tableCash = ds.Tables[2];

            model.BalanceInfo.Balances.Add(
                new ObjectInfo
                {
                    Value = Utils.ToDecimal(tableCash.Rows[0]["superCash"])
                            + Utils.ToDecimal(tableCash.Rows[0]["masterCash"])
                            + Utils.ToDecimal(tableCash.Rows[0]["agentCash"])
                            + Utils.ToDecimal(tableCash.Rows[0]["customerCash"]),
                    Text = "Total Balance"
                });
            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = Utils.ToDecimal(tableCash.Rows[0]["yesterday_superCash"])
                        + Utils.ToDecimal(tableCash.Rows[0]["yesterday_masterCash"])
                        + Utils.ToDecimal(tableCash.Rows[0]["yesterday_agentCash"])
                        + Utils.ToDecimal(tableCash.Rows[0]["yesterday_customerCash"]),
                Text = "Yesterday Balance"
            });
            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = Utils.ToDecimal(tableCash.Rows[0]["customerOutstanding"]),
                Text = "Master Outstanding"
            });
        }

        private static void SetSettingInfo(CustomerModel model, DataSet ds)
        {
            var tableCustomer = ds.Tables[0];
            var table3RdDcs = ds.Tables[4];

            model.Setting = new Setting
            {
                EnableItems = EnableItems(tableCustomer, table3RdDcs),
                SyncItems = SyncItems(tableCustomer, table3RdDcs),
                EditItems = EditItems(),
                Details = Details()
            };

            EnableAndDetailProducts(model, table3RdDcs);

            model.Setting.EnableItems.Add(new ObjectInfo
            {
                Key = "EnableGoldDeluxe",
                Value = !Utils.ToBoolean(tableCustomer.Rows[0]["DisableGoldDeluxe"]),
                Text = "Gold Deluxe"
            });

            model.AddBettingLimitForSuper();
        }

        private static void EnableAndDetailProducts(CustomerModel model, DataTable table3RdDcs)
        {
            model.AddEnableAndDetailProduct(table3RdDcs, 21, "Allbet", eFunctions.AllbetBetting, false);
            model.AddEnableAndDetailProduct(table3RdDcs, 22, "Macau Games", eFunctions.VoidbridgeBetting);
            model.AddEnableAndDetailProduct(table3RdDcs, 23, "NG2", eFunctions.NG2Betting);
            model.AddEnableAndDetailProduct(table3RdDcs, 24, "Fishing World");
            model.AddEnableAndDetailProduct(table3RdDcs, 28, "Lottery", eFunctions.LotteryBetting);
            model.AddEnableAndDetailProduct(table3RdDcs, 26, "Lucky 3");
        }

        private static List<ObjectInfo> EditItems()
        {
            return new List<ObjectInfo>
            {
                new ObjectInfo {Key = "EnableRacing", Text = "Enable Racing"},
                new ObjectInfo {Key = "EnableKeno", Text = "Enable Keno"},
                new ObjectInfo {Key = "EnableCockfighting", Text = "Enable Cockfighting"}
            };
        }

        private static List<ObjectInfo> Details()
        {
            return new List<ObjectInfo>
            {
                new ObjectInfo {Key = "Info", Value = "Edit Info"},
                new ObjectInfo {Key = "PT", Value = "Edit Position Taking"},
                new ObjectInfo {Key = "AddNew", Value = "Add New"},
                new ObjectInfo {Key = "Transfer", Value = "Edit Transfer Condition"},
                new ObjectInfo {Key = "Racing", Value = "Edit Racing"},
                new ObjectInfo {Key = "Casino", Value = "Edit Casino"},
                new ObjectInfo {Key = "LiveCasino", Value = "Edit Live Casino"},
                new ObjectInfo {Key = "VirtualSports", Value = "Edit Virtual Sports"},
                new ObjectInfo {Key = "Keno", Value = "Edit Keno"},
                new ObjectInfo {Key = "GoldDeluxe", Value = "Edit Gold Deluxe"},
                new ObjectInfo {Key = "ColossusBets", Value = "Edit Colossus Bets"},
                new ObjectInfo {Key = "Sportsbook2", Value = "Edit Sportsbook 2"},
                new ObjectInfo {Key = "Cockfighting", Value = "Edit Cockfighting"},
                new ObjectInfo {Key = "Cricket", Value = "Edit Cricket"}
            };
        }

        private static List<ObjectInfo> EnableItems(DataTable tableCustomer, DataTable table3RdDcs)
        {
            var customerRow = tableCustomer.Rows[0];
            return new List<ObjectInfo>
            {
                new ObjectInfo {  Key = "Closed",  Value = Utils.ToBoolean(customerRow["closed"]),  Text = "Closed"  },
                new ObjectInfo {  Key = "Suspended",  Value = Utils.ToBoolean(customerRow["suspended"]),  Text = "Suspened"  },
                new ObjectInfo {  Key = "Outright",  Value = !Utils.ToBoolean(customerRow["disableOutright"]),  Text = "Allow Outright"  },
                new ObjectInfo {  Key = "EnableCasino",  Value = !Utils.ToBoolean(customerRow["disableCasino"]),  Text = "Enable Casino"  },
                new ObjectInfo {  Key = "EnableNumberGame",  Value = !Utils.ToBoolean(customerRow["disableNumber"]),  Text = "Enable Num Game",  Tooltip = "Enable Number Game"  },
                new ObjectInfo {  Key = "EnableLiveCasino",  Value = !Utils.ToBoolean(customerRow["disableLiveCasino"]),  Text = "Enable Live Casino"  },
                new ObjectInfo {  Key = "EnableVirtualSports",  Value = GetVirtualSportsStatus(table3RdDcs),  Text = "Virtual Sports"  },
                new ObjectInfo {  Key = "DoubleComm",  Value = Utils.ToBoolean(customerRow["doubleCommLosing"]),  Text = "Double Comm",  Tooltip = "Enable Double Commission"  },
                new ObjectInfo {  Key = "EnableHRFixedOdds",  Value = !customerRow.ToBoolean("DisableHRFixedOdds"),  Text = "HR Fixed Odds",  Tooltip = "Enable Horse Racing Fixed Odds"  },
                new ObjectInfo {  Key = "EnableESports",  Value = !Utils.ToBoolean(customerRow["disableESports"]),  Text = "E-Sports",  Tooltip = "Enable E-Sports"  },
                new ObjectInfo {  Key = "EnableColossusBets",  Value = !Utils.ToBoolean(customerRow["DisableColossusBet"]),  Text = "Colossus Bets",  Tooltip = "Enable Colossus Bets"  }
            };
        }

        private static List<ObjectInfo> SyncItems(DataTable tableCustomer, DataTable table3RdDcs)
        {
            return new List<ObjectInfo>
            {
                new ObjectInfo {  Key = "SyncCasino",  Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncCasino"]),  Text = "Casino Synced"  },
                new ObjectInfo {  Key = "SyncLiveCasino",  Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncLiveCasino"]),  Text = "Live Casino Synced"  },
                new ObjectInfo
                {
                    Key = "HideAllbet",
                    Value = !table3RdDcs.Select("ProductID = 21").Any()
                            || Utils.ToBoolean(table3RdDcs.Select("ProductID = 21").FirstOrDefault()?["IsHidden"])
                            || !Utils.ToBoolean(table3RdDcs.Select("ProductID = 21").FirstOrDefault()?["IsSync"])
                },
                new ObjectInfo
                {
                    Key = "HideVoidbridge",
                    Value = !table3RdDcs.Select("ProductID = 22").Any()
                            || Utils.ToBoolean(table3RdDcs.Select("ProductID = 22").FirstOrDefault()?["IsHidden"])
                            || !Utils.ToBoolean(table3RdDcs.Select("ProductID = 22").FirstOrDefault()?["IsSync"])
                },
                new ObjectInfo
                {
                    Key = "HideNG2",
                    Value = !table3RdDcs.Select("ProductID = 23").Any()
                            || !Utils.ToBoolean(table3RdDcs.Select("ProductID = 23").FirstOrDefault()?["IsSync"])
                },
                new ObjectInfo
                {
                    Key = "HideFishingWorld",
                    Value = !table3RdDcs.Select("ProductID = 24").Any()
                            || Utils.ToBoolean(table3RdDcs.Select("ProductID = 24").FirstOrDefault()?["IsHidden"])
                            || !Utils.ToBoolean(table3RdDcs.Select("ProductID = 24").FirstOrDefault()?["IsSync"])
                },
                new ObjectInfo
                {
                    Key = "HideLottery",
                    Value = !table3RdDcs.Select("ProductID = 28").Any()
                            || Utils.ToBoolean(table3RdDcs.Select("ProductID = 28").FirstOrDefault()?["IsHidden"])
                            || !Utils.ToBoolean(table3RdDcs.Select("ProductID = 28").FirstOrDefault()?["IsSync"])
                },
                new ObjectInfo
                {
                    Key = "HideLucky3",
                    Value = !table3RdDcs.Select("ProductID = 26").Any()
                            || Utils.ToBoolean(table3RdDcs.Select("ProductID = 26").FirstOrDefault()?["IsHidden"])
                            || !Utils.ToBoolean(table3RdDcs.Select("ProductID = 26").FirstOrDefault()?["IsSync"])
                }
            };
        }

        private static CustomerModel AddBettingLimitForSuper(this CustomerModel model)
        {
            var userId = CurrentSession.Instance.UserId;
            var showBetSetting = CustomerModelExtensions.IsAuthorized(userId, eFunctions.CasinoBetting,
                                     AuthenticateAction.AuthenticationType.Read)
                                 || CustomerModelExtensions.IsAuthorized(userId, eFunctions.GoldDeluxeBetting,
                                     AuthenticateAction.AuthenticationType.Read)
                                 || CustomerModelExtensions.IsAuthorized(userId, eFunctions.KenoBetting,
                                     AuthenticateAction.AuthenticationType.Read)
                                 || CustomerModelExtensions.IsAuthorized(userId, eFunctions.LiveCasinoBetting,
                                     AuthenticateAction.AuthenticationType.Read)
                                 || CustomerModelExtensions.IsAuthorized(userId, eFunctions.RacingBetting,
                                     AuthenticateAction.AuthenticationType.Read)
                                 || CustomerModelExtensions.IsAuthorized(userId, eFunctions.SportsbookBetting,
                                     AuthenticateAction.AuthenticationType.Read)
                                 || CustomerModelExtensions.IsAuthorized(userId, eFunctions.VirtualSportsBetting,
                                     AuthenticateAction.AuthenticationType.Read)
                                 || CustomerModelExtensions.IsAuthorized(userId, eFunctions.AllbetBetting,
                                     AuthenticateAction.AuthenticationType.Read)
                                 || CustomerModelExtensions.IsAuthorized(userId, eFunctions.VoidbridgeBetting,
                                     AuthenticateAction.AuthenticationType.Read)
                                 || CustomerModelExtensions.IsAuthorized(userId, eFunctions.NG2Betting,
                                     AuthenticateAction.AuthenticationType.Read)
                                 || CustomerModelExtensions.IsAuthorized(userId, eFunctions.LotteryBetting,
                                     AuthenticateAction.AuthenticationType.Read);

            if (showBetSetting)
            {
                model.Setting.Details.Add(new ObjectInfo
                {
                    Key = "BetSetting",
                    Value = "Edit Betting Limit"
                });
            }

            return model;
        }

        private static bool GetVirtualSportsStatus(DataTable table3RdDcs)
        {
            var drowVirtualSports = table3RdDcs.Select("ProductID = 5");

            return drowVirtualSports.Length > 0 && !Utils.ToBoolean(drowVirtualSports[0]["isDisabled"]);
        }
    }
}