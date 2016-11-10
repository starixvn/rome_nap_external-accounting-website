// -----------------------------------------------------------------------
// <copyright file="MemberModelExtensions.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.Utilities;
    using RemotingRef;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class MemberModelExtensions
    {
        /// <summary>
        /// Gets customer model for super level.
        /// </summary>
        /// <param name="model">Model object.</param>
        /// <param name="ds">DataSet object that contains user information.</param>
        /// <returns>CustomerModel object.</returns>
        public static CustomerModel MemberInfo(this CustomerModel model, DataSet ds)
        {
            var tableCustomer = ds.Tables[0];
            var tableLine = ds.Tables[1];
            model.PersonalInfo.UplineId = new ObjectInfo { Value = Utils.ToInt32(tableLine.Rows[0]["agentID"]), Text = "Agent ID" };
            model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["memberName"]), Text = "Member" };
            model.PersonalInfo.Downlines = new List<ObjectInfo>
                    {
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("superID"), Value = Utils.ToString(tableLine.Rows[0]["superName"]), Text = "Super" },
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("masterID"), Value = Utils.ToString(tableLine.Rows[0]["masterName"]), Text = "Master" },
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("agentID"), Value = Utils.ToString(tableLine.Rows[0]["agentName"]), Text = "Agent" }
                    };
            model.MemberSettings(ds);

            return model;
        }

        /// <summary>
        /// Customer settings.
        /// </summary>
        /// <param name="model">CustomerModel object extension.</param>
        /// <param name="ds">Customer data.</param>
        /// <returns>CustomerModel object.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private static CustomerModel MemberSettings(this CustomerModel model, DataSet ds)
        {
            var tableCustomer = ds.Tables[0];
            var table3rdDCS = ds.Tables[4];
            model.Setting = new Setting
            {
                EnableItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "Closed", Value = Utils.ToBoolean(tableCustomer.Rows[0]["closed"]), Text = "Closed" },
                            new ObjectInfo { Key = "Suspended", Value = Utils.ToBoolean(tableCustomer.Rows[0]["suspended"]), Text = "Suspened" },
                            new ObjectInfo { Key = "Outright", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableOutright"]), Text = "Allow Outright" },
                            new ObjectInfo { Key = "EnableCasino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableCasino"]), Text = "Enable Casino" },
                            new ObjectInfo { Key = "EnableBingo", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableBingo"]), Text = "Enable Bingo" },
                            new ObjectInfo
                            {
                            Key = "EnableNumberGame", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableNumber"]),
                            Text = "Enable Num Game", Tooltip = "Enable Number Game"
                            },
                            new ObjectInfo { Key = "EnableLiveCasino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableLiveCasino"]), Text = "Enable Live Casino" },
                            new ObjectInfo { Key = "EnableVirtualSports", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableVirtualSports"]), Text = "Virtual Sports" },
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
                                Key = "EnableGoldDeluxe", 
                                Value = !Utils.ToBoolean(tableCustomer.Rows[0]["DisableGoldDeluxe"]), 
                                Text = "Enable Gold Deluxe"
                            },
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
                            new ObjectInfo { Key = "SyncBingo", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncBingo"]), Text = "Bingo Synced" },
                            new ObjectInfo { Key = "SyncLiveCasino", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncLiveCasino"]), Text = "Live Casino Synced" },
                            new ObjectInfo
                            {
                                Key = "HideAllbet",
                                Value = table3rdDCS.Select("ProductID = 21").Count() > 0 ? (Utils.ToBoolean(table3rdDCS.Select("ProductID = 21").FirstOrDefault()["IsHidden"]) ||
                                !Utils.ToBoolean(table3rdDCS.Select("ProductID = 21").FirstOrDefault()["IsSync"])) : true
                            },
                            new ObjectInfo
                            {
                                Key = "HideVoidbridge",
                                Value = table3rdDCS.Select("ProductID = 22").Count() > 0 ? (Utils.ToBoolean(table3rdDCS.Select("ProductID = 22").FirstOrDefault()["IsHidden"]) ||
                                !Utils.ToBoolean(table3rdDCS.Select("ProductID = 22").FirstOrDefault()["IsSync"])) : true
                            },
                            new ObjectInfo
                            {
                                Key = "HideNG2",
                                Value = table3rdDCS.Select("ProductID = 23").Count() > 0 ? !Utils.ToBoolean(table3rdDCS.Select("ProductID = 23").FirstOrDefault()["IsSync"]) : true
                            },
                            new ObjectInfo
                            {
                                Key = "HideVoidbridge",
                                Value = table3rdDCS.Select("ProductID = 22").Count() > 0 ? (Utils.ToBoolean(table3rdDCS.Select("ProductID = 22").FirstOrDefault()["IsHidden"]) ||
                                !Utils.ToBoolean(table3rdDCS.Select("ProductID = 22").FirstOrDefault()["IsSync"])) : true
                            }
                        },
                EditItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                            new ObjectInfo { Key = "EnablePoker", Text = "Enable Poker" },
                            new ObjectInfo { Key = "EnableKeno", Text = "Enable Keno" },
                            new ObjectInfo { Key = "EnableCockfighting", Text = "Enable Cockfighting" },
                        },
                Details = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "Info", Value = "Edit Info" },
                            new ObjectInfo { Key = "PT", Value = "Edit Position Taking" },
                            new ObjectInfo { Key = "Racing", Value = "Edit Racing" },
                            new ObjectInfo { Key = "Casino", Value = "Edit Casino" },
                            new ObjectInfo { Key = "Bingo", Value = "Edit Bingo" },
                            new ObjectInfo { Key = "Poker", Value = "Edit Poker" },
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

            model.AddEnableAndDetailProduct(table3rdDCS, 21, "Allbet", false);
            model.AddEnableAndDetailProduct(table3rdDCS, 22, "Macau Games");
            model.AddEnableAndDetailProduct(table3rdDCS, 23, "NG2", true, true);
            model.AddEnableAndDetailProduct(table3rdDCS, 24, "Fishing World");

            return model;
        }

        /// <summary>
        /// Gets PT products on member level.
        /// </summary>
        /// <param name="model">CustomerModel item.</param>
        /// <param name="product">ProductStatuses item.</param>
        /// <returns>CustomerModel object.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
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
            model.AddDetailProductForCrossLevel("Bingo", "Bingo");
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

            return model;
        }

        private static IList<string> IgnoreOrExcludeProducts()
        {
            var items = new List<string>();
            items.Add("Info");
            items.Add("Poker");
            items.Add("ColossusBets");
            items.Add("Bingo");

            return items;
        }

        private static CustomerModel AddDetailProductForCrossLevel(this CustomerModel model, string productKey, string productName)
        {
            var product = model.Setting.MasterPts.Where(p => p.Key.ToString().Equals(productKey));
            if (product.Count() > 0)
            {
                product.FirstOrDefault().Value = productName;
            }

            model.Setting.SuperPts = model.Setting.MasterPts;

            return model;
        }
    }
}
