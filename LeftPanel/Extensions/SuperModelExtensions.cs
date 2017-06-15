// -----------------------------------------------------------------------
// <copyright file="SuperModelExtensions.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class SuperModelExtensions
    {
        /// <summary>
        /// Gets customer model for super level.
        /// </summary>
        /// <param name="model">Model object.</param>
        /// <param name="ds">DataSet object that contains user information.</param>
        /// <returns>CustomerModel object.</returns>
        public static CustomerModel SuperInfo(this CustomerModel model, DataSet ds)
        {
            var tableCustomer = ds.Tables[0];
            var tableLine = ds.Tables[1];
            var tableCash = ds.Tables[2];
            var table3rdDCS = ds.Tables[4];
            model.BalanceInfo.Balances.Add(
                new ObjectInfo
                {
                    Value = Utils.ToDecimal(tableCash.Rows[0]["superCash"]) +
                            Utils.ToDecimal(tableCash.Rows[0]["masterCash"]) +
                            Utils.ToDecimal(tableCash.Rows[0]["agentCash"]) +
                            Utils.ToDecimal(tableCash.Rows[0]["customerCash"]),
                    Text = "Total Balance"
                });
            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = Utils.ToDecimal(tableCash.Rows[0]["yesterday_superCash"]) +
                        Utils.ToDecimal(tableCash.Rows[0]["yesterday_masterCash"]) +
                        Utils.ToDecimal(tableCash.Rows[0]["yesterday_agentCash"]) +
                        Utils.ToDecimal(tableCash.Rows[0]["yesterday_customerCash"]),
                Text = "Yesterday Balance"
            });
            model.BalanceInfo.Balances.Add(new ObjectInfo { Value = Utils.ToDecimal(tableCash.Rows[0]["customerOutstanding"]), Text = "Master Outstanding" });
            model.PersonalInfo.Downlines = new List<ObjectInfo>
            {
                new ObjectInfo { Key = tableLine.Rows[0].ToInt32("superID"), Value = Utils.ToString(tableLine.Rows[0]["superName"]), Text = "Super" }
            };
            model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["superName"]), Text = "Super" };
            model.SuperSettings(tableCustomer, table3rdDCS);

            return model;
        }

        /// <summary>
        /// Customer settings.
        /// </summary>
        /// <param name="model">CustomerModel object extension.</param>
        /// <param name="tableCustomer">Customer data.</param>
        /// <returns>CustomerModel object.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private static CustomerModel SuperSettings(this CustomerModel model, DataTable tableCustomer, DataTable table3rdDCS)
        {
            model.Setting = new Setting
            {
                EnableItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "Closed", Value = Utils.ToBoolean(tableCustomer.Rows[0]["closed"]), Text = "Closed" },
                            new ObjectInfo { Key = "Suspended", Value = Utils.ToBoolean(tableCustomer.Rows[0]["suspended"]), Text = "Suspened" },
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
                                Value = table3rdDCS.Select("ProductID = 21").Any() ? (Utils.ToBoolean(table3rdDCS.Select("ProductID = 21").FirstOrDefault()["IsHidden"]) ||
                                !Utils.ToBoolean(table3rdDCS.Select("ProductID = 21").FirstOrDefault()["IsSync"])) : true
                            },
                            new ObjectInfo
                            {
                                Key = "HideVoidbridge",
                                Value = table3rdDCS.Select("ProductID = 22").Any() ? (Utils.ToBoolean(table3rdDCS.Select("ProductID = 22").FirstOrDefault()["IsHidden"]) ||
                                !Utils.ToBoolean(table3rdDCS.Select("ProductID = 22").FirstOrDefault()["IsSync"])) : true
                            },
                            new ObjectInfo
                            {
                                Key = "HideNG2",
                                Value = table3rdDCS.Select("ProductID = 23").Any() ? !Utils.ToBoolean(table3rdDCS.Select("ProductID = 23").FirstOrDefault()["IsSync"]) : true
                            },
                            new ObjectInfo
                            {
                                Key = "HideFishingWorld",
                                Value = table3rdDCS.Select("ProductID = 24").Any() ? (Utils.ToBoolean(table3rdDCS.Select("ProductID = 24").FirstOrDefault()["IsHidden"]) ||
                                !Utils.ToBoolean(table3rdDCS.Select("ProductID = 24").FirstOrDefault()["IsSync"])) : true
                            },
                            new ObjectInfo
                            {
                                Key = "HideLottery",
                                Value = table3rdDCS.Select("ProductID = 25").Any() ? (Utils.ToBoolean(table3rdDCS.Select("ProductID = 25").FirstOrDefault()["IsHidden"]) ||
                                !Utils.ToBoolean(table3rdDCS.Select("ProductID = 25").FirstOrDefault()["IsSync"])) : false
                            }
                        },
                EditItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                            new ObjectInfo { Key = "EnableKeno", Text = "Enable Keno" },
                            new ObjectInfo { Key = "EnableCockfighting", Text = "Enable Cockfighting" },
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
            model.AddEnableAndDetailProduct(table3rdDCS, 25, "Lottery", eFunctions.LotteryBetting);
            model.Setting.EnableItems.Add(new ObjectInfo
            {
                Key = "EnableGoldDeluxe",
                Value = !Utils.ToBoolean(tableCustomer.Rows[0]["DisableGoldDeluxe"]),
                Text = "Gold Deluxe"
            });
	     
            model.AddBettingLimitForSuper();

            return model;
        }

        /// <summary>
        /// Adds betting limit link for special admin.
        /// </summary>
        /// <param name="model">CustomerModel object.</param>
        /// <returns>CustomerModel context.</returns>
        private static CustomerModel AddBettingLimitForSuper(this CustomerModel model)
        {
            var userId = CurrentSession.Instance.UserId;
            var showBetSetting = CustomerModelExtensions.IsAuthorized(userId, eFunctions.CasinoBetting, AuthenticateAction.AuthenticationType.Read) ||
                CustomerModelExtensions.IsAuthorized(userId, eFunctions.GoldDeluxeBetting, AuthenticateAction.AuthenticationType.Read) ||
                CustomerModelExtensions.IsAuthorized(userId, eFunctions.KenoBetting, AuthenticateAction.AuthenticationType.Read) ||
                CustomerModelExtensions.IsAuthorized(userId, eFunctions.LiveCasinoBetting, AuthenticateAction.AuthenticationType.Read) ||
                CustomerModelExtensions.IsAuthorized(userId, eFunctions.RacingBetting, AuthenticateAction.AuthenticationType.Read) ||
                CustomerModelExtensions.IsAuthorized(userId, eFunctions.SportsbookBetting, AuthenticateAction.AuthenticationType.Read) ||
                CustomerModelExtensions.IsAuthorized(userId, eFunctions.VirtualSportsBetting, AuthenticateAction.AuthenticationType.Read) ||
                CustomerModelExtensions.IsAuthorized(userId, eFunctions.AllbetBetting, AuthenticateAction.AuthenticationType.Read) ||
                CustomerModelExtensions.IsAuthorized(userId, eFunctions.VoidbridgeBetting, AuthenticateAction.AuthenticationType.Read) ||
                CustomerModelExtensions.IsAuthorized(userId, eFunctions.NG2Betting, AuthenticateAction.AuthenticationType.Read) ||
                CustomerModelExtensions.IsAuthorized(userId, eFunctions.LotteryBetting, AuthenticateAction.AuthenticationType.Read);

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

        private static bool GetVirtualSportsStatus(DataTable table3rdDCS)
        {
            DataRow[] drowVirtualSports = table3rdDCS.Select("ProductID = 5");

            return drowVirtualSports.Length > 0 && !Utils.ToBoolean(drowVirtualSports[0]["isDisabled"]);
        }
    }
}
