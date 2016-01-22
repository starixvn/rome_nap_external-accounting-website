// -----------------------------------------------------------------------
// <copyright file="MemberModelExtensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.Utilities;

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
                        },
                EditItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                            new ObjectInfo { Key = "EnablePoker", Text = "Enable Poker" },
                            new ObjectInfo { Key = "EnableKeno", Text = "Enable Keno" },
                            new ObjectInfo { Key = "EnableCockfighting", Text = "Enable Cockfighting" }
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
            bool syncCasino = product.SyncCasino, syncBingo = product.SyncBingo, syncLiveCasino = product.SyncLiveCasino, hideVS = product.HideVirtualSports;
            bool hideKeno = product.HideKeno;
            bool hideGoldDeluxe = product.HideGoldDeluxe;
            bool hideSportsbook2 = product.HideSportsbook2;
            bool hideCockfighting = product.HideCockfighting;
            bool hideCricket = product.HideCricket;

            model.Setting.SuperPts.Clear();
            model.Setting.MasterPts.Clear();
            model.Setting.Details.ForEach(o =>
            {
                model.Setting.MasterPts.Add(new ObjectInfo { Key = o.Key, Value = o.Value });
            });
            model.Setting.MasterPts =
                   model.Setting.MasterPts.Where(o => o.Key.ToString() == "PT" ||
                       (o.Key.ToString() == "Casino" && syncCasino) ||
                       (o.Key.ToString() == "Bingo" && syncBingo) ||
                       (o.Key.ToString() == "LiveCasino" && syncLiveCasino) ||
                       (o.Key.ToString() == "VirtualSports" && !hideVS) ||
                       o.Key.ToString() == "Racing" ||
                       (o.Key.ToString() == "Keno" && !hideKeno) ||
                       (o.Key.ToString() == "GoldDeluxe" && !hideGoldDeluxe) ||
                       (o.Key.ToString() == "Sportsbook2" && !hideSportsbook2) ||
                       (o.Key.ToString() == "Cockfighting" && !hideCockfighting)||
                       (o.Key.ToString() == "Cricket" && !hideCricket)).ToList();

            model.Setting.MasterPts.Add(new ObjectInfo { Key = "SBPT", Value = "Sportsbook's PT" });
            model.Setting.MasterPts.Where(o => o.Key.ToString() == "PT").FirstOrDefault().Value = "Sportsbook";
            model.Setting.MasterPts.Where(o => o.Key.ToString() == "Racing").FirstOrDefault().Value = "Racing";
            var casino = model.Setting.MasterPts.Where(o => o.Key.ToString() == "Casino");
            var bingo = model.Setting.MasterPts.Where(o => o.Key.ToString() == "Bingo");
            var liveCasino = model.Setting.MasterPts.Where(o => o.Key.ToString() == "LiveCasino");
            var virtualSports = model.Setting.MasterPts.Where(o => o.Key.ToString() == "VirtualSports");
            var keno = model.Setting.MasterPts.Where(o => o.Key.ToString() == "Keno");
            var goldDeluxe = model.Setting.MasterPts.Where(o => o.Key.ToString() == "GoldDeluxe");
            var sportsbook2 = model.Setting.MasterPts.Where(o => o.Key.ToString() == "Sportsbook2");
            var cockFighting = model.Setting.MasterPts.Where(o => o.Key.ToString() == "Cockfighting");
            var cricket = model.Setting.MasterPts.Where(o => o.Key.ToString() == "Cricket");

            if (casino.Count() > 0)
            {
                casino.FirstOrDefault().Value = "Casino";
            }

            if (bingo.Count() > 0)
            {
                bingo.FirstOrDefault().Value = "Bingo";
            }

            if (liveCasino.Count() > 0)
            {
                liveCasino.FirstOrDefault().Value = "Live Casino";
            }

            if (virtualSports.Count() > 0)
            {
                virtualSports.FirstOrDefault().Value = "Virtual Sports";
            }

            if (keno.Count() > 0)
            {
                keno.FirstOrDefault().Value = "Keno";
            }

            if (goldDeluxe.Count() > 0)
            {
                goldDeluxe.FirstOrDefault().Value = "Gold Deluxe";
            }

            if (sportsbook2.Count() > 0)
            {
                sportsbook2.FirstOrDefault().Value = "Sportsbook 2";
            }

            if (cockFighting.Count() > 0)
            {
                cockFighting.FirstOrDefault().Value = "Cockfighting";
            }

            if (cricket.Count() > 0)
            {
                cockFighting.FirstOrDefault().Value = "Cricket";
            }

            model.Setting.SuperPts = model.Setting.MasterPts;
            return model;
        }
    }
}
