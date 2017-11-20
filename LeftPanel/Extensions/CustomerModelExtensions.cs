// --------------------------------------------------------------------------
// <copyright file="CustomerModelExtensions.cs" company="">
// Copyright (c) . All rights reserved.
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
    using DTO.Customer;
    using Fanex.CustomerSetting.IP.Utils;
    using Fanex.CustomerSetting.Module.Keno.KenoSetting.Configurations;
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
    using RemotingRef;

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
            else
            {
                return "agentUsedCredit";
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

            hasPerm = menuList.Any(o => !string.IsNullOrWhiteSpace(o.Url));

            if (!hasPerm && (foundItems == null || foundItems.Any()))
            {
                return false;
            }

            if (foundItems.Any())
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
                    new ObjectInfo { Value = roleId != 1 ? Utils.ToString(tableCustomer.Rows[0]["site"]) : Utils.ToString(tableCustomer.Rows[0]["membersite"]), Text = "Site" },
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

                model.Login.Add(
                  new ObjectInfo
                  {
                      Value = Utils.ToDateTime(tableCustomer.Rows[0]["OTPCreateDate"]).ToStringExt("M/d/yyyy H:mm tt"),
                      Text = "OTP"
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
            var closedReason = string.Empty;

            if (closed)
            {
                var customerClosedReason = new CustomerClosedReason(tableCustomer.Rows[0]["closedreason"]);
                closedReason = customerClosedReason.ClassName;
                status = customerClosedReason.ClosedReason;
            }

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
                Status = new ObjectInfo { Tooltip = !enabled ? status + "disabled" : closed ? status : suspended ? status + "suspended" : string.Empty },
                BackgroundColor = new ObjectInfo { Value = !enabled ? "disabled" : closed ? closedReason : suspended ? "suspended" : string.Empty },
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
        /// Checks product statues.
        /// </summary>
        /// <param name="model">Customer model item.</param>
        /// <param name="groupName">Group name.</param>
        /// <param name="username">Username of customer.</param>
        /// <returns>CustomerModel object.</returns>
        public static CustomerModel GetProductStatuses(this CustomerModel model, int userId, string groupName, string username)
        {
            ProductStatuses product = model.Get3rdPartyProducts(userId, groupName, username, model.PersonalInfo.UplineId == null ? 0 : (int)model.PersonalInfo.UplineId.Value);
            model.GetSuperMasterPtsOnMember(product);
            model.RemoveEnableAndPtforProducts(product);

            if (product.RoleId == 4)
            {
                model.CheckConfig();

                return model;
            }

            model.DisableUplineStatuses(product);

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
            var hideColossusBets = ConfigurationManager.AppSettings["IsShowColossusBets"].Equals("0");

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

            if (hideColossusBets)
            {
                model.RemoveProduct("ColossusBets");
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
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private static CustomerModel RemoveEnableAndPtforProducts(this CustomerModel model, ProductStatuses product)
        {
            if (!product.SyncCasino)
            {
                model.RemoveEnableProduct("EnableCasino");

                if (!product.UplineStatuses.SyncCasino)
                {
                    model.RemoveProduct("Casino")
                        .RemoveCrossLevelProduct("Casino");
                }
            }

            if (!product.SyncLiveCasino)
            {
                model.RemoveEnableProduct("EnableLiveCasino");

                if (!product.UplineStatuses.SyncLiveCasino)
                {
                    model.RemoveProduct("LiveCasino")
                        .RemoveCrossLevelProduct("LiveCasino");
                }
            }

            if (product.HideVirtualSports)
            {
                model.RemoveEnableProduct("EnableVirtualSports");

                bool hideVirtualSports = product.RoleId == 1 ?
                    product.UplineStatuses.HideVirtualSports :
                    product.HideVirtualSports;

                if (hideVirtualSports)
                {
                    model.RemoveProduct("VirtualSports")
                        .RemoveCrossLevelProduct("VirtualSports");
                }
            }

            if (product.HideEsports)
            {
                model.RemoveEnableProduct("EnableESports")
                    .RemoveProduct("ESports")
                    .RemoveCrossLevelProduct("ESports");
            }

            if (product.HideGoldDeluxe)
            {
                model.RemoveEnableProduct("EnableGoldDeluxe");
            }

            if (product.UplineStatuses.HideGoldDeluxe)
            {
                model.RemoveProduct("GoldDeluxe")
                    .RemoveCrossLevelProduct("GoldDeluxe");
            }

            if (product.HideColossusBets)
            {
                model.RemoveEnableProduct("EnableColossusBets")
                    .RemoveProduct("ColossusBets");
            }

            if (product.HideSportsbook2)
            {
                model.RemoveProduct("Sportsbook2")
                    .RemoveCrossLevelProduct("Sportsbook2");
            }

            if (product.HideCockfighting)
            {
                model.RemoveEnableProduct("EnableCockfighting")
                    .RemoveProduct("Cockfighting")
                    .RemoveCrossLevelProduct("Cockfighting");
            }

            if (product.HideCricket)
            {
                model.RemoveProduct("Cricket")
                    .RemoveCrossLevelProduct("Cricket");
            }

            model.RemoveEnableAndEditProductDCS(
                product.RoleId,
                0,
                product.RoleId < 4 ? product.UplineStatuses.HideAllbet : product.HideAllbet,
                product.RoleId < 4 ? product.UplineStatuses.SyncAllbet : product.SyncAllbet,
                product.HideAllbet,
                product.SyncAllbet,
                product.SyncAllbetDownlines,
                "Allbet",
                false);

            model.RemoveEnableAndEditProductDCS(
                product.RoleId,
                1,
                product.RoleId < 4 ? product.UplineStatuses.HideVoidbridge : product.HideVoidbridge,
                product.RoleId < 4 ? product.UplineStatuses.SyncVoidbridge : product.SyncVoidbridge,
                product.HideVoidbridge,
                product.SyncVoidbridge,
                product.SyncVoidbridgeDownlines,
                "MacauGames",
                false);

            model.RemoveEnableAndEditProductDCS(
                product.RoleId,
                2,
                product.RoleId < 4 ? product.UplineStatuses.HideNG2 : product.HideNG2,
                product.RoleId < 4 ? product.UplineStatuses.SyncNG2 : product.SyncNG2,
                product.HideNG2,
                product.SyncNG2,
                true,
                "NG2");

            model.RemoveEnableAndEditProductDCS(
                product.RoleId,
                3,
                product.RoleId < 4 ? product.UplineStatuses.HideFishingWorld : product.HideFishingWorld,
                product.RoleId < 4 ? product.UplineStatuses.SyncFishingWorld : product.SyncFishingWorld,
                product.HideFishingWorld,
                product.SyncFishingWorld,
                product.SyncFishingWorldDownlines,
                "FishingWorld");

            model.RemoveEnableAndEditProductDCS(
                product.RoleId,
                4,
                product.RoleId < 4 ? product.UplineStatuses.HideLottery : product.HideLottery,
                product.RoleId < 4 ? product.UplineStatuses.SyncLottery : product.SyncLottery,
                product.HideLottery,
                product.SyncLottery,
                product.SyncLotteryDownlines,
                "Lottery");

            model.RemoveEnableAndEditProductDCS(
                product.RoleId,
                5,
                product.RoleId < 4 ? product.UplineStatuses.HideLucky3 : product.HideLucky3,
                product.RoleId < 4 ? product.UplineStatuses.SyncLucky3 : product.SyncLucky3,
                product.HideLucky3,
                product.SyncLucky3,
                product.SyncLucky3Downlines,
                "Lucky3");

            return model;
        }

        private static CustomerModel RemoveEnableAndEditProductDCS(
            this CustomerModel model,
            int roleId,
            int productIndex,
            bool hideUpline,
            bool syncUpline,
            bool hide,
            bool sync,
            bool syncDownline,
            string productName,
            bool isCheckSyncDownlineField = true)
        {
            var showProductIndex = productIndex == 0 ? productIndex : productIndex * 2;
            if (hide)
            {
                model.PersonalInfo.IsShowProducts.ElementAt(showProductIndex + 1).Value = false;
            }

            if (((roleId == 4 || roleId == 3 || roleId == 2) && hide == true)
                || (roleId == 1 && hideUpline == true))
            {
                model.RemoveEnableProduct("Enable" + productName);
                model.RemoveProduct(productName);

                model.PersonalInfo.IsShowProducts.ElementAt(showProductIndex).Value = false;

                if (roleId == 1)
                {
                    model.RemoveCrossLevelProduct(productName);
                }

                return model;
            }

            if (hideUpline || !syncUpline)
            {
                model.RemoveEnableProduct("Enable" + productName);
                if ((roleId != 4) ||
                    (roleId == 4 && hideUpline))
                {
                    model.RemoveProduct(productName);
                }

                model.PersonalInfo.IsShowProducts.ElementAt(showProductIndex).Value = false;

                if (roleId == 1)
                {
                    model.RemoveCrossLevelProduct(productName);
                }
            }
            else if ((!sync && roleId == 1) || (!hide && !sync) || (isCheckSyncDownlineField && !syncDownline))
            {
                // Show but not sync or not sync all downlines => don't show enable, show edit setting link only
                model.RemoveEnableProduct("Enable" + productName);
                model.PersonalInfo.IsShowProducts.ElementAt(showProductIndex).Value = false;
                if (roleId == 1)
                {
                    model.RemoveCrossLevelProduct(productName);
                }
            }

            return model;
        }

        /// <summary>
        /// Gets upper statuses.
        /// </summary>
        /// <param name="model">CustomerModel item.</param>
        /// <param name="product">Product statuses.</param>
        /// <returns>CustomerModel object.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private static CustomerModel DisableUplineStatuses(this CustomerModel model, ProductStatuses product)
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

            if (product.UplineStatuses.DisableAllbet)
            {
                model.DisableProduct("EnableAllbet");
            }

            if (product.UplineStatuses.DisableVoidbridge)
            {
                model.DisableProduct("EnableMacauGames");
            }

            if (product.UplineStatuses.DisableNG2)
            {
                model.DisableProduct("EnableNG2");
            }

            if (product.UplineStatuses.DisableFishingWorld)
            {
                model.DisableProduct("EnableFishingWorld");
            }

            if (product.UplineStatuses.DisableLottery)
            {
                model.DisableProduct("EnableLottery");
            }

            if (product.UplineStatuses.DisableLucky3)
            {
                model.DisableProduct("EnableLucky3");
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

        private static CustomerModel RemoveCrossLevelProduct(this CustomerModel model, string key)
        {
            int index = -1;
            for (int i = 0; i < model.Setting.SuperPts.Count; i++)
            {
                if (model.Setting.SuperPts.ElementAt(i).Key.ToString().Equals(key, System.StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                model.Setting.MasterPts.RemoveAt(index);
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
                    model.Setting.EnableItems.RemoveAt(index);
                    break;
                }
            }

            if (index == -1)
            {
                for (int i = 0; i < model.Setting.EditItems.Count; i++)
                {
                    if (model.Setting.EditItems.ElementAt(i).Key.ToString().Equals(key, System.StringComparison.OrdinalIgnoreCase))
                    {
                        index = i;
                        model.Setting.EditItems.RemoveAt(index);
                        break;
                    }
                }
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
        private static ProductStatuses Get3rdPartyProducts(this CustomerModel model, int userId, string groupName, string username, int uplineId = 0)
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
                    result = new SuperService().GetData(userId, null, custId, null, null, null, groupName);
                }
                else if (roleId == 3)
                {
                    result = new MasterService().GetData(userId, srecommend, custId, null, null, null);
                    upperInfo = new SuperService().GetData(userId, null, uplineId, null, null, null, groupName);
                }
                else if (roleId == 2)
                {
                    result = new AgentService().GetData(userId, mrecommend, custId, null, null, null);
                    upperInfo = new MasterService().GetData(userId, srecommend, uplineId, null, null, null);
                }
                else
                {
                    result = new MemberService().GetData(userId, recommend, custId, null, null, null);
                    upperInfo = new AgentService().GetData(userId, mrecommend, uplineId, null, null, null);
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

            var athenticationService = new ExternalAccountAuthenticate();

            product.SyncCasino = result.Rows[0].ToBoolean("syncCasino");
            product.SyncLiveCasino = result.Rows[0].ToBoolean("syncLiveCasino");
            product.DisableTransfer = roleId == 4 ? false : result.Rows[0].ToBoolean("disableTransfer");
            product.AllowDoubleComm = result.Rows[0].ToBoolean("doubleCommLosing");
            product.DisableVirtualSports = result.Rows[0].ToBoolean("disableVirtualSports");
            product.AllowOutright = !result.Rows[0].ToBoolean("disableOutright");
            product.DisableNumberGame = result.Rows[0].ToBoolean("disableNumber");
            product.DisableCasino = result.Rows[0].ToBoolean("disableCasino");
            product.DisableLiveCasino = result.Rows[0].ToBoolean("disableLiveCasino");
            product.Closed = result.Rows[0].ToBoolean("closed");
            product.Suspended = result.Rows[0].ToBoolean("suspended");
            product.HideVirtualSports = result.Rows[0].ToBoolean("hideVirtualSports");
            product.DisableHRFixedOdds = result.Rows[0].ToBoolean("disableHRFixedOdds");
            product.HideKeno = result.Rows[0].ToBoolean("HideKeno") || !KenoSection.Instance.ShowSetting;
            product.DisableKeno = result.Rows[0].ToBoolean("DisableKeno");
            product.HideEsports = !athenticationService.AllowReadByAuthenticationParam(eFunctions.ShowEnableEsports) || Utils.ToBoolean(ConfigurationManager.AppSettings["HideEsports"]);
            product.DisableESports = result.Rows[0].ToBoolean("DisableESports");
            product.HideGoldDeluxe = result.Rows[0].ToBoolean("HideGoldDeluxe") || ConfigurationManager.AppSettings["IsShowGoldDeluxeSettingForCredit"] == "0";
            product.HideSportsbook2 = result.Rows[0].ToBoolean("HideMuayThai") || ConfigurationManager.AppSettings["IsShowSportsbook2"] == "0";
            product.HideCockfighting = result.Rows[0].ToBoolean("HideCockfighting") || ConfigurationManager.AppSettings["IsShowCockfighting"] == "0";
            product.HideCricket = result.Rows[0].ToBoolean("HideCricket") ||
                ConfigurationManager.AppSettings["IsShowCricket"] == "0" || !result.Rows[0].ToBoolean("IsIMBC");
            product.HideColossusBets = ConfigurationManager.AppSettings["IsShowColossusBets"] == "0" || result.Rows[0].ToBoolean("HideColossusBet");

            product.HideAllbet = ConfigurationManager.AppSettings["IsShowAllbet"] == "0" || result.Rows[0].ToBoolean("HideAllbet");
            product.SyncAllbet = result.Rows[0].ToBoolean("SyncedAllBet");
            product.SyncAllbetDownlines = result.Rows[0].ToBoolean("SyncedAllBetDownline");
            product.DisableAllbet = result.Rows[0].ToBoolean("DisableAllbet");

            product.HideVoidbridge = ConfigurationManager.AppSettings["IsShowVoidbridge"] == "0" || result.Rows[0].ToBoolean("HideVoidBridge");
            product.SyncVoidbridge = result.Rows[0].ToBoolean("SyncedVoidBridge");
            product.SyncVoidbridgeDownlines = result.Rows[0].ToBoolean("SyncedVoidBridgeDownline");
            product.DisableVoidbridge = result.Rows[0].ToBoolean("DisableVoidBridge");

            product.HideNG2 = ConfigurationManager.AppSettings["IsShowNG2"] == "0" || result.Rows[0].ToBoolean("HideNG2");
            product.SyncNG2 = result.Rows[0].ToBoolean("SyncedNG2");
            product.DisableNG2 = result.Rows[0].ToBoolean("DisableNG2");

            product.HideFishingWorld = ConfigurationManager.AppSettings["IsShowFishingWorld"] == "0" || result.Rows[0].ToBoolean("HideFishingWorld");
            product.SyncFishingWorld = result.Rows[0].ToBoolean("SyncedFishingWorld");
            product.SyncFishingWorldDownlines = result.Rows[0].ToBoolean("SyncedFishingWorldDownline");
            product.DisableFishingWorld = result.Rows[0].ToBoolean("DisableFishingWorld");

            product.SyncGoldDeluxeDownlines = result.Rows[0].ToBoolean("SyncedGoldDeluxeDownline");

            product.HideLottery = ConfigurationManager.AppSettings["IsShowLottery"] == "0" || result.Rows[0].ToBoolean("HideLottery");
            product.SyncLottery = true;
            product.SyncLotteryDownlines = true;
            product.DisableLottery = result.Rows[0].ToBoolean("DisableLottery");

            product.HideLucky3 = ConfigurationManager.AppSettings["IsShowLucky3"] == "0" || result.Rows[0].ToBoolean("HideLucky3");
            product.SyncLucky3 = true;
            product.SyncLucky3Downlines = true;
            product.DisableLucky3 = result.Rows[0].ToBoolean("DisableLucky3");

            model.Setting.MinMaxPts.Add(new ObjectInfo { Key = "MinSB", Value = "Sportsbook" });
            model.Setting.MinMaxPts.Add(new ObjectInfo { Key = "MinRB", Value = "Racing" });
            model.Setting.SuperPts = model.Setting.MasterPts = model.Setting.Details.Where(o => o.Key.ToString() == "PT" || o.Key.ToString() == "Casino" ||
                o.Key.ToString() == "LiveCasino" || o.Key.ToString() == "VirtualSports" || o.Key.ToString() == "Racing").ToList();
            if (roleId < 4 && upperInfo.Rows.Count > 0)
            {
                product.UplineStatuses.RoleId = roleId + 1;
                product.UplineStatuses.SyncCasino = upperInfo.Rows[0].ToBoolean("syncCasino");
                product.UplineStatuses.SyncLiveCasino = upperInfo.Rows[0].ToBoolean("syncLiveCasino");
                product.UplineStatuses.DisableTransfer = roleId + 1 == 4 ? false : upperInfo.Rows[0].ToBoolean("disableTransfer");
                product.UplineStatuses.AllowDoubleComm = upperInfo.Rows[0].ToBoolean("doubleCommLosing");
                product.UplineStatuses.DisableVirtualSports = upperInfo.Rows[0].ToBoolean("disableVirtualSports");
                product.UplineStatuses.AllowOutright = !upperInfo.Rows[0].ToBoolean("disableOutright");
                product.UplineStatuses.DisableNumberGame = upperInfo.Rows[0].ToBoolean("disableNumber");
                product.UplineStatuses.DisableCasino = upperInfo.Rows[0].ToBoolean("disableCasino");
                product.UplineStatuses.DisableLiveCasino = upperInfo.Rows[0].ToBoolean("disableLiveCasino");
                product.UplineStatuses.DisableGoldDeluxe = upperInfo.Rows[0].ToBoolean("DisableGoldDeluxe");
                product.UplineStatuses.HideGoldDeluxe = upperInfo.Rows[0].ToBoolean("HideGoldDeluxe") || ConfigurationManager.AppSettings["IsShowGoldDeluxeSettingForCredit"] == "0";

                product.UplineStatuses.DisableColossusBets = upperInfo.Rows[0].ToBoolean("DisableColossusBet");
                product.UplineStatuses.Closed = upperInfo.Rows[0].ToBoolean("closed");
                product.UplineStatuses.Suspended = upperInfo.Rows[0].ToBoolean("suspended");
                product.UplineStatuses.HideVirtualSports = upperInfo.Rows[0].ToBoolean("hideVirtualSports");
                product.UplineStatuses.DisableHRFixedOdds = upperInfo.Rows[0].ToBoolean("disableHRFixedOdds");
                product.UplineStatuses.DisableESports = upperInfo.Rows[0].ToBoolean("DisableESports");

                product.UplineStatuses.DisableAllbet = upperInfo.Rows[0].ToBoolean("DisableAllbet");
                product.UplineStatuses.HideAllbet = upperInfo.Rows[0].ToBoolean("HideAllbet");
                product.UplineStatuses.SyncAllbet = upperInfo.Rows[0].ToBoolean("SyncedAllBet");

                product.UplineStatuses.DisableVoidbridge = upperInfo.Rows[0].ToBoolean("DisableVoidBridge");
                product.UplineStatuses.HideVoidbridge = upperInfo.Rows[0].ToBoolean("HideVoidBridge");
                product.UplineStatuses.SyncVoidbridge = upperInfo.Rows[0].ToBoolean("SyncedVoidBridge");

                product.UplineStatuses.DisableNG2 = upperInfo.Rows[0].ToBoolean("DisableNG2");
                product.UplineStatuses.HideNG2 = upperInfo.Rows[0].ToBoolean("HideNG2");
                product.UplineStatuses.SyncNG2 = upperInfo.Rows[0].ToBoolean("SyncedNG2");

                product.UplineStatuses.DisableFishingWorld = upperInfo.Rows[0].ToBoolean("DisableFishingWorld");
                product.UplineStatuses.HideFishingWorld = upperInfo.Rows[0].ToBoolean("HideFishingWorld");
                product.UplineStatuses.SyncFishingWorld = upperInfo.Rows[0].ToBoolean("SyncedFishingWorld");

                product.UplineStatuses.DisableLottery = upperInfo.Rows[0].ToBoolean("DisableLottery");
                product.UplineStatuses.HideLottery = upperInfo.Rows[0].ToBoolean("HideLottery");
                product.UplineStatuses.SyncLottery = true;

                product.UplineStatuses.DisableLucky3 = upperInfo.Rows[0].ToBoolean("DisableLucky3");
                product.UplineStatuses.HideLucky3 = upperInfo.Rows[0].ToBoolean("HideLucky3");
                product.UplineStatuses.SyncLucky3 = true;
            }

            return product;
        }

        public static CustomerModel AddEnableAndDetailProduct(this CustomerModel model, DataTable table3rdDCS, int productId, string productName, string editBettingLimitFunction = "", bool checkboxType = true, bool getFromRemotingRef = false)
        {
            model.PersonalInfo.IsShowProducts.Add(new ObjectInfo { Key = "isShow" + productName.Replace(" ", string.Empty), Value = true });
            model.PersonalInfo.IsShowProducts.Add(new ObjectInfo { Key = "isShow" + productName.Replace(" ", string.Empty) + "Betting", Value = string.IsNullOrEmpty(editBettingLimitFunction) ? false : IsAuthorized(CurrentSession.Instance.UserId, editBettingLimitFunction, AuthenticateAction.AuthenticationType.Read) });
            if (table3rdDCS.Select("ProductID = " + productId).Length > 0 &&
                Utils.ToBoolean(ConfigurationManager.AppSettings["IsShow" + productName.Replace(" ", string.Empty)]))
            {
                var productDataRow = table3rdDCS.Select("ProductID = " + productId)[0];
                var isDisabled = Utils.ToBoolean(productDataRow["isDisabled"]);
                var name = getFromRemotingRef ? RefData.GetResourceDataByRefIdAndResourceId("Product", productId.ToString(), "en-US", productName) : productName;
                model.Setting.Details.Add(new ObjectInfo { Key = productName.Replace(" ", string.Empty), Value = "Edit " + name });
                if (checkboxType == true)
                {
                    model.Setting.EnableItems.Add(new ObjectInfo
                    {
                        Key = "Enable" + productName.Replace(" ", string.Empty),
                        Value = !isDisabled,
                        Text = name,
                        Tooltip = "Enable " + name
                    });
                }
                else
                {
                    model.Setting.EditItems.Add(new ObjectInfo
                    {
                        Key = "Enable" + productName.Replace(" ", string.Empty),
                        Text = "Enable " + name
                    });
                }
            }

            return model;
        }
    }
}