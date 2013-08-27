// --------------------------------------------------------------------------
// <copyright file="LeftPanelService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.7.29">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.ExternalAccount.Module.LeftPanel.Repositories.Impl;
    using Fanex.ExternalAccount.Module.LeftPanel.Repositories.Interface;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeftPanelService : BaseService
    {
        private readonly ILeftPanelDao data = new LeftPanelDao();

        public override CustomerModel GetCustomerInfo(string username, string groupName = "ACCDDD")
        {
            var ds = this.data.GetCustomerInfo(username, groupName);
            var model = new CustomerModel();

            if (ds != null && ds.Tables.Count > 0)
            {
                var tableCustomer = ds.Tables[0];
                var tableLine = ds.Tables[1];
                var tableCash = ds.Tables[2];
                var tableTotalMember = ds.Tables[3];
                var roleId = Utils.ToInt32(tableCustomer.Rows[0]["roleid"]);

                model.BalanceInfo = new BalanceInfo()
                {
                    CurrencyId = new ObjectInfo { Value = (int)tableCustomer.Rows[0]["exchangeid"], Text = "Currency Id" },
                    Currency = new ObjectInfo { Value = tableCustomer.Rows[0]["currency"].ToString(), Text = "Currency" },
                    Balances = new List<ObjectInfo>()
                    {                        
                        new ObjectInfo { Value = (decimal)tableCash.Rows[0][this.GetCashColumn(roleId)], Text = "Total Cash" },
                        new ObjectInfo { Value = (decimal)tableCash.Rows[0][this.GetYesterdayCashColumn(roleId)], Text = "Yesterday Cash" },
                        new ObjectInfo { Value = (decimal)tableCash.Rows[0][this.GetOutstandingColumn(roleId)], Text = "Outstanding" },
                        new ObjectInfo { Value = (decimal)tableCustomer.Rows[0]["credit"], Text = "Given Credit" },
                        //new ObjectInfo { Value = (decimal)tableCustomer.Rows[0]["credit"] - (decimal)tableCash.Rows[0][this.GetOutstandingColumn(roleId)], Text = "Bet Credit" }
                    }
                };

                model.PersonalInfo = new PersonalInfo
                {
                    CustId = new ObjectInfo { Value = (int)tableCustomer.Rows[0]["custid"], Text = "Cust Id" },
                    Fax = new ObjectInfo { Value = Utils.ToString(tableCustomer.Rows[0]["fax"]), Text = "Fax" },
                    Mobile = new ObjectInfo { Value = Utils.ToString(tableCustomer.Rows[0]["mobilephone"]), Text = "Mobile" },
                    Name = new ObjectInfo { Value = Utils.ToString(tableCustomer.Rows[0]["firstname"]) + " " + Utils.ToString(tableCustomer.Rows[0]["lastname"]), Text = "Name" },
                    Nickname = new ObjectInfo { Value = Utils.ToString(tableCustomer.Rows[0]["username2"]), Text = "Nickname" },
                    Phone = new ObjectInfo { Value = Utils.ToString(tableCustomer.Rows[0]["phone"]), Text = "Phone" },
                    RoleId = new ObjectInfo { Value = roleId, Text = "Role Id" },
                    UplineRoleId = new ObjectInfo { Value = roleId + 1, Text = "Upline Role Id" },
                };

                model.Login = new List<ObjectInfo>
                {
                    new ObjectInfo { Value = tableCustomer.Rows[0]["site"], Text = "Site" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["createdate"]).ToString("M/d/yyyy H:mm tt"), Text = "Created Date" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["lastlogged"]).ToString("M/d/yyyy H:mm tt"), Text = "Last Login" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCash.Rows[0]["lastTxn"]).ToString("M/d/yyyy H:mm tt"), Text = "Last Txn", Tooltip = "Last Transaction" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["pwdexpiry"]).ToString("M/d/yyyy H:mm tt"), Text = "Password" },
                    new ObjectInfo { Value = tableCustomer.Rows[0]["lastloginip"], Text = "Last Login IP" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["LastChangedSecurityCodeDate"]).ToString("M/d/yyyy H:mm tt"), Text = "Security Code" },
                };

                if (roleId != this.Member)
                {
                    model.Login.Add(new ObjectInfo { Value = (bool)tableCustomer.Rows[0]["NicknameLoginOnly"] ? "Nickname Only" : "Username/Nickname", Text = "Login Options" });
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = tableCash.Rows[0][this.GetUsedCreditColumn(roleId)], Text = "Used Credit" });
                }

                model.DownlineList = new DownlineList();

                var downlines = this.GetDownlineList((int)model.PersonalInfo.CustId.Value, 100, 1);

                if (downlines != null && downlines.Rows.Count > 0)
                {
                    foreach (DataRow item in downlines.Rows)
                    {
                        model.DownlineList.Downlines.Add(new ObjectInfo { Text = item["username"].ToString() });
                    }
                }

                if (roleId == this.Super)
                {
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 1203, Text = "Total Balance" });
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 4521, Text = "Yesterday Balance" });
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 444521, Text = "Master Outstanding" });

                    //model.BalanceInfo.Statuses.Add(new Status { TotalActive = 3, TotalClosed = 2, TotalSuspended = 4, Text = "Masters A/C/S", Tooltip="Total Masters Active/Closed/Suspended" });
                    //model.BalanceInfo.Statuses.Add(new Status { TotalActive = 3, TotalClosed = 1, TotalSuspended = 0, Text = "Agents A/C/S", Tooltip = "Total Agents Active/Closed/Suspended" });
                    //model.BalanceInfo.Statuses.Add(new Status { TotalActive = 5, TotalClosed = 0, TotalSuspended = 9, Text = "Members A/C/S", Tooltip = "Total Members Active/Closed/Suspended" });
                    
                    model.PersonalInfo.Downlines = new List<ObjectInfo> { new ObjectInfo { Value = (string)tableLine.Rows[0]["superName"], Text = "Super" } };
                    model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = (string)tableLine.Rows[0]["superName"], Text = "Super" };
                    
                    model.Setting = new Setting
                    {
                        EnableItems = new List<ObjectInfo>
                        {                            
                            new ObjectInfo { Key = "Closed", Value = (bool)tableCustomer.Rows[0]["closed"], Text = "Closed" },                            
                            new ObjectInfo { Key = "Suspended", Value = (bool)tableCustomer.Rows[0]["suspended"], Text = "Suspened" },
                            new ObjectInfo { Key = "Outright", Value = !(bool)tableCustomer.Rows[0]["disableOutright"], Text = "Allow Outright" },
                            new ObjectInfo { Key = "Casino", Value = !(bool)tableCustomer.Rows[0]["disableCasino"], Text = "Enable Casino" },
                            new ObjectInfo { Key = "NumberGame", Value = !(bool)tableCustomer.Rows[0]["disableNumber"], Text = "Enable Num Game", Tooltip = "Enable Number Game" },
                            new ObjectInfo { Key = "LiveCasino", Value = !(bool)tableCustomer.Rows[0]["disableLiveCasino"], Text = "Enable Live Casino" },                            
                            new ObjectInfo { Key = "VirtualSports", Value = !(bool)tableCustomer.Rows[0]["disableVirtualSports"], Text = "Virtual Sports" },
                            new ObjectInfo { Key = "DoubleComm", Value = (bool)tableCustomer.Rows[0]["doubleCommLosing"], Text = "Double Comm", Tooltip = "Enable Double Commission On Losing Ticket" }
                        },
                        EditItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                            new ObjectInfo { Key = "EnablePoker", Text = "Enable Poker" },
                        },
                        SyncItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "SyncCasino", Value = (bool)tableCustomer.Rows[0]["syncCasino"], Text = "Casino Synced" },
                            new ObjectInfo { Key = "SyncBingo", Value = (bool)tableCustomer.Rows[0]["syncBingo"], Text = "Bingo Synced" },
                            new ObjectInfo { Key = "SyncLiveCasino", Value = (bool)tableCustomer.Rows[0]["syncLiveCasino"], Text = "Live Casino Synced" },
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
                }
                else if (roleId == this.Master)
                {
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 1203, Text = "Total Balance" });
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 4521, Text = "Yesterday Balance" });
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 444521, Text = "Agent Outstanding" });

                    //model.BalanceInfo.Statuses.Add(new Status { TotalActive = 3, TotalClosed = 2, TotalSuspended = 4, Text = "Agents A/C/S", Tooltip = "Total Agents Active/Closed/Suspended" });
                    //model.BalanceInfo.Statuses.Add(new Status { TotalActive = 3, TotalClosed = 2, TotalSuspended = 4, Text = "Members A/C/S", Tooltip = "Total Members Active/Closed/Suspended" });

                    model.PersonalInfo.UplineId = new ObjectInfo { Value = (int)tableLine.Rows[0]["superID"], Text = "Super ID" };
                    model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = (string)tableLine.Rows[0]["masterName"], Text = "Master" };
                    model.PersonalInfo.Downlines = new List<ObjectInfo>
                    { 
                        new ObjectInfo { Value = (string)tableLine.Rows[0]["superName"], Text = "Super" },
                        new ObjectInfo { Value = (string)tableLine.Rows[0]["masterName"], Text = "Master" } 
                    };

                    model.Setting = new Setting
                    {
                        EnableItems = new List<ObjectInfo>
                        {                            
                            new ObjectInfo { Key = "Closed", Value = (bool)tableCustomer.Rows[0]["closed"], Text = "Closed" },                            
                            new ObjectInfo { Key = "Suspended", Value = (bool)tableCustomer.Rows[0]["suspended"], Text = "Suspened" },
                            new ObjectInfo { Key = "AutoPT", Value = !(bool)tableCash.Rows[0]["disableAutoMPT"], Text = "Allow Auto PT" },
                            new ObjectInfo { Key = "Outright", Value = !(bool)tableCustomer.Rows[0]["disableOutright"], Text = "Allow Outright" },
                            new ObjectInfo { Key = "Casino", Value = !(bool)tableCustomer.Rows[0]["disableCasino"], Text = "Enable Casino" },
                            new ObjectInfo { Key = "NumberGame", Value = !(bool)tableCustomer.Rows[0]["disableNumber"], Text = "Enable Num Game", Tooltip = "Enable Number Game" },
                            new ObjectInfo { Key = "LiveCasino", Value = !(bool)tableCustomer.Rows[0]["disableLiveCasino"], Text = "Enable Live Casino" },                            
                            new ObjectInfo { Key = "VirtualSports", Value = !(bool)tableCustomer.Rows[0]["disableVirtualSports"], Text = "Virtual Sports" },
                            new ObjectInfo { Key = "DoubleComm", Value = (bool)tableCustomer.Rows[0]["doubleCommLosing"], Text = "Double Comm", Tooltip = "Enable Double Commission On Losing Ticket" },                            
                        },
                        EditItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                            new ObjectInfo { Key = "EnablePoker", Text = "Enable Poker" },
                        },
                        SyncItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "SyncCasino", Value = (bool)tableCustomer.Rows[0]["syncCasino"], Text = "Casino Synced" },
                            new ObjectInfo { Key = "SyncBingo", Value = (bool)tableCustomer.Rows[0]["syncBingo"], Text = "Bingo Synced" },
                            new ObjectInfo { Key = "SyncLiveCasino", Value = (bool)tableCustomer.Rows[0]["syncLiveCasino"], Text = "Live Casino Synced" },
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
                }
                else if (roleId == this.Agent)
                {
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 1203, Text = "Total Balance" });
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 4521, Text = "Yesterday Balance" });
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 444521, Text = "Member Outstanding" });

                    //model.BalanceInfo.Statuses.Add(new Status { TotalActive = 3, TotalClosed = 2, TotalSuspended = 4, Text = "Members A/C/S", Tooltip = "Total Members Active/Closed/Suspended" });

                    model.PersonalInfo.UplineId = new ObjectInfo { Value = (int)tableLine.Rows[0]["masterID"], Text = "Master ID" };
                    model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = (string)tableLine.Rows[0]["agentName"], Text = "Agent" };
                    model.PersonalInfo.Downlines = new List<ObjectInfo>
                    { 
                        new ObjectInfo { Value = (string)tableLine.Rows[0]["superName"], Text = "Super" },
                        new ObjectInfo { Value = (string)tableLine.Rows[0]["masterName"], Text = "Master" },
                        new ObjectInfo { Value = (string)tableLine.Rows[0]["agentName"], Text = "Agent" }
                    };

                    model.Setting = new Setting
                    {
                        EnableItems = new List<ObjectInfo>
                        {                            
                            new ObjectInfo { Key = "Closed", Value = (bool)tableCustomer.Rows[0]["closed"], Text = "Closed" },                            
                            new ObjectInfo { Key = "Suspended", Value = (bool)tableCustomer.Rows[0]["suspended"], Text = "Suspened" },
                            new ObjectInfo { Key = "Outright", Value = !(bool)tableCustomer.Rows[0]["disableOutright"], Text = "Allow Outright" },
                            new ObjectInfo { Key = "Casino", Value = !(bool)tableCustomer.Rows[0]["disableCasino"], Text = "Enable Casino" },
                            new ObjectInfo { Key = "NumberGame", Value = !(bool)tableCustomer.Rows[0]["disableNumber"], Text = "Enable Num Game", Tooltip = "Enable Number Game" },
                            new ObjectInfo { Key = "LiveCasino", Value = !(bool)tableCustomer.Rows[0]["disableLiveCasino"], Text = "Enable Live Casino" },                            
                            new ObjectInfo { Key = "VirtualSports", Value = !(bool)tableCustomer.Rows[0]["disableVirtualSports"], Text = "Virtual Sports" },
                            new ObjectInfo { Key = "DoubleComm", Value = (bool)tableCustomer.Rows[0]["doubleCommLosing"], Text = "Double Comm", Tooltip = "Enable Double Commission On Losing Ticket" },                            
                        },
                        EditItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                            new ObjectInfo { Key = "EnablePoker", Text = "Enable Poker" },
                        },
                        SyncItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "SyncCasino", Value = (bool)tableCustomer.Rows[0]["syncCasino"], Text = "Casino Synced" },
                            new ObjectInfo { Key = "SyncBingo", Value = (bool)tableCustomer.Rows[0]["syncBingo"], Text = "Bingo Synced" },
                            new ObjectInfo { Key = "SyncLiveCasino", Value = (bool)tableCustomer.Rows[0]["syncLiveCasino"], Text = "Live Casino Synced" },
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
                }
                else
                {
                    model.PersonalInfo.UplineId = new ObjectInfo { Value = (int)tableLine.Rows[0]["agentID"], Text = "Agent ID" };
                    model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = (string)tableLine.Rows[0]["memberName"], Text = "Member" };
                    model.PersonalInfo.Downlines = new List<ObjectInfo>
                    { 
                        new ObjectInfo { Value = (string)tableLine.Rows[0]["superName"], Text = "Super" },
                        new ObjectInfo { Value = (string)tableLine.Rows[0]["masterName"], Text = "Master" },
                        new ObjectInfo { Value = (string)tableLine.Rows[0]["agentName"], Text = "Agent" },
                    };

                    model.Setting = new Setting
                    {
                        EnableItems = new List<ObjectInfo>
                        {                            
                            new ObjectInfo { Key = "Closed", Value = (bool)tableCustomer.Rows[0]["closed"], Text = "Closed" },                            
                            new ObjectInfo { Key = "Suspended", Value = (bool)tableCustomer.Rows[0]["suspended"], Text = "Suspened" },
                            new ObjectInfo { Key = "Outright", Value = !(bool)tableCustomer.Rows[0]["disableOutright"], Text = "Allow Outright" },
                            new ObjectInfo { Key = "Casino", Value = !(bool)tableCustomer.Rows[0]["disableCasino"], Text = "Enable Casino" },
                            new ObjectInfo { Key = "NumberGame", Value = !(bool)tableCustomer.Rows[0]["disableNumber"], Text = "Enable Num Game", Tooltip = "Enable Number Game" },
                            new ObjectInfo { Key = "LiveCasino", Value = !(bool)tableCustomer.Rows[0]["disableLiveCasino"], Text = "Enable Live Casino" },                            
                            new ObjectInfo { Key = "VirtualSports", Value = !(bool)tableCustomer.Rows[0]["disableVirtualSports"], Text = "Virtual Sports" },
                            new ObjectInfo { Key = "DoubleComm", Value = (bool)tableCustomer.Rows[0]["doubleCommLosing"], Text = "Double Comm", Tooltip = "Enable Double Commission On Losing Ticket" },                            
                        },
                        EditItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                            new ObjectInfo { Key = "EnablePoker", Text = "Enable Poker" },
                        },
                        SyncItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Key = "SyncCasino", Value = (bool)tableCustomer.Rows[0]["syncCasino"], Text = "Casino Synced" },
                            new ObjectInfo { Key = "SyncBingo", Value = (bool)tableCustomer.Rows[0]["syncBingo"], Text = "Bingo Synced" },
                            new ObjectInfo { Key = "SyncLiveCasino", Value = (bool)tableCustomer.Rows[0]["syncLiveCasino"], Text = "Live Casino Synced" },
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
                }

                return model;
            }

            return null;
        }

        public override Result Update(int id, int uplineId, CustomerModel model)
        {
            throw new NotImplementedException();
        }

        public override DataTable GetDownlineList(int custid, int pageSize, int pageIndex)
        {
            var dataTable = new DataTable();
            var dr = this.data.GetDownlineList(custid, pageSize, pageIndex);

            if (dr != null && dr.ResultSet != null && dr.ResultSet.Tables.Count > 0)
            {
                return dr.ResultSet.Tables[0];
            }

            return null;
        }

        public override DataRowCollection GetOnlineList(int custid)
        {
            throw new NotImplementedException();
        }
    }
}