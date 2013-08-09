// --------------------------------------------------------------------------
// <copyright file="LeftPanelService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.7.29">Created</change>
// </history>

namespace Fanex.CustomerSetting.Module.LeftPanel.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.CustomerSetting.Module.LeftPanel.Repositories.Impl;
    using Fanex.CustomerSetting.Module.LeftPanel.Repositories.Interface;
    using Fanex.CustomerSetting.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeftPanelService : BaseService
    {
        private readonly ILeftPanelDao data = new LeftPanelDao();

        public override CustomerModel GetCustomerInfo(string username)
        {
            var ds = this.data.GetCustomerInfo(username);
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
                        new ObjectInfo { Value = (decimal)tableCustomer.Rows[0]["credit"] - (decimal)tableCash.Rows[0][this.GetOutstandingColumn(roleId)], Text = "Bet Credit" }
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

                model.Login = new Login
                {
                    CreatedDate = new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["createdate"]).ToString("M/d/yyyy H:mm tt"), Text = "Created Date" },
                    IP = new ObjectInfo { Value = tableCustomer.Rows[0]["lastloginip"], Text = "Last Login IP" },
                    LastTrans = new ObjectInfo { Value = Utils.ToDateTime(tableCash.Rows[0]["lastTxn"]).ToString("M/d/yyyy H:mm tt"), Text = "Last Txn" },
                    LoginTime = new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["lastlogged"]).ToString("M/d/yyyy H:mm tt"), Text = "Last Login" },
                    Password = new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["pwdexpiry"]).ToString("M/d/yyyy H:mm tt"), Text = "Password" },
                    SecurityCode = new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["LastChangedSecurityCodeDate"]).ToString("M/d/yyyy H:mm tt"), Text = "Security Code" },
                    Site = new ObjectInfo { Value = tableCustomer.Rows[0]["site"], Text = "Site" }
                };

                if (roleId != this.Member)
                {
                    model.Login.LoginOption = new ObjectInfo { Value = (bool)tableCustomer.Rows[0]["NicknameLoginOnly"] ? "Nickname Only" : "Username/Nickname", Text = "Login Options" };
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = tableCash.Rows[0][this.GetUsedCreditColumn(roleId)], Text = "Used Credit" });
                }

                if (roleId == this.Super)
                {
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 1203, Text = "Total Balance" });
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 4521, Text = "Yesterday Balance" });
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 444521, Text = "Master Outstanding" });

                    model.BalanceInfo.Statuses.Add(new Status { TotalActive = 3, TotalClosed = 2, TotalSuspended = 4, Text = "Masters A/C/S", Tooltip="Total Masters Active/Closed/Suspended" });
                    model.BalanceInfo.Statuses.Add(new Status { TotalActive = 3, TotalClosed = 1, TotalSuspended = 0, Text = "Agents A/C/S", Tooltip = "Total Agents Active/Closed/Suspended" });
                    model.BalanceInfo.Statuses.Add(new Status { TotalActive = 5, TotalClosed = 0, TotalSuspended = 9, Text = "Members A/C/S", Tooltip = "Total Members Active/Closed/Suspended" });
                    
                    model.PersonalInfo.Downlines = new List<ObjectInfo> { new ObjectInfo { Value = (string)tableLine.Rows[0]["superName"], Text = "Super" } };
                    model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = (string)tableLine.Rows[0]["superName"], Text = "Super" };
                    model.DownlineList = new DownlineList
                    {
                        Downlines = new List<ObjectInfo>
                        {
                            new ObjectInfo { Text = "Testcs00"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs02"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs04"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs01"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs03"  },
                            new ObjectInfo { Text = "Testcs04"  },
                            new ObjectInfo { Text = "Testcs00"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs02"  },
                            new ObjectInfo { Text = "Testcs00"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs02"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs04"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs01"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs03"  },
                            new ObjectInfo { Text = "Testcs04"  },
                            new ObjectInfo { Text = "Testcs00"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs02"  }, new ObjectInfo { Text = "Testcs00"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs02"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs04"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs01"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs03"  },
                            new ObjectInfo { Text = "Testcs04"  },
                            new ObjectInfo { Text = "Testcs00"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs02"  }, new ObjectInfo { Text = "Testcs00"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs02"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs04"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs01"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs03"  },
                            new ObjectInfo { Text = "Testcs04"  },
                            new ObjectInfo { Text = "Testcs00"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs02"  }, new ObjectInfo { Text = "Testcs00"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs02"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs04"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs01"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs03"  },
                            new ObjectInfo { Text = "Testcs04"  },
                            new ObjectInfo { Text = "Testcs00"  },
                            new ObjectInfo { Text = "SuperTestcs0101004"  },
                            new ObjectInfo { Text = "Testcs02"  },
                        }
                    };

                    model.Setting = new Setting
                    {
                        EnableItems = new List<ObjectInfo>
                        {
                            new ObjectInfo { Value = true, Text = "Special" },
                            new ObjectInfo { Value = false, Text = "Closed" },
                            new ObjectInfo { Value = true, Text = "Special" },
                            new ObjectInfo { Value = false, Text = "Suspened" },
                            new ObjectInfo { Value = false, Text = "Allow Outright" },
                            new ObjectInfo { Value = false, Text = "Enable Casino" },
                            new ObjectInfo { Value = true, Text = "Enable Live Casino" },
                            new ObjectInfo { Value = true, Text = "Double Comm" },
                            new ObjectInfo { Value = true, Text = "Selected Betting" },
                            new ObjectInfo { Value = false, Text = "Internal" },
                            new ObjectInfo { Value = true, Text = "Enable Finance" },
                            new ObjectInfo { Value = false, Text = "Enable Num Game" },
                            new ObjectInfo { Value = true, Text = "Virtual Sports" }
                        }
                    };
                }
                else if (roleId == this.Master)
                {
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 1203, Text = "Total Balance" });
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 4521, Text = "Yesterday Balance" });
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 444521, Text = "Agent Outstanding" });

                    model.BalanceInfo.Statuses.Add(new Status { TotalActive = 3, TotalClosed = 2, TotalSuspended = 4, Text = "Agents A/C/S", Tooltip = "Total Agents Active/Closed/Suspended" });
                    model.BalanceInfo.Statuses.Add(new Status { TotalActive = 3, TotalClosed = 2, TotalSuspended = 4, Text = "Members A/C/S", Tooltip = "Total Members Active/Closed/Suspended" });

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
                            new ObjectInfo { Value = true, Text = "Special" },
                            new ObjectInfo { Value = false, Text = "Closed" },
                            new ObjectInfo { Value = true, Text = "Special" },
                            new ObjectInfo { Value = false, Text = "Suspened" },
                            new ObjectInfo { Value = false, Text = "Allow Outright" },
                            new ObjectInfo { Value = false, Text = "Enable Casino" },
                            new ObjectInfo { Value = true, Text = "Enable Live Casino" },
                            new ObjectInfo { Value = true, Text = "Double Comm" },
                            new ObjectInfo { Value = true, Text = "Selected Betting" },
                            new ObjectInfo { Value = false, Text = "Internal" },
                            new ObjectInfo { Value = true, Text = "Enable Finance" },
                            new ObjectInfo { Value = false, Text = "Enable Num Game" },
                            new ObjectInfo { Value = true, Text = "Virtual Sports" }
                        }
                    };
                }
                else if (roleId == this.Agent)
                {
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 1203, Text = "Total Balance" });
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 4521, Text = "Yesterday Balance" });
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = 444521, Text = "Member Outstanding" });

                    model.BalanceInfo.Statuses.Add(new Status { TotalActive = 3, TotalClosed = 2, TotalSuspended = 4, Text = "Members A/C/S", Tooltip = "Total Members Active/Closed/Suspended" });

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
                            new ObjectInfo { Value = true, Text = "Special" },
                            new ObjectInfo { Value = false, Text = "Closed" },
                            new ObjectInfo { Value = true, Text = "Special" },
                            new ObjectInfo { Value = false, Text = "Suspened" },
                            new ObjectInfo { Value = false, Text = "Allow Outright" },
                            new ObjectInfo { Value = false, Text = "Enable Casino" },
                            new ObjectInfo { Value = true, Text = "Enable Live Casino" },
                            new ObjectInfo { Value = true, Text = "Double Comm" },
                            new ObjectInfo { Value = true, Text = "Selected Betting" },
                            new ObjectInfo { Value = false, Text = "Internal" },
                            new ObjectInfo { Value = true, Text = "Enable Finance" },
                            new ObjectInfo { Value = false, Text = "Enable Num Game" },
                            new ObjectInfo { Value = true, Text = "Virtual Sports" }
                        }
                    };
                }
                else
                {
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
                            new ObjectInfo { Value = true, Text = "Special" },
                            new ObjectInfo { Value = false, Text = "Closed" },
                            new ObjectInfo { Value = true, Text = "Special" },
                            new ObjectInfo { Value = false, Text = "Suspened" },
                            new ObjectInfo { Value = false, Text = "Allow Outright" },
                            new ObjectInfo { Value = false, Text = "Enable Casino" },
                            new ObjectInfo { Value = true, Text = "Enable Live Casino" },
                            new ObjectInfo { Value = true, Text = "Double Comm" },
                            new ObjectInfo { Value = true, Text = "Selected Betting" },
                            new ObjectInfo { Value = false, Text = "Internal" },
                            new ObjectInfo { Value = true, Text = "Enable Finance" },
                            new ObjectInfo { Value = false, Text = "Enable Num Game" },
                            new ObjectInfo { Value = true, Text = "Virtual Sports" }
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
            throw new NotImplementedException();
        }

        public override DataRowCollection GetOnlineList(int custid)
        {
            throw new NotImplementedException();
        }
    }
}