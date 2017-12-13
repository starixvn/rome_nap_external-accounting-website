namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using RemotingRef;
    using Utilities;
    using ViewModels;

    public static class MemberModelExtensions
    {
        public static CustomerModel MemberInfo(this CustomerModel model, DataSet ds)
        {
            SetPersonalInfo(model, ds);

            SetSettingInfo(model, ds);

            return model;
        }

        private static void SetPersonalInfo(CustomerModel model, DataSet ds)
        {
            var lineRow = ds.Tables[1].Rows[0];

            model.PersonalInfo.UplineId =
                new ObjectInfo { Value = Utils.ToInt32(lineRow["agentID"]), Text = "Agent ID" };
            model.PersonalInfo.CurrentLevel =
                new ObjectInfo { Value = Utils.ToString(lineRow["memberName"]), Text = "Member" };

            model.PersonalInfo.Downlines = new List<ObjectInfo>
            {
                new ObjectInfo
                {
                    Key = lineRow.ToInt32("superID"),
                    Value = Utils.ToString(lineRow["superName"]),
                    Text = "Super"
                },
                new ObjectInfo
                {
                    Key = lineRow.ToInt32("masterID"),
                    Value = Utils.ToString(lineRow["masterName"]),
                    Text = "Master"
                },
                new ObjectInfo
                {
                    Key = lineRow.ToInt32("agentID"),
                    Value = Utils.ToString(lineRow["agentName"]),
                    Text = "Agent"
                }
            };
        }

        private static void SetSettingInfo(CustomerModel model, DataSet ds)
        {
            var table3RdDcs = ds.Tables[4];
            var customerRow = ds.Tables[0].Rows[0];

            model.Setting = new Setting
            {
                EnableItems = EnableItems(customerRow, table3RdDcs),
                SyncItems = SyncItems(customerRow, table3RdDcs),
                EditItems = EditItems(),
                Details = Details()
            };

            model.AddEnableAndDetailProduct(table3RdDcs, 21, "Allbet", eFunctions.AllbetBetting, false);
            model.AddEnableAndDetailProduct(table3RdDcs, 22, "Macau Games", eFunctions.VoidbridgeBetting);
            model.AddEnableAndDetailProduct(table3RdDcs, 23, "NG2", eFunctions.NG2Betting);
            model.AddEnableAndDetailProduct(table3RdDcs, 24, "Fishing World");
            model.AddEnableAndDetailProduct(table3RdDcs, 28, "Lottery", eFunctions.LotteryBetting);
            model.AddEnableAndDetailProduct(table3RdDcs, 26, "Lucky 3");

            model.Setting.EnableItems.Add(new ObjectInfo
            {
                Key = "EnableGoldDeluxe",
                Value = !Utils.ToBoolean(customerRow["DisableGoldDeluxe"]),
                Text = "Gold Deluxe"
            });
        }

        private static List<ObjectInfo> Details()
        {
            return new List<ObjectInfo>
            {
                new ObjectInfo {Key = "Info", Value = "Edit Info"},
                new ObjectInfo {Key = "PT", Value = "Edit Position Taking"},
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

        private static List<ObjectInfo> EditItems()
        {
            return new List<ObjectInfo>
            {
                new ObjectInfo {Key = "EnableRacing", Text = "Enable Racing"},
                new ObjectInfo {Key = "EnableKeno", Text = "Enable Keno"},
                new ObjectInfo {Key = "EnableCockfighting", Text = "Enable Cockfighting"}
            };
        }

        private static List<ObjectInfo> SyncItems(DataRow customerRow, DataTable table3RdDcs)
        {
            return new List<ObjectInfo>
            {
                new ObjectInfo
                {
                    Key = "SyncCasino",
                    Value = Utils.ToBoolean(customerRow["syncCasino"]),
                    Text = "Casino Synced"
                },
                new ObjectInfo
                {
                    Key = "SyncLiveCasino",
                    Value = Utils.ToBoolean(customerRow["syncLiveCasino"]),
                    Text = "Live Casino Synced"
                },
                new ObjectInfo
                {
                    Key = "HideAllbet",
                    Value = Get3rdProductStatus(table3RdDcs, 21)
                },
                new ObjectInfo
                {
                    Key = "HideVoidbridge",
                    Value =  Get3rdProductStatus(table3RdDcs, 22)

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
                    Value = Get3rdProductStatus(table3RdDcs, 24)
                },
                new ObjectInfo
                {
                    Key = "HideLottery",
                    Value = Get3rdProductStatus(table3RdDcs, 28)
                },
                new ObjectInfo
                {
                    Key = "HideLucky3",
                    Value = Get3rdProductStatus(table3RdDcs, 26)
                }
            };
        }

        private static bool Get3rdProductStatus(DataTable table3RdDcs, int productId)
        {
            return !table3RdDcs.Select("ProductID = " + productId).Any()
                   || Utils.ToBoolean(table3RdDcs.Select("ProductID = " + productId).FirstOrDefault()?["IsHidden"])
                   || !Utils.ToBoolean(table3RdDcs.Select("ProductID = " + productId).FirstOrDefault()?["IsSync"]);
        }

        private static List<ObjectInfo> EnableItems(DataRow customerRow, DataTable table3RdDcs)
        {
            return new List<ObjectInfo>
            {
                new ObjectInfo
                {
                    Key = "Closed",
                    Value = Utils.ToBoolean(customerRow["closed"]),
                    Text = "Closed"
                },
                new ObjectInfo
                {
                    Key = "Suspended",
                    Value = Utils.ToBoolean(customerRow["suspended"]),
                    Text = "Suspened"
                },
                new ObjectInfo
                {
                    Key = "Outright",
                    Value = !Utils.ToBoolean(customerRow["disableOutright"]),
                    Text = "Allow Outright"
                },
                new ObjectInfo
                {
                    Key = "EnableCasino",
                    Value = !Utils.ToBoolean(customerRow["disableCasino"]),
                    Text = "Enable Casino"
                },
                new ObjectInfo
                {
                    Key = "EnableNumberGame",
                    Value = !Utils.ToBoolean(customerRow["disableNumber"]),
                    Text = "Enable Num Game",
                    Tooltip = "Enable Number Game"
                },
                new ObjectInfo
                {
                    Key = "EnableLiveCasino",
                    Value = !Utils.ToBoolean(customerRow["disableLiveCasino"]),
                    Text = "Enable Live Casino"
                },
                new ObjectInfo
                {
                    Key = "EnableVirtualSports",
                    Value = GetVirtualSportsStatus(table3RdDcs),
                    Text = "Virtual Sports"
                },
                new ObjectInfo
                {
                    Key = "DoubleComm",
                    Value = Utils.ToBoolean(customerRow["doubleCommLosing"]),
                    Text = "Double Comm",
                    Tooltip = "Enable Double Commission"
                },
                new ObjectInfo
                {
                    Key = "EnableHRFixedOdds",
                    Value = !customerRow.ToBoolean("DisableHRFixedOdds"),
                    Text = "HR Fixed Odds",
                    Tooltip = "Enable Horse Racing Fixed Odds"
                },
                new ObjectInfo
                {
                    Key = "EnableESports",
                    Value = !Utils.ToBoolean(customerRow["disableESports"]),
                    Text = "E-Sports",
                    Tooltip = "Enable E-Sports"
                },
                new ObjectInfo
                {
                    Key = "EnableColossusBets",
                    Value = !Utils.ToBoolean(customerRow["DisableColossusBet"]),
                    Text = "Colossus Bets",
                    Tooltip = "Enable Colossus Bets"
                }
            };
        }

        public static CustomerModel GetSuperMasterPtsOnMember(this CustomerModel model, ProductStatuses product)
        {
            model.Setting.SuperPts.Clear();
            model.Setting.MasterPts.Clear();

            foreach (var item in model.Setting.Details)
            {
                if (!IgnoreOrExcludeProducts().Contains(item.Key.ToString()))
                {
                    model.Setting.MasterPts.Add(new ObjectInfo { Key = item.Key, Value = item.Value });
                }
            }

            model.Setting.MasterPts.Add(new ObjectInfo { Key = "SBPT", Value = "Sportsbook's PT" });

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
            model.AddDetailProductForCrossLevel("NG2",
                RefData.GetResourceDataByRefIdAndResourceId("Product", "23", "en-US", "NG2"));
            model.AddDetailProductForCrossLevel("FishingWorld", "Fishing World");
            model.AddDetailProductForCrossLevel("Lottery", "Lottery");
            model.AddDetailProductForCrossLevel("Lucky3", "Lucky 3");

            return model;
        }

        private static IList<string> IgnoreOrExcludeProducts() => new List<string> { "Info", "ColossusBets" };

        private static CustomerModel AddDetailProductForCrossLevel(this CustomerModel model, string productKey,
            string productName)
        {
            var product = model.Setting.MasterPts.Where(p => p.Key.ToString().Equals(productKey));
            if (product.Any())
            {
                product.FirstOrDefault().Value = productName;
            }

            model.Setting.SuperPts = model.Setting.MasterPts;

            return model;
        }

        private static bool GetVirtualSportsStatus(DataTable table3RdDcs)
        {
            var drowVirtualSports = table3RdDcs.Select("ProductID = 5");

            return drowVirtualSports.Length > 0 && !Utils.ToBoolean(drowVirtualSports[0]["isDisabled"]);
        }
    }
}