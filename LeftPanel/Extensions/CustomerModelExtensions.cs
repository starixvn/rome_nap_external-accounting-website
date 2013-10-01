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
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Module.LeftPanel.Repositories.Impl;
    using Fanex.ExternalAccount.Module.LeftPanel.Services.Impl;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CustomerModelExtensions
    {
        /// <summary>
        /// Gets outstanding column.
        /// </summary>
        /// <param name="roleId">Role id of customer.</param>
        /// <returns>String object.</returns>
        public static string GetOutstandingColumn(int roleId)
        {
            var levelName = string.Empty;

            switch (roleId)
            {
                case 4:
                    levelName = "super";
                    break;

                case 3:
                    levelName = "master";
                    break;

                case 2:
                    levelName = "agent";
                    break;

                default:
                    break;
            }

            return roleId != 1 ? levelName + "PositionTaking" : "outstanding";
        }

        /// <summary>
        /// Gets used column of customer.
        /// </summary>
        /// <param name="roleId">Customer role id.</param>
        /// <returns>String object.</returns>
        public static string GetUsedCreditColumn(int roleId)
        {
            if (roleId == 4)
            {
                return "superUsedCredit";
            }
            else if (roleId == 3)
            {
                return "masterUsedCredit";
            }
            else if (roleId == 2)
            {
                return "agentUsedCredit";
            }
            else
            {
                return "superUsedCredit";
            }
        }

        /// <summary>
        /// Gets yester day cash column.
        /// </summary>
        /// <param name="roleId">Role id of customer.</param>
        /// <returns>String object.</returns>
        public static string GetYesterdayCashColumn(int roleId)
        {
            if (roleId == 4)
            {
                return "yesterday_superCash";
            }
            else if (roleId == 3)
            {
                return "yesterday_masterCash";
            }
            else if (roleId == 2)
            {
                return "yesterday_agentCash";
            }
            else
            {
                return "yesterday_cash";
            }
        }

        /// <summary>
        /// Gets yester cash column of customer.
        /// </summary>
        /// <returns>String object.</returns>
        public static string GetYesterdayCustomerCash()
        {
            return "yesterday_customerCash";
        }

        /// <summary>
        /// Gets cash column.
        /// </summary>
        /// <param name="roleId">Role id of customer.</param>
        /// <returns>String object.</returns>
        public static string GetCashColumn(int roleId)
        {
            if (roleId == 4)
            {
                return "superCash";
            }
            else if (roleId == 3)
            {
                return "masterCash";
            }
            else if (roleId == 2)
            {
                return "agentCash";
            }
            else
            {
                return "cash";
            }
        }

        /// <summary>
        /// Gets customer cash column.
        /// </summary>
        /// <returns>String object.</returns>
        public static string GetCustomerCash()
        {
            return "customerCash";
        }

        /// <summary>
        /// Gets down-line list.
        /// </summary>
        /// <param name="model">CustomerModel object data.</param>
        /// <param name="ds">Customer data.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <returns>CustomerModel object.</returns>
        public static CustomerModel GetDownlineList(this CustomerModel model, DataSet ds, int pageIndex)
        {
            var totalPages = 0;
            var downlineList = new LeftPanelDao().GetDownlineList(Utils.ToInt32(model.PersonalInfo.CustId.Value), 100, pageIndex);
            var downlines = downlineList.ResultSet.Tables[0];
            
            LeftPanelService.TotalPages = totalPages = Utils.ToInt32(downlineList.Outputs["totalpages"]);
            model.DownlineList = new DownlineList();

            if (downlines != null && downlines.Rows.Count > 0)
            {
                foreach (DataRow item in downlines.Rows)
                {
                    model.DownlineList.Downlines.Add(new ObjectInfo { Text = item["username"].ToString() });
                }

                model.DownlineList.CurrentPage = pageIndex;

                if (totalPages > 1)
                {
                    for (int i = 1; i <= totalPages; i++)
                    {
                        model.DownlineList.Pages.Add(new ObjectInfo { Value = i, Text = i.ToString() });
                    }
                }
                else
                {
                    model.DownlineList.Pages.Add(new ObjectInfo { Value = 1, Text = "1" });
                }
            }

            return model;
        }

        /// <summary>
        /// Gets online list.
        /// </summary>
        /// <param name="model">CustomerModel object data.</param>
        /// <param name="ds">Customer data.</param>
        /// <returns>CustomerModel object.</returns>
        public static CustomerModel GetOnlineList(this CustomerModel model, DataSet ds)
        {
            var onlines = new LeftPanelDao().GetOnlineList(Utils.ToInt32(model.PersonalInfo.CustId.Value));

            foreach (DataRow row in onlines)
            {
                model.OnlineList.Onlines.Add(new OnlineInfo
                {
                    IP = new ObjectInfo { Text = row["ip"].ToString() },
                    LoginTime = new ObjectInfo { Text = Utils.ToDateTime(row["reload_time"]).ToString("MM/dd/yyyy HH:mm:ss tt") }
                });
            }

            return model;
        }

        /// <summary>
        /// Gets login info.
        /// </summary>
        /// <param name="model">CustomerModel object data.</param>
        /// <param name="ds">Customer data.</param>
        /// <returns>CustomerModel object.</returns>
        public static CustomerModel LoginInfo(this CustomerModel model, DataSet ds)
        {
            var tableCustomer = ds.Tables[0];
            var tableCash = ds.Tables[2];
            var roleId = Utils.ToInt32(tableCustomer.Rows[0]["roleid"]);
            model.Login = new List<ObjectInfo>
                {
                    new ObjectInfo { Value = Utils.ToString(tableCustomer.Rows[0]["site"]), Text = "Site" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["createdate"]).ToString("M/d/yyyy H:mm tt"), Text = "Created Date" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["lastlogged"]).ToString("M/d/yyyy H:mm tt"), Text = "Last Login" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCash.Rows[0]["lastTxn"]).ToString("M/d/yyyy H:mm tt"), Text = "Last Txn", Tooltip = "Last Transaction" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["pwdexpiry"]).ToString("M/d/yyyy H:mm tt"), Text = "Password" },
                    new ObjectInfo { Value = Utils.ToString(tableCustomer.Rows[0]["lastloginip"]), Text = "Last Login IP" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["LastChangedSecurityCodeDate"]).ToString("M/d/yyyy H:mm tt"), Text = "Security Code" },
                };

            if (roleId != 1)
            {
                model.Login.Add(
                    new ObjectInfo
                    {
                        Value = Utils.ToBoolean(tableCustomer.Rows[0]["NicknameLoginOnly"]) ? "Nickname Only" : "Username/Nickname",
                        Text = "Login Options"
                    });
                model.BalanceInfo.Balances.Add(new ObjectInfo { Value = tableCash.Rows[0][GetUsedCreditColumn(roleId)], Text = "Used Credit" });
            }

            return model;
        }

        /// <summary>
        /// Gets personal info.
        /// </summary>
        /// <param name="model">CustomerModel object data.</param>
        /// <param name="ds">Customer data.</param>
        /// <returns>CustomerModel object.</returns>
        public static CustomerModel PersonalInfo(this CustomerModel model, DataSet ds)
        {
            var tableCustomer = ds.Tables[0];
            var roleId = Utils.ToInt32(tableCustomer.Rows[0]["roleid"]);
            model.PersonalInfo = new PersonalInfo
            {
                CustId = new ObjectInfo { Value = Utils.ToInt32(tableCustomer.Rows[0]["custid"]), Text = "Cust Id" },
                Fax = new ObjectInfo { Value = Utils.ToString(tableCustomer.Rows[0]["fax"]), Text = "Fax" },
                Mobile = new ObjectInfo { Value = Utils.ToString(tableCustomer.Rows[0]["mobilephone"]), Text = "Mobile" },
                Name = new ObjectInfo { Value = Utils.ToString(tableCustomer.Rows[0]["firstname"]) + " " + Utils.ToString(tableCustomer.Rows[0]["lastname"]), Text = "Name" },
                Nickname = new ObjectInfo { Value = Utils.ToString(tableCustomer.Rows[0]["username2"]), Text = "Nickname" },
                Phone = new ObjectInfo { Value = Utils.ToString(tableCustomer.Rows[0]["phone"]), Text = "Phone" },
                RoleId = new ObjectInfo { Value = roleId, Text = "Role Id" },
                UplineRoleId = new ObjectInfo { Value = roleId + 1, Text = "Upline Role Id" },
            };

            return model;
        }

        /// <summary>
        /// Gets balance info.
        /// </summary>
        /// <param name="model">CustomerModel object data.</param>
        /// <param name="ds">Customer data.</param>
        /// <returns>CustomerModel object.</returns>
        public static CustomerModel BalanceInfo(this CustomerModel model, DataSet ds)
        {
            var tableCustomer = ds.Tables[0];            
            var tableCash = ds.Tables[2];
            var roleId = Utils.ToInt32(tableCustomer.Rows[0]["roleid"]);
            model.BalanceInfo = new BalanceInfo()
            {
                CurrencyId = new ObjectInfo { Value = Utils.ToInt32(tableCustomer.Rows[0]["exchangeid"]), Text = "Currency Id" },
                Currency = new ObjectInfo { Value = Utils.ToString(tableCustomer.Rows[0]["currency"]), Text = "Currency" },
                Balances = new List<ObjectInfo>()
                    {                        
                        new ObjectInfo { Value = Utils.ToDecimal(tableCash.Rows[0][GetCashColumn(roleId)]), Text = "Total Cash" },
                        new ObjectInfo { Value = Utils.ToDecimal(tableCash.Rows[0][GetYesterdayCashColumn(roleId)]), Text = "Yesterday Cash" },
                        new ObjectInfo { Value = Utils.ToDecimal(tableCash.Rows[0][GetOutstandingColumn(roleId)]), Text = "Outstanding" },
                        new ObjectInfo { Value = Utils.ToDecimal(tableCustomer.Rows[0]["credit"]), Text = "Given Credit" },
                    }
            };

            return model;
        }

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
            model.SuperSettings(tableCustomer);

            return model;
        }

        /// <summary>
        /// Gets customer model for super level.
        /// </summary>
        /// <param name="model">Model object.</param>
        /// <param name="ds">DataSet object that contains user information.</param>
        /// <returns>CustomerModel object.</returns>
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

            model.PersonalInfo.UplineId = new ObjectInfo { Value = Utils.ToInt32(tableLine.Rows[0]["superID"]), Text = "Super ID" };
            model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["masterName"]), Text = "Master" };
            model.PersonalInfo.Downlines = new List<ObjectInfo>
                    { 
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("superID"), Value = Utils.ToString(tableLine.Rows[0]["superName"]), Text = "Super" },
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("masterID"), Value = Utils.ToString(tableLine.Rows[0]["masterName"]), Text = "Master" } 
                    };
            model.MasterSettings(tableCustomer, tableCash);            

            return model;
        }

        /// <summary>
        /// Gets customer model for super level.
        /// </summary>
        /// <param name="model">Model object.</param>
        /// <param name="ds">DataSet object that contains user information.</param>
        /// <returns>CustomerModel object.</returns>
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

            model.PersonalInfo.UplineId = new ObjectInfo { Value = Utils.ToInt32(tableLine.Rows[0]["masterID"]), Text = "Master ID" };
            model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["agentName"]), Text = "Agent" };
            model.PersonalInfo.Downlines = new List<ObjectInfo>
                    { 
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("superID"), Value = Utils.ToString(tableLine.Rows[0]["superName"]), Text = "Super" },
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("masterID"), Value = Utils.ToString(tableLine.Rows[0]["masterName"]), Text = "Master" },
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("agentID"), Value = Utils.ToString(tableLine.Rows[0]["agentName"]), Text = "Agent" }
                    };
            model.AgentSettings(ds);            

            return model;
        }

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
                        new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["superName"]), Text = "Super" },
                        new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["masterName"]), Text = "Master" },
                        new ObjectInfo { Value = Utils.ToString(tableLine.Rows[0]["agentName"]), Text = "Agent" },
                    };
            model.MemberSettings(ds);

            return model;
        }

        /// <summary>
        /// Checks transfer condition.
        /// </summary>
        /// <param name="model">Customer model item.</param>
        /// <param name="groupName">Group name.</param>
        /// <param name="username">Username of customer.</param>
        /// <returns>CustomerModel object.</returns>
        public static CustomerModel TransferCondition(this CustomerModel model, string groupName, string username)
        {
            DataTable customerInfo = new ReportService().GetCustomerInfo(groupName, 0, username, true);
            DataTable result = null;
            int srecommend = 0;
            int mrecommend = 0;
            int recommend = 0;
            int custId = 0;
            int roleId = 0;

            if (customerInfo != null)
            {
                roleId = Utils.ToInt32(customerInfo.Rows[0]["roleid"]);
                srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);
                custId = Utils.ToInt32(customerInfo.Rows[0]["custid"]);

                if (roleId == 4)
                {
                    result = new SuperService().GetData(null, custId, null, null, null, groupName);
                }
                else if (roleId == 3)
                {
                    result = new MasterService().GetData(srecommend, custId, null, null, null);
                }
                else if (roleId == 2)
                {
                    result = new AgentService().GetData(mrecommend, custId, null, null, null);
                }
                else
                {
                    result = new MemberService().GetData(recommend, custId, null, null, null);
                }

                if (roleId != 4 && roleId != 1 && Utils.ToBoolean(result.Rows[0]["disableTransfer"]))
                {
                    model.Setting.Details.RemoveAt(3);
                }
            }

            return model;
        }

        /// <summary>
        /// Customer settings.
        /// </summary>
        /// <param name="model">CustomerModel object extension.</param>
        /// <param name="tableCustomer">Customer data.</param>
        /// <returns>CustomerModel object.</returns>
        private static CustomerModel SuperSettings(this CustomerModel model, DataTable tableCustomer)
        {
            model.Setting = new Setting
            {
                EnableItems = new List<ObjectInfo>
                        {                            
                            new ObjectInfo { Key = "Closed", Value = Utils.ToBoolean(tableCustomer.Rows[0]["closed"]), Text = "Closed" },                            
                            new ObjectInfo { Key = "Suspended", Value = Utils.ToBoolean(tableCustomer.Rows[0]["suspended"]), Text = "Suspened" },
                            new ObjectInfo { Key = "Outright", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableOutright"]), Text = "Allow Outright" },
                            new ObjectInfo { Key = "Casino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableCasino"]), Text = "Enable Casino" },
                            new ObjectInfo { Key = "Bingo", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableBingo"]), Text = "Enable Bingo" },
                            new ObjectInfo 
                            { 
                                Key = "NumberGame", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableNumber"]), Text = "Enable Num Game", Tooltip = "Enable Number Game" 
                            },
                            new ObjectInfo { Key = "LiveCasino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableLiveCasino"]), Text = "Enable Live Casino" },                            
                            new ObjectInfo { Key = "VirtualSports", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableVirtualSports"]), Text = "Virtual Sports" },
                            new ObjectInfo 
                            { 
                                Key = "DoubleComm", Value = Utils.ToBoolean(tableCustomer.Rows[0]["doubleCommLosing"]), Text = "Double Comm", Tooltip = "Enable Double Commission" 
                            }
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
                            new ObjectInfo { Key = "Bingo", Value = "Edit Bingo" },
                            new ObjectInfo { Key = "Poker", Value = "Edit Poker" },
                            new ObjectInfo { Key = "LiveCasino", Value = "Edit Live Casino" },
                            new ObjectInfo { Key = "VirtualSports", Value = "Edit Virtual Sports" },
                        }
            };
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
            IList<int> groups = GroupConfigurations.AllowBettingGroups;
            int groupId = new LeftPanelService().UserGroupId(CurrentSession.Instance.UserId);

            if (groups.Contains(groupId))
            {
                model.Setting.Details.Add(new ObjectInfo
                {
                    Key = "BetSetting",
                    Value = "Edit Betting Limit"
                });
            }

            return model;
        }

        /// <summary>
        /// Customer settings.
        /// </summary>
        /// <param name="model">CustomerModel object extension.</param>
        /// <param name="tableCustomer">Customer data.</param>
        /// <param name="tableCash">Cash info table.</param>
        /// <returns>CustomerModel object.</returns>
        private static CustomerModel MasterSettings(this CustomerModel model, DataTable tableCustomer, DataTable tableCash = null)
        {
            model.Setting = new Setting
            {
                EnableItems = new List<ObjectInfo>
                        {                            
                            new ObjectInfo { Key = "Closed", Value = Utils.ToBoolean(tableCustomer.Rows[0]["closed"]), Text = "Closed" },                            
                            new ObjectInfo { Key = "Suspended", Value = Utils.ToBoolean(tableCustomer.Rows[0]["suspended"]), Text = "Suspened" },
                            new ObjectInfo { Key = "AutoPT", Value = !Utils.ToBoolean(tableCash.Rows[0]["disableAutoMPT"]), Text = "Allow Auto PT" },
                            new ObjectInfo { Key = "Outright", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableOutright"]), Text = "Allow Outright" },
                            new ObjectInfo { Key = "Casino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableCasino"]), Text = "Enable Casino" },
                            new ObjectInfo { Key = "Bingo", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableBingo"]), Text = "Enable Bingo" },
                            new ObjectInfo 
                            { 
                                Key = "NumberGame", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableNumber"]), Text = "Enable Num Game", Tooltip = "Enable Number Game" 
                            },
                            new ObjectInfo { Key = "LiveCasino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableLiveCasino"]), Text = "Enable Live Casino" },                            
                            new ObjectInfo { Key = "VirtualSports", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableVirtualSports"]), Text = "Virtual Sports" },
                            new ObjectInfo 
                            { 
                                Key = "DoubleComm", Value = Utils.ToBoolean(tableCustomer.Rows[0]["doubleCommLosing"]), Text = "Double Comm", Tooltip = "Enable Double Commission" 
                            },                            
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
                            new ObjectInfo { Key = "Bingo", Value = "Edit Bingo" },
                            new ObjectInfo { Key = "Poker", Value = "Edit Poker" },
                            new ObjectInfo { Key = "LiveCasino", Value = "Edit Live Casino" },
                            new ObjectInfo { Key = "VirtualSports", Value = "Edit Virtual Sports" },
                        }
            };

            return model;
        }

        /// <summary>
        /// Customer settings.
        /// </summary>
        /// <param name="model">CustomerModel object extension.</param>
        /// <param name="ds">Customer data.</param>
        /// <returns>CustomerModel object.</returns>
        private static CustomerModel AgentSettings(this CustomerModel model, DataSet ds)
        {
            var tableCustomer = ds.Tables[0];
            model.Setting = new Setting
            {
                EnableItems = new List<ObjectInfo>
                        {                            
                            new ObjectInfo { Key = "Closed", Value = Utils.ToBoolean(tableCustomer.Rows[0]["closed"]), Text = "Closed" },                            
                            new ObjectInfo { Key = "Suspended", Value = Utils.ToBoolean(tableCustomer.Rows[0]["suspended"]), Text = "Suspened" },
                            new ObjectInfo { Key = "Outright", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableOutright"]), Text = "Allow Outright" },
                            new ObjectInfo { Key = "Casino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableCasino"]), Text = "Enable Casino" },
                            new ObjectInfo { Key = "Bingo", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableBingo"]), Text = "Enable Bingo" },
                            new ObjectInfo 
                            { 
                                Key = "NumberGame", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableNumber"]), Text = "Enable Num Game", Tooltip = "Enable Number Game" 
                            },
                            new ObjectInfo { Key = "LiveCasino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableLiveCasino"]), Text = "Enable Live Casino" },                            
                            new ObjectInfo { Key = "VirtualSports", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableVirtualSports"]), Text = "Virtual Sports" },
                            new ObjectInfo 
                            { 
                                Key = "DoubleComm", Value = Utils.ToBoolean(tableCustomer.Rows[0]["doubleCommLosing"]), Text = "Double Comm", Tooltip = "Enable Double Commission" 
                            },                            
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
                            new ObjectInfo { Key = "Bingo", Value = "Edit Bingo" },
                            new ObjectInfo { Key = "Poker", Value = "Edit Poker" },
                            new ObjectInfo { Key = "LiveCasino", Value = "Edit Live Casino" },
                            new ObjectInfo { Key = "VirtualSports", Value = "Edit Virtual Sports" },
                        }
            };

            return model;
        }

        /// <summary>
        /// Customer settings.
        /// </summary>
        /// <param name="model">CustomerModel object extension.</param>
        /// <param name="ds">Customer data.</param>
        /// <returns>CustomerModel object.</returns>
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
                            new ObjectInfo { Key = "Casino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableCasino"]), Text = "Enable Casino" },
                            new ObjectInfo { Key = "Bingo", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableBingo"]), Text = "Enable Bingo" },
                            new ObjectInfo 
                            { 
                                Key = "NumberGame", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableNumber"]), 
                                Text = "Enable Num Game", Tooltip = "Enable Number Game" 
                            },
                            new ObjectInfo { Key = "LiveCasino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableLiveCasino"]), Text = "Enable Live Casino" },                            
                            new ObjectInfo { Key = "VirtualSports", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableVirtualSports"]), Text = "Virtual Sports" },
                            new ObjectInfo 
                            { 
                                Key = "DoubleComm", Value = Utils.ToBoolean(tableCustomer.Rows[0]["doubleCommLosing"]), Text = "Double Comm", Tooltip = "Enable Double Commission" 
                            },                            
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
                            new ObjectInfo { Key = "Racing", Value = "Edit Racing" },
                            new ObjectInfo { Key = "Casino", Value = "Edit Casino" },
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
