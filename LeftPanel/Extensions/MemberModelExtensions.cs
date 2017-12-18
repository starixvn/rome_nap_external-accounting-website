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
            SetMemberSettings(model, ds);

            return model;
        }

        private static void SetPersonalInfo(CustomerModel model, DataSet ds)
        {
            var lineRow = ds.Tables[1].Rows[0];

            model.PersonalInfo.UplineId = new ObjectInfo
            {
                Value = Utils.ToInt32(lineRow["agentID"]),
                Text = "Agent ID"
            };

            model.PersonalInfo.CurrentLevel = new ObjectInfo
            {
                Value = Utils.ToString(lineRow["memberName"]),
                Text = "Member"
            };

            model.PersonalInfo.Downlines = new List<ObjectInfo>
            {
                new ObjectInfo
                {
                    Key = lineRow.ToInt32("superID").ToString(),
                    Value = Utils.ToString(lineRow["superName"]),
                    Text = "Super"
                },
                new ObjectInfo
                {
                    Key = lineRow.ToInt32("masterID").ToString(),
                    Value = Utils.ToString(lineRow["masterName"]),
                    Text = "Master"
                },
                new ObjectInfo
                {
                    Key = lineRow.ToInt32("agentID").ToString(),
                    Value = Utils.ToString(lineRow["agentName"]),
                    Text = "Agent"
                }
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
                new ObjectInfo { Key = "Info", Value = "Edit Info" },
                new ObjectInfo { Key = "PT", Value = "Edit Position Taking" },
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
            };
        }

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
        {
            return new List<ObjectInfo>
            {
                new ObjectInfo("SyncCasino", Utils.ToBoolean(customerRow["syncCasino"]), "Casino Synced"),
                new ObjectInfo("SyncLiveCasino", Utils.ToBoolean(customerRow["syncLiveCasino"]), "Live Casino Synced"),
                new ObjectInfo("HideAllbet", Get3rdProductStatus(table3RdDcs, 21)),
                new ObjectInfo("HideVoidbridge", Get3rdProductStatus(table3RdDcs, 22)),
                new ObjectInfo("HideNG2", GetNG2ProductStatus(table3RdDcs)),
                new ObjectInfo("HideFishingWorld", Get3rdProductStatus(table3RdDcs, 24)),
                new ObjectInfo("HideLottery", Get3rdProductStatus(table3RdDcs, 28)),
                new ObjectInfo("HideLucky3", Get3rdProductStatus(table3RdDcs, 26))
            };
        }

        private static DataRow GetProductRow(DataTable table3RdDcs, int productId)
            => table3RdDcs.Select($"ProductID = {productId}").FirstOrDefault();

        private static bool Get3rdProductStatus(DataTable table3RdDcs, int productId)
        {
            var productRow = GetProductRow(table3RdDcs, productId);

            return productRow == null
                   || Utils.ToBoolean(productRow["IsHidden"])
                   || !Utils.ToBoolean(productRow["IsSync"]);
        }

        private static bool GetNG2ProductStatus(DataTable table3RdDcs)
        {
            var productRow = GetProductRow(table3RdDcs, 23);

            return productRow == null || !Utils.ToBoolean(productRow["IsSync"]);
        }

        private static bool GetVirtualSportsStatus(DataTable table3rdDCS)
        {
            var drowVirtualSports = table3rdDCS.Select("ProductID = 5");

            return drowVirtualSports.Length > 0 && !Utils.ToBoolean(drowVirtualSports[0]["isDisabled"]);
        }

        private static List<ObjectInfo> EnableItems(DataRow customerRow, DataTable table3rdDCS)
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
                    Value = GetVirtualSportsStatus(table3rdDCS),
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

        public static CustomerModel GetSuperMasterPtsOnMember(this CustomerModel model)
        {
            model.Setting.SuperPts.Clear();
            model.Setting.MasterPts.Clear();

            foreach (var item in model.Setting.Details)
            {
                if (!IgnoreOrExcludeProducts().Contains(item.Key))
                {
                    model.Setting.MasterPts.Add(new ObjectInfo { Key = item.Key, Value = item.Value });
                }
            }

            model.Setting.MasterPts.Add(
                new ObjectInfo
                {
                    Key = "SBPT",
                    Value = "Sportsbook's PT"
                });

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