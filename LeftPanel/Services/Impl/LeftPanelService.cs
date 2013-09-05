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
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.Extensions;
    using Fanex.ExternalAccount.Module.LeftPanel.Repositories.Impl;
    using Fanex.ExternalAccount.Module.LeftPanel.Repositories.Interface;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeftPanelService : BaseService
    {
        private readonly ILeftPanelDao data = new LeftPanelDao();
        private int TotalPages = 1;

        public override CustomerModel GetCustomerInfo(string username, string groupName = "ACCDDD", int pageIndex = 1)
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
                    CurrencyId = new ObjectInfo { Value = Utils.ToInt32(tableCustomer.Rows[0]["exchangeid"]), Text = "Currency Id" },
                    Currency = new ObjectInfo { Value = Utils.ToString(tableCustomer.Rows[0]["currency"]), Text = "Currency" },
                    Balances = new List<ObjectInfo>()
                    {                        
                        new ObjectInfo { Value = Utils.ToDecimal(tableCash.Rows[0][this.GetCashColumn(roleId)]), Text = "Total Cash" },
                        new ObjectInfo { Value = Utils.ToDecimal(tableCash.Rows[0][this.GetYesterdayCashColumn(roleId)]), Text = "Yesterday Cash" },
                        new ObjectInfo { Value = Utils.ToDecimal(tableCash.Rows[0][this.GetOutstandingColumn(roleId)]), Text = "Outstanding" },
                        new ObjectInfo { Value = Utils.ToDecimal(tableCustomer.Rows[0]["credit"]), Text = "Given Credit" },
                        //new ObjectInfo { Value = Utils.ToDecimal(tableCustomer.Rows[0]["credit"] - Utils.ToDecimal(tableCash.Rows[0][this.GetOutstandingColumn(roleId)], Text = "Bet Credit" }
                    }
                };

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

                var onlines = this.GetOnlineList(Utils.ToInt32(model.PersonalInfo.CustId.Value));

                foreach (DataRow row in onlines)
                {
                    model.OnlineList.Onlines.Add(new OnlineInfo
                    {                        
                        IP = new ObjectInfo { Text = row["ip"].ToString() },
                        LoginTime = new ObjectInfo { Text = Utils.ToDateTime(row["reload_time"]).ToString("MM/dd/yyyy HH:mm:ss tt") }
                    });
                }

                if (roleId != this.Member)
                {
                    model.Login.Add(new ObjectInfo { Value = Utils.ToBoolean(tableCustomer.Rows[0]["NicknameLoginOnly"]) ? "Nickname Only" : "Username/Nickname", Text = "Login Options" });
                    model.BalanceInfo.Balances.Add(new ObjectInfo { Value = tableCash.Rows[0][this.GetUsedCreditColumn(roleId)], Text = "Used Credit" });
                }

                model.DownlineList = new DownlineList();

                var downlines = this.GetDownlineList(Utils.ToInt32(model.PersonalInfo.CustId.Value), 100, pageIndex);

                if (downlines != null && downlines.Rows.Count > 0)
                {
                    foreach (DataRow item in downlines.Rows)
                    {
                        model.DownlineList.Downlines.Add(new ObjectInfo { Text = item["username"].ToString() });
                    }

                    model.DownlineList.CurrentPage = pageIndex;

                    if (this.TotalPages > 1)
                    {
                        for (int i = 1; i <= this.TotalPages; i++)
                        {
                            model.DownlineList.Pages.Add(new ObjectInfo { Value = i, Text = i.ToString() });
                        }
                    }
                    else
                    {
                        model.DownlineList.Pages.Add(new ObjectInfo { Value = 1, Text = "1" });
                    }
                }

                if (roleId == this.Super)
                {
                    model.SuperInfo(ds);
                }
                else if (roleId == this.Master)
                {
                    model.MasterInfo(ds);
                }
                else if (roleId == this.Agent)
                {
                    model.AgentInfo(ds);
                }
                else
                {
                    model.MemberInfo(ds);
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
                this.TotalPages = Utils.ToInt32(dr.Outputs["totalpages"]);

                return dr.ResultSet.Tables[0];
            }

            return null;
        }

        public override DataRowCollection GetOnlineList(int custid)
        {
            return this.data.GetOnlineList(custid);
        }
    }
}