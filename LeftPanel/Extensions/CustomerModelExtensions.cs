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
    using System.Configuration;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.CustomerSetting.IP.Utils;
    using Fanex.CustomerSetting.Module.KenoStatus.Configurations;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.Repositories.Impl;
    using Fanex.ExternalAccount.Module.LeftPanel.Services.Impl;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.Report;
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
        /// Checks authorize on function of an admin.
        /// </summary>
        /// <param name="userId">Admin user id.</param>
        /// <param name="functionKey">Function key.</param>
        /// <param name="policy">Policy permission.</param>
        /// <returns>Boolean object.</returns>
        public static bool IsAuthorized(int userId, string functionKey, AuthenticateAction.AuthenticationType policy)
        {
            var userCache = new CachingService().GetUserCache(userId);
            var menuList = userCache.MenuList;
            var foundItems = userCache.PolicyList.Where(x => x.FuncKey == functionKey);
            var hasPerm = true;

            hasPerm = menuList.Where(o => !string.IsNullOrWhiteSpace(o.Url)).Count() > 0;

            if (!hasPerm && (foundItems == null || foundItems.Count() == 0))
            {
                return false;
            }

            if (foundItems.Count() > 0)
            {
                var firstItem = foundItems.First();
                switch (policy)
                {
                    case AuthenticateAction.AuthenticationType.Read:
                        if (firstItem.Policy == EPolicy.Read || firstItem.Policy == EPolicy.Update || firstItem.Policy == EPolicy.Delete ||
                            firstItem.Policy == EPolicy.Create)
                        {
                            return true;
                        }

                        break;
                    case AuthenticateAction.AuthenticationType.Host:
                        if (firstItem.Policy == EPolicy.Read)
                        {
                            return false;
                        }

                        return true;
                }
            }

            return false;
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
                    IP = new ObjectInfo { Text = CheckVNIP(row.ToString("ip")) },
                    LoginTime = new ObjectInfo { Text = Utils.ToDateTime(row["reload_time"]).ToStringExt("MM/dd/yyyy HH:mm:ss tt") }
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
                    new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["createdate"]).ToStringExt("M/d/yyyy H:mm tt"), Text = "Created Date" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["lastlogged"]).ToStringExt("M/d/yyyy H:mm tt"), Text = "Last Login" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCash.Rows[0]["lastTxn"]).ToStringExt("M/d/yyyy H:mm tt"), Text = "Last Txn", Tooltip = "Last Transaction" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["pwdexpiry"]).ToStringExt("M/d/yyyy H:mm tt"), Text = "Password" },
                    new ObjectInfo { Value = CheckVNIP(tableCustomer.Rows[0].ToString("lastloginip")), Text = "Last Login IP" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCustomer.Rows[0]["LastChangedSecurityCodeDate"]).ToStringExt("M/d/yyyy H:mm tt"), Text = "Security Code" },
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
            var enabled = tableCustomer.Rows[0].ToBoolean("enabled");
            var closed = tableCustomer.Rows[0].ToBoolean("closed");
            var suspended = tableCustomer.Rows[0].ToBoolean("suspended");
            var status = "This account was ";
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
                CustList = new ObjectInfo { Key = "CustList", Value = roleId == 4 ? "Master List" : roleId == 3 ? "Agent List" : "Member List" },
                Status = new ObjectInfo { Tooltip = !enabled ? status + "disabled" : closed ? status + "closed" : suspended ? status + "suspended" : string.Empty },
                BackgroundColor = new ObjectInfo { Value = !enabled ? "disabled" : closed ? "closed" : suspended ? "suspended" : string.Empty },
                IsInternal = new ObjectInfo { Value = Utils.ToBoolean(tableCustomer.Rows[0]["isinternal"]) }
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
            model.BalanceInfo.IsUpdateCredit = IsAuthorized(CurrentSession.Instance.UserId, eFunctions.UpdateCredit, AuthenticateAction.AuthenticationType.Host);

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
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("superID"), Value = Utils.ToString(tableLine.Rows[0]["superName"]), Text = "Super" },
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("masterID"), Value = Utils.ToString(tableLine.Rows[0]["masterName"]), Text = "Master" },
                        new ObjectInfo { Key = tableLine.Rows[0].ToInt32("agentID"), Value = Utils.ToString(tableLine.Rows[0]["agentName"]), Text = "Agent" }
                    };
            model.MemberSettings(ds);

            return model;
        }

        /// <summary>
        /// Checks product statues.
        /// </summary>
        /// <param name="model">Customer model item.</param>
        /// <param name="groupName">Group name.</param>
        /// <param name="username">Username of customer.</param>
        /// <returns>CustomerModel object.</returns>
        public static CustomerModel GetProductStatuses(this CustomerModel model, string groupName, string username)
        {
            ProductStatuses product = model.Get3rdPartyProducts(groupName, username, model.PersonalInfo.UplineId == null ? 0 : (int)model.PersonalInfo.UplineId.Value);
            model.RemoveEnableProducts(product);

            if (product.RoleId == 4)
            {
                model.CheckConfig();

                return model;
            }

            model.UplineStatuses(product);
            model.RemoveProducts(product);
            model.GetSuperMasterPtsOnMember(product);

            return model;
        }

        /// <summary>
        /// Remove config to show or hide.
        /// </summary>
        /// <param name="model">Customer model.</param>
        /// <returns>Customer model item.</returns>
        public static CustomerModel CheckConfig(this CustomerModel model)
        {
            var hideVS = !Utils.ToBoolean(ConfigurationManager.AppSettings["EnableVirtualSports"]);
            var hideKeno = !KenoSection.Instance.ShowSetting;
            var hideGoldDeluxe = ConfigurationManager.AppSettings["IsShowGoldDeluxeSettingForCredit"].Equals("0");

            if (hideVS)
            {
                model.RemoveProduct("VirtualSports");
            }

            if (hideKeno)
            {
                model.RemoveProduct("Keno");
            }

            if (hideGoldDeluxe)
            {
                model.RemoveProduct("GoldDeluxe");
            }

            return model;
        }

        public static string CheckVNIP(string ip)
        {
            return string.IsNullOrWhiteSpace(ip) || (
                IpUtils.IsVnIp(ip) && !IsAuthorized(CurrentSession.Instance.UserId, eFunctions.ViewVNIP, AuthenticateAction.AuthenticationType.Read)) ? string.Empty : ip;
        }

        /// <summary>
        /// Removes Enable products.
        /// </summary>
        /// <param name="model">CustomerModel item.</param>
        /// <param name="product">ProductStatuses item.</param>
        /// <returns>CustomerModel object.</returns>
        private static CustomerModel RemoveEnableProducts(this CustomerModel model, ProductStatuses product)
        {
            if (!product.SyncBingo)
            {
                model.RemoveEnableProduct("EnableBingo");
            }

            if (!product.SyncCasino)
            {
                model.RemoveEnableProduct("EnableCasino");
            }

            if (!product.SyncLiveCasino)
            {
                model.RemoveEnableProduct("EnableLiveCasino");
            }

            if (product.HideVirtualSports)
            {
                model.RemoveEnableProduct("EnableVirtualSports");
            }

            if (product.HideEsports)
            {
                model.RemoveEnableProduct("EnableESports");
            }

            if (product.HideGoldDeluxe)
            {
                model.RemoveEnableProduct("EnableGoldDeluxe");
                model.RemoveProduct("GoldDeluxe");
            }

            if (product.HideColossusBets)
            {
                model.RemoveEnableProduct("EnableColossusBets");
                model.RemoveProduct("ColossusBets");
            }

            return model;
        }

        /// <summary>
        /// Removes Detail products.
        /// </summary>
        /// <param name="model">CustomerModel item.</param>
        /// <param name="product">ProductStatuses item.</param>
        /// <returns>CustomerModel object.</returns>
        private static CustomerModel RemoveProducts(this CustomerModel model, ProductStatuses product)
        {
            if (product.RoleId != 1 && product.DisableTransfer)
            {
                model.RemoveProduct("Transfer");
            }

            if (!product.UplineStatuses.SyncBingo)
            {
                model.RemoveProduct("Bingo");
            }

            if (!product.UplineStatuses.SyncCasino)
            {
                model.RemoveProduct("Casino");
            }

            if (!product.UplineStatuses.SyncLiveCasino)
            {
                model.RemoveProduct("LiveCasino");
            }

            if (product.UplineStatuses.HideVirtualSports)
            {
                model.RemoveProduct("VirtualSports");
            }

            if (product.HideKeno)
            {
                model.RemoveProduct("Keno");
            }

            if (product.HideGoldDeluxe)
            {
                model.RemoveProduct("GoldDeluxe");
            }

            return model;
        }

        /// <summary>
        /// Gets PT products on member level.
        /// </summary>
        /// <param name="model">CustomerModel item.</param>
        /// <param name="product">ProductStatuses item.</param>
        /// <returns>CustomerModel object.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private static CustomerModel GetSuperMasterPtsOnMember(this CustomerModel model, ProductStatuses product)
        {
            bool syncCasino = product.SyncCasino, syncBingo = product.SyncBingo, syncLiveCasino = product.SyncLiveCasino, hideVS = product.HideVirtualSports;
            bool hideKeno = product.HideKeno;
            bool hideGoldDeluxe = product.HideGoldDeluxe;

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
                       (o.Key.ToString() == "GoldDeluxe" && !hideGoldDeluxe)).ToList();
            model.Setting.MasterPts.Add(new ObjectInfo { Key = "SBPT", Value = "Sportsbook's PT" });
            model.Setting.MasterPts.Where(o => o.Key.ToString() == "PT").FirstOrDefault().Value = "Sportsbook";
            model.Setting.MasterPts.Where(o => o.Key.ToString() == "Racing").FirstOrDefault().Value = "Racing";
            var casino = model.Setting.MasterPts.Where(o => o.Key.ToString() == "Casino");
            var bingo = model.Setting.MasterPts.Where(o => o.Key.ToString() == "Bingo");
            var liveCasino = model.Setting.MasterPts.Where(o => o.Key.ToString() == "LiveCasino");
            var virtualSports = model.Setting.MasterPts.Where(o => o.Key.ToString() == "VirtualSports");
            var keno = model.Setting.MasterPts.Where(o => o.Key.ToString() == "Keno");
            var goldDeluxe = model.Setting.MasterPts.Where(o => o.Key.ToString() == "GoldDeluxe");

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

            model.Setting.SuperPts = model.Setting.MasterPts;
            return model;
        }

        /// <summary>
        /// Gets upper statuses.
        /// </summary>
        /// <param name="model">CustomerModel item.</param>
        /// <param name="product">Product statuses.</param>
        /// <returns>CustomerModel object.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private static CustomerModel UplineStatuses(this CustomerModel model, ProductStatuses product)
        {
            if (!product.UplineStatuses.AllowDoubleComm)
            {
                model.DisableProduct("DoubleComm");
            }

            if (product.UplineStatuses.DisableNumberGame)
            {
                model.DisableProduct("EnableNumberGame");
            }

            if (!product.UplineStatuses.AllowOutright)
            {
                model.DisableProduct("Outright");
            }

            if (product.UplineStatuses.DisableVirtualSports)
            {
                model.DisableProduct("EnableVirtualSports");
            }

            if (product.UplineStatuses.DisableESports)
            {
                model.DisableProduct("EnableESports");
            }

            if (product.UplineStatuses.DisableHRFixedOdds)
            {
                model.DisableProduct("EnableHRFixedOdds");
            }

            if (product.UplineStatuses.DisableBingo)
            {
                model.DisableProduct("EnableBingo");
            }

            if (product.UplineStatuses.DisableCasino)
            {
                model.DisableProduct("EnableCasino");
            }

            if (product.UplineStatuses.DisableLiveCasino)
            {
                model.DisableProduct("EnableLiveCasino");
            }

            if (product.UplineStatuses.DisableGoldDeluxe)
            {
                model.DisableProduct("EnableGoldDeluxe");
            }

            if (product.UplineStatuses.DisableColossusBets)
            {
                model.DisableProduct("EnableColossusBets");
            }

            if (product.UplineStatuses.Closed)
            {
                model.DisableProduct("Closed");
            }

            if (product.UplineStatuses.Suspended)
            {
                model.DisableProduct("Suspended");
            }

            return model;
        }

        /// <summary>
        /// Removes disabled and unsynchronized products.
        /// </summary>
        /// <param name="model">CustomerModel item.</param>
        /// <param name="key">Key to specify item.</param>
        /// <returns>CustomerModel object.</returns>
        private static CustomerModel RemoveProduct(this CustomerModel model, string key)
        {
            int index = -1;
            for (int i = 0; i < model.Setting.Details.Count; i++)
            {
                if (model.Setting.Details.ElementAt(i).Key.ToString().Equals(key, System.StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                model.Setting.Details.RemoveAt(index);
            }

            return model;
        }

        /// <summary>
        /// Removes unsynchronized products on setting tab.
        /// </summary>
        /// <param name="model">CustomerModel item.</param>
        /// <param name="key">Key to specify item.</param>
        /// <returns>CustomerModel object.</returns>
        private static CustomerModel RemoveEnableProduct(this CustomerModel model, string key)
        {
            int index = -1;
            for (int i = 0; i < model.Setting.EnableItems.Count; i++)
            {
                if (model.Setting.EnableItems.ElementAt(i).Key.ToString().Equals(key, System.StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                model.Setting.EnableItems.RemoveAt(index);
            }

            return model;
        }

        /// <summary>
        /// Disables product that is not enabled at upper customer.
        /// </summary>
        /// <param name="model">CustomerModel item.</param>
        /// <param name="key">Key to specify product.</param>
        /// <returns>CustomerModel object.</returns>
        private static CustomerModel DisableProduct(this CustomerModel model, string key)
        {
            int index = -1;
            for (int i = 0; i < model.Setting.EnableItems.Count; i++)
            {
                if (model.Setting.EnableItems.ElementAt(i).Key.ToString().Equals(key, System.StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                model.Setting.EnableItems.ElementAt(index).Disabled = true;
                model.Setting.EnableItems.ElementAt(index).Tooltip = key.Equals("Closed", System.StringComparison.OrdinalIgnoreCase) ?
                    "Upline has been closed" : key.Equals("Suspended", System.StringComparison.OrdinalIgnoreCase) ? "Upline has been suspended" : "Upline has been disabled";
            }

            return model;
        }

        /// <summary>
        /// Gets third party product statuses.
        /// </summary>
        /// <param name="model">CustomerModel item.</param>
        /// <param name="groupName">Group name to get.</param>
        /// <param name="username">Username to search.</param>
        /// <param name="uplineId">Upper customer id.</param>
        /// <returns>ProductStatuses object.</returns>
        private static ProductStatuses Get3rdPartyProducts(this CustomerModel model, string groupName, string username, int uplineId = 0)
        {
            IReportService report = new ReportService();

            DataTable customerInfo = report.GetCustomerInfo(groupName, 0, username, true);
            DataTable upperInfo = uplineId != 0 ? report.GetCustomerInfo(groupName, uplineId, null, true) : null;
            DataTable result = null;
            int srecommend = 0, mrecommend = 0, recommend = 0, custId = 0, roleId = 0;

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
                    upperInfo = new SuperService().GetData(null, uplineId, null, null, null, groupName);
                }
                else if (roleId == 2)
                {
                    result = new AgentService().GetData(mrecommend, custId, null, null, null);
                    upperInfo = new MasterService().GetData(srecommend, uplineId, null, null, null);
                }
                else
                {
                    result = new MemberService().GetData(recommend, custId, null, null, null);
                    upperInfo = result = new AgentService().GetData(mrecommend, uplineId, null, null, null);
                }

                return model.GetProductStatusDetail(roleId, result, upperInfo);
            }

            return null;
        }

        /// <summary>
        /// Gets detail statuses.
        /// </summary>
        /// <param name="model">CustomerModel item.</param>
        /// <param name="roleId">Role id of customer to search.</param>
        /// <param name="result">Current customer info.</param>
        /// <param name="upperInfo">Upper customer info.</param>
        /// <returns>ProductStatuses object.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private static ProductStatuses GetProductStatusDetail(this CustomerModel model, int roleId, DataTable result, DataTable upperInfo)
        {
            ProductStatuses product = new ProductStatuses();
            product.UplineStatuses = new ProductStatuses();
            product.RoleId = roleId;
            if (result.Rows.Count == 0)
            {
                return product;
            }

            product.SyncBingo = result.Rows[0].ToBoolean("syncBingo");
            product.SyncCasino = result.Rows[0].ToBoolean("syncCasino");
            product.SyncLiveCasino = result.Rows[0].ToBoolean("syncLiveCasino");
            product.DisableTransfer = roleId == 4 ? false : result.Rows[0].ToBoolean("disableTransfer");
            product.AllowDoubleComm = result.Rows[0].ToBoolean("doubleCommLosing");
            product.DisableVirtualSports = result.Rows[0].ToBoolean("disableVirtualSports");
            product.AllowOutright = !result.Rows[0].ToBoolean("disableOutright");
            product.DisableNumberGame = result.Rows[0].ToBoolean("disableNumber");
            product.DisableBingo = result.Rows[0].ToBoolean("disableBingo");
            product.DisableCasino = result.Rows[0].ToBoolean("disableCasino");
            product.DisableLiveCasino = result.Rows[0].ToBoolean("disableLiveCasino");
            product.Closed = result.Rows[0].ToBoolean("closed");
            product.Suspended = result.Rows[0].ToBoolean("suspended");
            product.HideVirtualSports = result.Rows[0].ToBoolean("hideVirtualSports");
            product.DisableHRFixedOdds = result.Rows[0].ToBoolean("disableHRFixedOdds");
            product.HideKeno = result.Rows[0].ToBoolean("HideKeno") || !KenoSection.Instance.ShowSetting;
            product.DisableKeno = result.Rows[0].ToBoolean("DisableKeno");
            product.DisableESports = result.Rows[0].ToBoolean("DisableESports");
            product.HideGoldDeluxe = result.Rows[0].ToBoolean("HideGoldDeluxe") || ConfigurationManager.AppSettings["IsShowGoldDeluxeSettingForCredit"] == "0";

            product.HideColossusBets = ConfigurationManager.AppSettings["IsShowColossusBets"] == "0" || result.Rows[0].ToBoolean("HideColossusBet");
            model.Setting.MinMaxPts.Add(new ObjectInfo { Key = "MinSB", Value = "Sportsbook" });
            model.Setting.MinMaxPts.Add(new ObjectInfo { Key = "MinRB", Value = "Racing" });
            model.Setting.SuperPts = model.Setting.MasterPts = model.Setting.Details.Where(o => o.Key.ToString() == "PT" || o.Key.ToString() == "Casino" ||
                o.Key.ToString() == "Bingo" || o.Key.ToString() == "LiveCasino" || o.Key.ToString() == "VirtualSports" || o.Key.ToString() == "Racing").ToList();
            if (roleId < 4 && upperInfo.Rows.Count > 0)
            {
                product.UplineStatuses.RoleId = roleId + 1;
                product.UplineStatuses.SyncBingo = upperInfo.Rows[0].ToBoolean("syncBingo");
                product.UplineStatuses.SyncCasino = upperInfo.Rows[0].ToBoolean("syncCasino");
                product.UplineStatuses.SyncLiveCasino = upperInfo.Rows[0].ToBoolean("syncLiveCasino");
                product.UplineStatuses.DisableTransfer = roleId + 1 == 4 ? false : upperInfo.Rows[0].ToBoolean("disableTransfer");
                product.UplineStatuses.AllowDoubleComm = upperInfo.Rows[0].ToBoolean("doubleCommLosing");
                product.UplineStatuses.DisableVirtualSports = upperInfo.Rows[0].ToBoolean("disableVirtualSports");
                product.UplineStatuses.AllowOutright = !upperInfo.Rows[0].ToBoolean("disableOutright");
                product.UplineStatuses.DisableNumberGame = upperInfo.Rows[0].ToBoolean("disableNumber");
                product.UplineStatuses.DisableBingo = upperInfo.Rows[0].ToBoolean("disableBingo");
                product.UplineStatuses.DisableCasino = upperInfo.Rows[0].ToBoolean("disableCasino");
                product.UplineStatuses.DisableLiveCasino = upperInfo.Rows[0].ToBoolean("disableLiveCasino");
                product.UplineStatuses.DisableGoldDeluxe = upperInfo.Rows[0].ToBoolean("DisableGoldDeluxe");

                product.UplineStatuses.DisableColossusBets = upperInfo.Rows[0].ToBoolean("DisableColossusBet");
                product.UplineStatuses.Closed = upperInfo.Rows[0].ToBoolean("closed");
                product.UplineStatuses.Suspended = upperInfo.Rows[0].ToBoolean("suspended");
                product.UplineStatuses.HideVirtualSports = upperInfo.Rows[0].ToBoolean("hideVirtualSports");
                product.UplineStatuses.DisableHRFixedOdds = upperInfo.Rows[0].ToBoolean("disableHRFixedOdds");
                product.UplineStatuses.DisableESports = upperInfo.Rows[0].ToBoolean("DisableESports");
            }

            return product;
        }

        /// <summary>
        /// Customer settings.
        /// </summary>
        /// <param name="model">CustomerModel object extension.</param>
        /// <param name="tableCustomer">Customer data.</param>
        /// <returns>CustomerModel object.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private static CustomerModel SuperSettings(this CustomerModel model, DataTable tableCustomer)
        {
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
                                Key = "EnableNumberGame", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableNumber"]), Text = "Enable Num Game", Tooltip = "Enable Number Game"
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
                                Tooltip ="Enable Colossus Bets"
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
                            new ObjectInfo { Key = "EnableKeno", Text = "Enable Keno" }
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
                            new ObjectInfo { Key = "Keno", Value = "Edit Keno" },
                            new ObjectInfo { Key = "GoldDeluxe", Value = "Edit Gold Deluxe" }
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
            var userId = CurrentSession.Instance.UserId;
            var showBetSetting = IsAuthorized(userId, eFunctions.BingoBetting, AuthenticateAction.AuthenticationType.Read) ||
                IsAuthorized(userId, eFunctions.CasinoBetting, AuthenticateAction.AuthenticationType.Read) ||
                IsAuthorized(userId, eFunctions.GoldDeluxeBetting, AuthenticateAction.AuthenticationType.Read) ||
                IsAuthorized(userId, eFunctions.KenoBetting, AuthenticateAction.AuthenticationType.Read) ||
                IsAuthorized(userId, eFunctions.LiveCasinoBetting, AuthenticateAction.AuthenticationType.Read) ||
                IsAuthorized(userId, eFunctions.RacingBetting, AuthenticateAction.AuthenticationType.Read) ||
                IsAuthorized(userId, eFunctions.SportsbookBetting, AuthenticateAction.AuthenticationType.Read) ||
                IsAuthorized(userId, eFunctions.VirtualSportsBetting, AuthenticateAction.AuthenticationType.Read) ? true : false;
                        
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

        /// <summary>
        /// Customer settings.
        /// </summary>
        /// <param name="model">CustomerModel object extension.</param>
        /// <param name="tableCustomer">Customer data.</param>
        /// <param name="tableCash">Cash info table.</param>
        /// <returns>CustomerModel object.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
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
                            new ObjectInfo { Key = "EnableCasino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableCasino"]), Text = "Enable Casino" },
                            new ObjectInfo { Key = "EnableBingo", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableBingo"]), Text = "Enable Bingo" },
                            new ObjectInfo
                            {
                                Key = "EnableNumberGame", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableNumber"]), Text = "Enable Num Game", Tooltip = "Enable Number Game"
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
                                Tooltip ="Enable Colossus Bets"
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
                            new ObjectInfo { Key = "EnableKeno", Text = "Enable Keno" }
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
                            new ObjectInfo { Key = "Keno", Value = "Edit Keno" },
                            new ObjectInfo { Key = "GoldDeluxe", Value = "Edit Gold Deluxe" }
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
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
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
                            new ObjectInfo { Key = "EnableCasino", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableCasino"]), Text = "Enable Casino" },
                            new ObjectInfo { Key = "EnableBingo", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableBingo"]), Text = "Enable Bingo" },
                            new ObjectInfo
                            {
                                Key = "EnableNumberGame", Value = !Utils.ToBoolean(tableCustomer.Rows[0]["disableNumber"]), Text = "Enable Num Game", Tooltip = "Enable Number Game"
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
                                Tooltip ="Enable Colossus Bets"
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
                            new ObjectInfo { Key = "EnableKeno", Text = "Enable Keno" }
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
                            new ObjectInfo { Key = "Keno", Value = "Edit Keno" },
                            new ObjectInfo { Key = "GoldDeluxe", Value = "Edit Gold Deluxe" }
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
                                Tooltip ="Enable Colossus Bets"
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
                            new ObjectInfo { Key = "EnableKeno", Text = "Enable Keno" }
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
                            new ObjectInfo { Key = "GoldDeluxe", Value = "Edit Gold Deluxe" }
                        }
            };

            return model;
        }
    }
}