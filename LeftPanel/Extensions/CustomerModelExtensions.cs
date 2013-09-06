// --------------------------------------------------------------------------
// <copyright file="CustomerModelExtensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.8.30">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Collections.Generic;
    using System.Data;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CustomerModelExtensions
    {
        public static CustomerModel SuperInfo(this CustomerModel model, DataSet ds)
        {
            var tableCustomer = ds.Tables[0];
            var tableLine = ds.Tables[1];
            var tableCash = ds.Tables[2];

            model.BalanceInfo.Balances.Add(new ObjectInfo { 
                Value = Utils.ToDecimal(tableCash.Rows[0]["superCash"]) +
                        Utils.ToDecimal(tableCash.Rows[0]["masterCash"]) +
                        Utils.ToDecimal(tableCash.Rows[0]["agentCash"]) + 
                        Utils.ToDecimal(tableCash.Rows[0]["customerCash"]), 
                Text = "Total Balance" 
            });
            model.BalanceInfo.Balances.Add(new ObjectInfo { 
                Value = Utils.ToDecimal(tableCash.Rows[0]["yesterday_superCash"]) +
                        Utils.ToDecimal(tableCash.Rows[0]["yesterday_masterCash"]) +
                        Utils.ToDecimal(tableCash.Rows[0]["yesterday_agentCash"]) + 
                        Utils.ToDecimal(tableCash.Rows[0]["yesterday_customerCash"]), 
                Text = "Yesterday Balance" });
            model.BalanceInfo.Balances.Add(new ObjectInfo { Value = Utils.ToDecimal(tableCash.Rows[0]["customerOutstanding"]), Text = "Master Outstanding" });

            //model.BalanceInfo.Statuses.Add(new Status { TotalActive = 3, TotalClosed = 2, TotalSuspended = 4, Text = "Masters A/C/S", Tooltip="Total Masters Active/Closed/Suspended" });
            //model.BalanceInfo.Statuses.Add(new Status { TotalActive = 3, TotalClosed = 1, TotalSuspended = 0, Text = "Agents A/C/S", Tooltip = "Total Agents Active/Closed/Suspended" });
            //model.BalanceInfo.Statuses.Add(new Status { TotalActive = 5, TotalClosed = 0, TotalSuspended = 9, Text = "Members A/C/S", Tooltip = "Total Members Active/Closed/Suspended" });

            model.PersonalInfo.Downlines = new List<ObjectInfo> { new ObjectInfo { Key = tableLine.Rows[0].ToInt32("superID"), Value = Utils.ToString(tableLine.Rows[0]["superName"]), Text = "Super" } };
            model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["superName"]), Text = "Super" };

            model.Setting = new Setting
            {
                EnableItems = new List<ObjectInfo>
                        {                            
                            new ObjectInfo { Key = "Closed", Value = Utils.ToBoolean(tableCustomer.Rows[0]["closed"]), Text = "Closed" },                            
                            new ObjectInfo { Key = "Suspended", Value = Utils.ToBoolean(tableCustomer.Rows[0]["suspended"]), Text = "Suspened" },
                            new ObjectInfo { Key = "Outright", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableOutright"]), Text = "Allow Outright" },
                            new ObjectInfo { Key = "Casino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableCasino"]), Text = "Enable Casino" },
                            new ObjectInfo { Key = "NumberGame", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableNumber"]), Text = "Enable Num Game", Tooltip = "Enable Number Game" },
                            new ObjectInfo { Key = "LiveCasino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableLiveCasino"]), Text = "Enable Live Casino" },                            
                            new ObjectInfo { Key = "VirtualSports", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableVirtualSports"]), Text = "Virtual Sports" },
                            new ObjectInfo { Key = "DoubleComm", Value = Utils.ToBoolean(tableCustomer.Rows[0]["doubleCommLosing"]), Text = "Double Comm", Tooltip = "Enable Double Commission On Losing Ticket" }
                        },
                EditItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                            new ObjectInfo { Key = "EnablePoker", Text = "Enable Poker" },
                        },
                SyncItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "SyncCasino", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncCasino"]), Text = "Casino Synced" },
                            new ObjectInfo { Key = "SyncBingo", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncBingo"]), Text = "Bingo Synced" },
                            new ObjectInfo { Key = "SyncLiveCasino", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncLiveCasino"]), Text = "Live Casino Synced" },
                        },
                Details = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "Info", Value = "Edit Info" },
                            new ObjectInfo { Key = "PT", Value = "Edit Position Taking" },
                            new ObjectInfo { Key = "AddNew", Value = "Add New" },
                            new ObjectInfo { Key = "Transfer", Value = "Edit Transfer Condition" },
                            new ObjectInfo { Key = "Racing", Value = "Edit Racing" },
                            new ObjectInfo { Key = "Casino", Value = "Edit Casino" },
                            new ObjectInfo { Key = "Finance", Value = "Edit Finance" },
                            new ObjectInfo { Key = "Bingo", Value = "Edit Bingo" },
                            new ObjectInfo { Key = "Poker", Value = "Edit Poker" },
                            new ObjectInfo { Key = "LiveCasino", Value = "Edit Live Casino" },
                            new ObjectInfo { Key = "VirtualSports", Value = "Edit Virtual Sports" },
                        }
            };

            return model;
        }

        public static CustomerModel MasterInfo(this CustomerModel model, DataSet ds)
        {
            var tableCustomer = ds.Tables[0];
            var tableLine = ds.Tables[1];
            var tableCash = ds.Tables[2];

            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = Utils.ToDecimal(tableCash.Rows[0]["masterCash"]) +
                        Utils.ToDecimal(tableCash.Rows[0]["agentCash"]) +
                        Utils.ToDecimal(tableCash.Rows[0]["customerCash"]),
                Text = "Total Balance"
            });
            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = Utils.ToDecimal(tableCash.Rows[0]["yesterday_masterCash"]) +
                        Utils.ToDecimal(tableCash.Rows[0]["yesterday_agentCash"]) +
                        Utils.ToDecimal(tableCash.Rows[0]["yesterday_customerCash"]),
                Text = "Yesterday Balance"
            }); 
            model.BalanceInfo.Balances.Add(new ObjectInfo { Value = Utils.ToDecimal(tableCash.Rows[0]["customerOutstanding"]), Text = "Agent Outstanding" });

            //model.BalanceInfo.Statuses.Add(new Status { TotalActive = 3, TotalClosed = 2, TotalSuspended = 4, Text = "Agents A/C/S", Tooltip = "Total Agents Active/Closed/Suspended" });
            //model.BalanceInfo.Statuses.Add(new Status { TotalActive = 3, TotalClosed = 2, TotalSuspended = 4, Text = "Members A/C/S", Tooltip = "Total Members Active/Closed/Suspended" });

            model.PersonalInfo.UplineId = new ObjectInfo { Value = Utils.ToInt32(tableLine.Rows[0]["superID"]), Text = "Super ID" };
            model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["masterName"]), Text = "Master" };
            model.PersonalInfo.Downlines = new List<ObjectInfo>
                    { 
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("superID"), Value = Utils.ToString(tableLine.Rows[0]["superName"]), Text = "Super" },
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("masterID"), Value = Utils.ToString(tableLine.Rows[0]["masterName"]), Text = "Master" } 
                    };

            model.Setting = new Setting
            {
                EnableItems = new List<ObjectInfo>
                        {                            
                            new ObjectInfo { Key = "Closed", Value = Utils.ToBoolean(tableCustomer.Rows[0]["closed"]), Text = "Closed" },                            
                            new ObjectInfo { Key = "Suspended", Value = Utils.ToBoolean(tableCustomer.Rows[0]["suspended"]), Text = "Suspened" },
                            new ObjectInfo { Key = "AutoPT", Value = !Utils.ToBoolean(tableCash.Rows[0]["disableAutoMPT"]), Text = "Allow Auto PT" },
                            new ObjectInfo { Key = "Outright", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableOutright"]), Text = "Allow Outright" },
                            new ObjectInfo { Key = "Casino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableCasino"]), Text = "Enable Casino" },
                            new ObjectInfo { Key = "NumberGame", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableNumber"]), Text = "Enable Num Game", Tooltip = "Enable Number Game" },
                            new ObjectInfo { Key = "LiveCasino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableLiveCasino"]), Text = "Enable Live Casino" },                            
                            new ObjectInfo { Key = "VirtualSports", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableVirtualSports"]), Text = "Virtual Sports" },
                            new ObjectInfo { Key = "DoubleComm", Value = Utils.ToBoolean(tableCustomer.Rows[0]["doubleCommLosing"]), Text = "Double Comm", Tooltip = "Enable Double Commission On Losing Ticket" },                            
                        },
                EditItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                            new ObjectInfo { Key = "EnablePoker", Text = "Enable Poker" },
                        },
                SyncItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "SyncCasino", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncCasino"]), Text = "Casino Synced" },
                            new ObjectInfo { Key = "SyncBingo", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncBingo"]), Text = "Bingo Synced" },
                            new ObjectInfo { Key = "SyncLiveCasino", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncLiveCasino"]), Text = "Live Casino Synced" },
                        },
                Details = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "Info", Value = "Edit Info" },
                            new ObjectInfo { Key = "PT", Value = "Edit Position Taking" },
                            new ObjectInfo { Key = "AddNew", Value = "Add New" },
                            new ObjectInfo { Key = "Transfer", Value = "Edit Transfer Condition" },
                            new ObjectInfo { Key = "Racing", Value = "Edit Racing" },
                            new ObjectInfo { Key = "Casino", Value = "Edit Casino" },
                            new ObjectInfo { Key = "Finance", Value = "Edit Finance" },
                            new ObjectInfo { Key = "Bingo", Value = "Edit Bingo" },
                            new ObjectInfo { Key = "Poker", Value = "Edit Poker" },
                            new ObjectInfo { Key = "LiveCasino", Value = "Edit Live Casino" },
                            new ObjectInfo { Key = "VirtualSports", Value = "Edit Virtual Sports" },
                        }
            };

            return model;
        }

        public static CustomerModel AgentInfo(this CustomerModel model, DataSet ds)
        {
            var tableCustomer = ds.Tables[0];
            var tableLine = ds.Tables[1];
            var tableCash = ds.Tables[2];

            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = Utils.ToDecimal(tableCash.Rows[0]["agentCash"]) +
                        Utils.ToDecimal(tableCash.Rows[0]["customerCash"]),
                Text = "Total Balance"
            });
            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = Utils.ToDecimal(tableCash.Rows[0]["yesterday_agentCash"]) +
                        Utils.ToDecimal(tableCash.Rows[0]["yesterday_customerCash"]),
                Text = "Yesterday Balance"
            }); 
            model.BalanceInfo.Balances.Add(new ObjectInfo { Value = Utils.ToDecimal(tableCash.Rows[0]["customerOutstanding"]), Text = "Member Outstanding" });

            //model.BalanceInfo.Statuses.Add(new Status { TotalActive = 3, TotalClosed = 2, TotalSuspended = 4, Text = "Members A/C/S", Tooltip = "Total Members Active/Closed/Suspended" });

            model.PersonalInfo.UplineId = new ObjectInfo { Value = Utils.ToInt32(tableLine.Rows[0]["masterID"]), Text = "Master ID" };
            model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["agentName"]), Text = "Agent" };
            model.PersonalInfo.Downlines = new List<ObjectInfo>
                    { 
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("superID"), Value = Utils.ToString(tableLine.Rows[0]["superName"]), Text = "Super" },
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("masterID"), Value = Utils.ToString(tableLine.Rows[0]["masterName"]), Text = "Master" },
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("agentID"), Value = Utils.ToString(tableLine.Rows[0]["agentName"]), Text = "Agent" }
                    };

            model.Setting = new Setting
            {
                EnableItems = new List<ObjectInfo>
                        {                            
                            new ObjectInfo { Key = "Closed", Value = Utils.ToBoolean(tableCustomer.Rows[0]["closed"]), Text = "Closed" },                            
                            new ObjectInfo { Key = "Suspended", Value = Utils.ToBoolean(tableCustomer.Rows[0]["suspended"]), Text = "Suspened" },
                            new ObjectInfo { Key = "Outright", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableOutright"]), Text = "Allow Outright" },
                            new ObjectInfo { Key = "Casino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableCasino"]), Text = "Enable Casino" },
                            new ObjectInfo { Key = "NumberGame", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableNumber"]), Text = "Enable Num Game", Tooltip = "Enable Number Game" },
                            new ObjectInfo { Key = "LiveCasino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableLiveCasino"]), Text = "Enable Live Casino" },                            
                            new ObjectInfo { Key = "VirtualSports", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableVirtualSports"]), Text = "Virtual Sports" },
                            new ObjectInfo { Key = "DoubleComm", Value = Utils.ToBoolean(tableCustomer.Rows[0]["doubleCommLosing"]), Text = "Double Comm", Tooltip = "Enable Double Commission On Losing Ticket" },                            
                        },
                EditItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                            new ObjectInfo { Key = "EnablePoker", Text = "Enable Poker" },
                        },
                SyncItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "SyncCasino", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncCasino"]), Text = "Casino Synced" },
                            new ObjectInfo { Key = "SyncBingo", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncBingo"]), Text = "Bingo Synced" },
                            new ObjectInfo { Key = "SyncLiveCasino", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncLiveCasino"]), Text = "Live Casino Synced" },
                        },
                Details = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "Info", Value = "Edit Info" },
                            new ObjectInfo { Key = "PT", Value = "Edit Position Taking" },
                            new ObjectInfo { Key = "AddNew", Value = "Add New" },
                            new ObjectInfo { Key = "Transfer", Value = "Edit Transfer Condition" },
                            new ObjectInfo { Key = "Racing", Value = "Edit Racing" },
                            new ObjectInfo { Key = "Casino", Value = "Edit Casino" },
                            new ObjectInfo { Key = "Finance", Value = "Edit Finance" },
                            new ObjectInfo { Key = "Bingo", Value = "Edit Bingo" },
                            new ObjectInfo { Key = "Poker", Value = "Edit Poker" },
                            new ObjectInfo { Key = "LiveCasino", Value = "Edit Live Casino" },
                            new ObjectInfo { Key = "VirtualSports", Value = "Edit Virtual Sports" },
                        }
            };

            return model;
        }

        public static CustomerModel MemberInfo(this CustomerModel model, DataSet ds)
        {
            var tableCustomer = ds.Tables[0];
            var tableLine = ds.Tables[1];

            model.PersonalInfo.UplineId = new ObjectInfo { Value = Utils.ToInt32(tableLine.Rows[0]["agentID"]), Text = "Agent ID" };
            model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["memberName"]), Text = "Member" };
            model.PersonalInfo.Downlines = new List<ObjectInfo>
                    { 
                        new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["superName"]), Text = "Super" },
                        new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["masterName"]), Text = "Master" },
                        new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["agentName"]), Text = "Agent" },
                    };

            model.Setting = new Setting
            {
                EnableItems = new List<ObjectInfo>
                        {                            
                            new ObjectInfo { Key = "Closed", Value = Utils.ToBoolean(tableCustomer.Rows[0]["closed"]), Text = "Closed" },                            
                            new ObjectInfo { Key = "Suspended", Value = Utils.ToBoolean(tableCustomer.Rows[0]["suspended"]), Text = "Suspened" },
                            new ObjectInfo { Key = "Outright", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableOutright"]), Text = "Allow Outright" },
                            new ObjectInfo { Key = "Casino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableCasino"]), Text = "Enable Casino" },
                            new ObjectInfo { Key = "NumberGame", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableNumber"]), Text = "Enable Num Game", Tooltip = "Enable Number Game" },
                            new ObjectInfo { Key = "LiveCasino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableLiveCasino"]), Text = "Enable Live Casino" },                            
                            new ObjectInfo { Key = "VirtualSports", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableVirtualSports"]), Text = "Virtual Sports" },
                            new ObjectInfo { Key = "DoubleComm", Value = Utils.ToBoolean(tableCustomer.Rows[0]["doubleCommLosing"]), Text = "Double Comm", Tooltip = "Enable Double Commission On Losing Ticket" },                            
                        },
                EditItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                            new ObjectInfo { Key = "EnablePoker", Text = "Enable Poker" },
                        },
                SyncItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "SyncCasino", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncCasino"]), Text = "Casino Synced" },
                            new ObjectInfo { Key = "SyncBingo", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncBingo"]), Text = "Bingo Synced" },
                            new ObjectInfo { Key = "SyncLiveCasino", Value = Utils.ToBoolean(tableCustomer.Rows[0]["syncLiveCasino"]), Text = "Live Casino Synced" },
                        },
                Details = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "Info", Value = "Edit Info" },
                            new ObjectInfo { Key = "PT", Value = "Edit Position Taking" },
                            new ObjectInfo { Key = "Transfer", Value = "Edit Transfer Condition" },
                            new ObjectInfo { Key = "Racing", Value = "Edit Racing" },
                            new ObjectInfo { Key = "Casino", Value = "Edit Casino" },
                            new ObjectInfo { Key = "Finance", Value = "Edit Finance" },
                            new ObjectInfo { Key = "Bingo", Value = "Edit Bingo" },
                            new ObjectInfo { Key = "Poker", Value = "Edit Poker" },
                            new ObjectInfo { Key = "LiveCasino", Value = "Edit Live Casino" },
                            new ObjectInfo { Key = "VirtualSports", Value = "Edit Virtual Sports" },
                        }
            };

            return model;
        }
    }
}
