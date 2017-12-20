namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Core.Context;
    using CustomerSetting.IP.Utils;
    using CustomerSetting.Module.Keno.KenoSetting.Configurations;
    using CustomerSetting.Security.Authenticate;
    using DTO;
    using DTO.Customer;
    using RemotingRef;
    using Service.Impl.Customer;
    using Service.Impl.Report;
    using Service.Impl.UserControl;
    using Service.Interface.Report;
    using Shared;
    using Utilities;
    using ViewModels;

    public static class CustomerModelExtensions
    {
        private const string DateTimeFormat = "M/d/yyyy H:mm tt";
        private const string ThisAccountWas = "This account was ";

        public static string GetOutstandingColumn(int roleId)
        {
            switch (roleId)
            {
                case 4:
                    return "superPositionTaking";

                case 3:
                    return "masterPositionTaking";

                case 2:
                    return "agentPositionTaking";
                case 1:
                    return "outstanding";
                default:
                    throw new ArgumentOutOfRangeException(nameof(roleId));
            }
        }

        public static string GetUsedCreditColumn(int roleId)
        {
            switch (roleId)
            {
                case 4:
                    return "superUsedCredit";
                case 3:
                    return "masterUsedCredit";
                case 2:
                    return "agentUsedCredit";
                default:
                    throw new ArgumentOutOfRangeException(nameof(roleId), "there is no UsedCredit on member level");
            }
        }

        public static string GetYesterdayCashColumn(int roleId)
        {
            switch (roleId)
            {
                case 4:
                    return "yesterday_superCash";
                case 3:
                    return "yesterday_masterCash";
                case 2:
                    return "yesterday_agentCash";
                case 1:
                    return "yesterday_cash";
                default:
                    throw new ArgumentOutOfRangeException(nameof(roleId));
            }
        }

        public static string GetCashColumn(int roleId)
        {
            switch (roleId)
            {
                case 4:
                    return "superCash";
                case 3:
                    return "masterCash";
                case 2:
                    return "agentCash";
                case 1:
                    return "cash";
                default:
                    throw new ArgumentOutOfRangeException(nameof(roleId));
            }
        }


        public static bool IsAuthorized(int userId, string functionKey, AuthenticateAction.AuthenticationType policy)
        {
            var userCache = new CachingService().GetUserCache(userId);

            var menuList = userCache.MenuList;
            var foundItems = userCache.PolicyList.Where(x => x.FuncKey == functionKey).ToList();

            var hasPerm = menuList.Any(menuItem => !string.IsNullOrWhiteSpace(menuItem.Url));

            if (!hasPerm && foundItems.Any())
            {
                return false;
            }

            if (foundItems.Count <= 0)
            {
                return false;
            }

            var firstItem = foundItems.First();

            if (policy == AuthenticateAction.AuthenticationType.Read)
            {
                if (firstItem.Policy == EPolicy.Read || firstItem.Policy == EPolicy.Update ||
                    firstItem.Policy == EPolicy.Delete
                    || firstItem.Policy == EPolicy.Create)
                {
                    return true;
                }
            }
            else
            {
                return firstItem.Policy != EPolicy.Read;
            }

            return false;
        }

        public static CustomerModel LoginInfo(this CustomerModel model, DataSet ds)
        {
            var tableCash = ds.Tables[2];
            var customerRow = ds.Tables[0].Rows[0];
            var roleId = Utils.ToInt32(customerRow["roleid"]);

            model.Login = new List<ObjectInfo>
                {
                    new ObjectInfo { Value = roleId != 1 ? Utils.ToString(customerRow["site"]) : Utils.ToString(customerRow["membersite"]), Text = "Site" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["createdate"]).ToStringExt(DateTimeFormat), Text = "Created Date" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["lastlogged"]).ToStringExt(DateTimeFormat), Text = "Last Login" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCash.Rows[0]["lastTxn"]).ToStringExt(DateTimeFormat), Text = "Last Txn", Tooltip = "Last Transaction" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["pwdexpiry"]).ToStringExt(DateTimeFormat), Text = "Password" },
                    new ObjectInfo { Value = CheckVNIP(customerRow.ToString("lastloginip")), Text = "Last Login IP" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["LastChangedSecurityCodeDate"]).ToStringExt(DateTimeFormat), Text = "Security Code" }
                };

            if (roleId != 1)
            {
                model.Login.Add(
                    new ObjectInfo
                    {
                        Value = Utils.ToBoolean(customerRow["NicknameLoginOnly"]) ? "Nickname Only" : "Username/Nickname",
                        Text = "Login Options"
                    });

                model.Login.Add(
                  new ObjectInfo
                  {
                      Value = Utils.ToDateTime(customerRow["OTPCreateDate"]).ToStringExt(DateTimeFormat),
                      Text = "OTP"
                  });

                model.BalanceInfo.Balances.Add(new ObjectInfo { Value = tableCash.Rows[0][GetUsedCreditColumn(roleId)], Text = "Used Credit" });
            }

            return model;
        }

        public static CustomerModel PersonalInfo(this CustomerModel model, DataSet ds)
        {

            var customerRow = ds.Tables[0].Rows[0];
            var roleId = Utils.ToInt32(customerRow["roleid"]);
            var enabled = customerRow.ToBoolean("enabled");
            var closed = customerRow.ToBoolean("closed");
            var suspended = customerRow.ToBoolean("suspended");
            var status = ThisAccountWas;
            var closedReason = string.Empty;

            if (closed)
            {
                var customerClosedReason = new CustomerClosedReason(customerRow["closedreason"]);
                closedReason = customerClosedReason.ClassName;
                status = customerClosedReason.ClosedReason;
            }

            model.PersonalInfo = new PersonalInfo
            {
                CustId = new ObjectInfo { Value = Utils.ToInt32(customerRow["custid"]), Text = "Cust Id" },
                Fax = new ObjectInfo { Value = Utils.ToString(customerRow["fax"]), Text = "Fax" },
                Mobile = new ObjectInfo { Value = Utils.ToString(customerRow["mobilephone"]), Text = "Mobile" },
                Name = new ObjectInfo { Value = Utils.ToString(customerRow["firstname"]) + " " + Utils.ToString(customerRow["lastname"]), Text = "Name" },
                Nickname = new ObjectInfo { Value = Utils.ToString(customerRow["username2"]), Text = "Nickname" },
                Phone = new ObjectInfo { Value = Utils.ToString(customerRow["phone"]), Text = "Phone" },
                RoleId = new ObjectInfo { Value = roleId, Text = "Role Id" },
                UplineRoleId = new ObjectInfo { Value = roleId + 1, Text = "Upline Role Id" },
                CustList = new ObjectInfo { Key = "CustList", Value = roleId == 4 ? "Master List" : roleId == 3 ? "Agent List" : "Member List" },
                Status = new ObjectInfo { Tooltip = !enabled ? status + "disabled" : closed ? status : suspended ? status + "suspended" : string.Empty },
                BackgroundColor = new ObjectInfo { Value = !enabled ? "disabled" : closed ? closedReason : suspended ? "suspended" : string.Empty },
                IsInternal = new ObjectInfo { Value = Utils.ToBoolean(customerRow["isinternal"]) }
            };

            return model;
        }

        public static CustomerModel BalanceInfo(this CustomerModel model, DataSet ds)
        {

            var customerRow = ds.Tables[0].Rows[0];
            var roleId = Utils.ToInt32(customerRow["roleid"]);
            var cashRow = ds.Tables[2].Rows[0];

            model.BalanceInfo = new BalanceInfo
            {
                CurrencyId = new ObjectInfo { Value = Utils.ToInt32(customerRow["exchangeid"]), Text = "Currency Id" },
                Currency = new ObjectInfo { Value = Utils.ToString(customerRow["currency"]), Text = "Currency" },
                Balances = new List<ObjectInfo>
                {
                        new ObjectInfo { Value = Utils.ToDecimal(cashRow[GetCashColumn(roleId)]), Text = "Total Cash" },
                        new ObjectInfo { Value = Utils.ToDecimal(cashRow[GetYesterdayCashColumn(roleId)]), Text = "Yesterday Cash" },
                        new ObjectInfo { Value = Utils.ToDecimal(cashRow[GetOutstandingColumn(roleId)]), Text = "Outstanding" },
                        new ObjectInfo { Value = Utils.ToDecimal(customerRow["credit"]), Text = "Given Credit" }
                    }
            };

            model.BalanceInfo.IsUpdateCredit
                = IsAuthorized(CurrentSession.Instance.UserId, eFunctions.UpdateCredit, AuthenticateAction.AuthenticationType.Host);

            return model;
        }

        public static CustomerModel GetProductStatuses(this CustomerModel model, int userId, string groupName, string username)
        {
            var productStatusInfo = model.Get3rdPartyProducts(userId, groupName, username, model.PersonalInfo.UplineId == null ? 0 : (int)model.PersonalInfo.UplineId.Value);
            model.GetSuperMasterPtsOnMember();
            model.RemoveEnableAndPtforProducts(productStatusInfo);

            if (productStatusInfo.RoleId.IsSuper())
            {
                model.CheckConfig();

                return model;
            }

            model.CheckAndDisableStatusSettingFollowUpline(productStatusInfo);

            return model;
        }

        public static CustomerModel CheckConfig(this CustomerModel model)
        {
            var hideVS = !Utils.ToBoolean(ConfigurationManager.AppSettings["EnableVirtualSports"]);
            var hideKeno = !KenoSection.Instance.ShowSetting;
            var hideGoldDeluxe = ConfigurationManager.AppSettings["IsShowGoldDeluxeSettingForCredit"].Equals("0");
            var hideColossusBets = ConfigurationManager.AppSettings["IsShowColossusBets"].Equals("0");

            if (hideVS)
            {
                model.RemoveProductSetting("VirtualSports");
            }

            if (hideKeno)
            {
                model.RemoveProductSetting("Keno");
            }

            if (hideGoldDeluxe)
            {
                model.RemoveProductSetting("GoldDeluxe");
            }

            if (hideColossusBets)
            {
                model.RemoveProductSetting("ColossusBets");
            }

            return model;
        }

        public static string CheckVNIP(string ip)
        {
            return string.IsNullOrWhiteSpace(ip) || (
                IpUtils.IsVnIp(ip) && !IsAuthorized(CurrentSession.Instance.UserId, eFunctions.ViewVNIP, AuthenticateAction.AuthenticationType.Read)) ? string.Empty : ip;
        }

        private static CustomerModel RemoveEnableAndPtforProducts(this CustomerModel model, ProductStatuses product)
        {
            if (!product.SyncCasino)
            {
                model.RemoveProductStatusSetting("EnableCasino");

                if (!product.UplineStatuses.SyncCasino)
                {
                    model.RemoveProductSetting("Casino")
                        .RemoveCrossLevelProductSetting("Casino");
                }
            }

            if (!product.SyncLiveCasino)
            {
                model.RemoveProductStatusSetting("EnableLiveCasino");

                if (!product.UplineStatuses.SyncLiveCasino)
                {
                    model.RemoveProductSetting("LiveCasino")
                        .RemoveCrossLevelProductSetting("LiveCasino");
                }
            }

            if (product.HideVirtualSports)
            {
                model.RemoveProductStatusSetting("EnableVirtualSports");

                bool hideVirtualSports = product.RoleId == 1 ?
                    product.UplineStatuses.HideVirtualSports :
                    product.HideVirtualSports;

                if (hideVirtualSports)
                {
                    model.RemoveProductSetting("VirtualSports")
                        .RemoveCrossLevelProductSetting("VirtualSports");
                }
            }

            if (product.HideEsports)
            {
                model.RemoveProductStatusSetting("EnableESports")
                    .RemoveProductSetting("ESports")
                    .RemoveCrossLevelProductSetting("ESports");
            }

            if (product.HideGoldDeluxe)
            {
                model.RemoveProductStatusSetting("EnableGoldDeluxe");
            }

            if (product.UplineStatuses.HideGoldDeluxe)
            {
                model.RemoveProductSetting("GoldDeluxe")
                    .RemoveCrossLevelProductSetting("GoldDeluxe");
            }

            if (product.HideColossusBets)
            {
                model.RemoveProductStatusSetting("EnableColossusBets")
                    .RemoveProductSetting("ColossusBets");
            }

            if (product.HideSportsbook2)
            {
                model.RemoveProductSetting("Sportsbook2")
                    .RemoveCrossLevelProductSetting("Sportsbook2");
            }

            if (product.HideCockfighting)
            {
                model.RemoveProductStatusSetting("EnableCockfighting")
                    .RemoveProductSetting("Cockfighting")
                    .RemoveCrossLevelProductSetting("Cockfighting");
            }

            if (product.HideCricket)
            {
                model.RemoveProductSetting("Cricket")
                    .RemoveCrossLevelProductSetting("Cricket");
            }

            model.RemoveEnableAndEditProductDCS(
                product.RoleId,
                product.RoleId.IsNotSuper() ? product.UplineStatuses.HideAllbet : product.HideAllbet,
                product.RoleId.IsNotSuper() ? product.UplineStatuses.SyncAllbet : product.SyncAllbet,
                product.HideAllbet,
                product.SyncAllbet,
                product.SyncAllbetDownlines,
                "Allbet",
                false);

            model.RemoveEnableAndEditProductDCS(
                product.RoleId,
                product.RoleId.IsNotSuper() ? product.UplineStatuses.HideVoidbridge : product.HideVoidbridge,
                product.RoleId.IsNotSuper() ? product.UplineStatuses.SyncVoidbridge : product.SyncVoidbridge,
                product.HideVoidbridge,
                product.SyncVoidbridge,
                product.SyncVoidbridgeDownlines,
                "MacauGames",
                false);

            model.RemoveEnableAndEditProductDCS(
                product.RoleId,
                product.RoleId.IsNotSuper() ? product.UplineStatuses.HideNG2 : product.HideNG2,
                product.RoleId.IsNotSuper() ? product.UplineStatuses.SyncNG2 : product.SyncNG2,
                product.HideNG2,
                product.SyncNG2,
                true,
                "NG2");

            model.RemoveEnableAndEditProductDCS(
                product.RoleId,
                product.RoleId.IsNotSuper() ? product.UplineStatuses.HideFishingWorld : product.HideFishingWorld,
                product.RoleId.IsNotSuper() ? product.UplineStatuses.SyncFishingWorld : product.SyncFishingWorld,
                product.HideFishingWorld,
                product.SyncFishingWorld,
                product.SyncFishingWorldDownlines,
                "FishingWorld");

            model.RemoveEnableAndEditProductDCS(
                product.RoleId,
                product.RoleId.IsNotSuper() ? product.UplineStatuses.HideLottery : product.HideLottery,
                product.RoleId.IsNotSuper() ? product.UplineStatuses.SyncLottery : product.SyncLottery,
                product.HideLottery,
                product.SyncLottery,
                product.SyncLotteryDownlines,
                "Lottery");

            model.RemoveEnableAndEditProductDCS(
                product.RoleId,
                product.RoleId.IsNotSuper() ? product.UplineStatuses.HideLucky3 : product.HideLucky3,
                product.RoleId.IsNotSuper() ? product.UplineStatuses.SyncLucky3 : product.SyncLucky3,
                product.HideLucky3,
                product.SyncLucky3,
                product.SyncLucky3Downlines,
                "Lucky3");

            return model;
        }

        private static CustomerModel RemoveEnableAndEditProductDCS(
            this CustomerModel model,
            int roleId,
            bool hideUpline,
            bool syncUpline,
            bool hide,
            bool sync,
            bool syncDownline,
            string productName,
            bool isCheckSyncDownlineField = true)
        {
            if ((roleId.IsNotMember() && hide)
                || (roleId.IsMember() && hideUpline))
            {
                return RemoveProduct(model, roleId, productName, true);
            }

            if (!syncUpline)
            {
                if (roleId.IsNotSuper()
                    || (roleId.IsSuper() && hideUpline))
                {
                    model.RemoveProductSetting(productName);
                }

                return RemoveProduct(model, roleId, productName, false);
            }

            if ((!sync && roleId.IsMember())
                || (!hide && !sync)
                || (isCheckSyncDownlineField && !syncDownline))
            {
                return RemoveProduct(model, roleId, productName, false);
            }

            return model;
        }

        private static CustomerModel RemoveProduct(
            CustomerModel model, 
            int roleId, 
            string productName, 
            bool isRemoveProductSetting)
        {
            model.RemoveProductStatusSetting("Enable" + productName);

            if (isRemoveProductSetting)
            {
                model.RemoveProductSetting(productName);
            }

            if (roleId.IsMember())
            {
                model.RemoveCrossLevelProductSetting(productName);
            }

            return model;
        }

        private static CustomerModel CheckAndDisableStatusSettingFollowUpline(this CustomerModel model, ProductStatuses product)
        {
            DisableNonProductStatusSettingFollowUpline(model, product);

            DisableProductStatusSettingFollowUpline(model, product);

            return model;
        }

        private static void DisableNonProductStatusSettingFollowUpline(CustomerModel model, ProductStatuses product)
        {
            if (!product.UplineStatuses.AllowDoubleComm)
            {
                model.DisableStatusSetting("DoubleComm");
            }

            if (product.UplineStatuses.Closed)
            {
                model.DisableStatusSetting("Closed");
            }

            if (product.UplineStatuses.Suspended)
            {
                model.DisableStatusSetting("Suspended");
            }

            if (!product.UplineStatuses.AllowOutright)
            {
                model.DisableStatusSetting("Outright");
            }
        }

        private static void DisableProductStatusSettingFollowUpline(CustomerModel model, ProductStatuses product)
        {
            if (product.UplineStatuses.DisableNumberGame)
            {
                model.DisableStatusSetting("EnableNumberGame");
            }

            if (product.UplineStatuses.DisableVirtualSports)
            {
                model.DisableStatusSetting("EnableVirtualSports");
            }

            if (product.UplineStatuses.DisableESports)
            {
                model.DisableStatusSetting("EnableESports");
            }

            if (product.UplineStatuses.DisableHRFixedOdds)
            {
                model.DisableStatusSetting("EnableHRFixedOdds");
            }

            if (product.UplineStatuses.DisableCasino)
            {
                model.DisableStatusSetting("EnableCasino");
            }

            if (product.UplineStatuses.DisableLiveCasino)
            {
                model.DisableStatusSetting("EnableLiveCasino");
            }

            if (product.UplineStatuses.DisableGoldDeluxe)
            {
                model.DisableStatusSetting("EnableGoldDeluxe");
            }

            if (product.UplineStatuses.DisableColossusBets)
            {
                model.DisableStatusSetting("EnableColossusBets");
            }

            if (product.UplineStatuses.DisableAllbet)
            {
                model.DisableStatusSetting("EnableAllbet");
            }

            if (product.UplineStatuses.DisableVoidbridge)
            {
                model.DisableStatusSetting("EnableMacauGames");
            }

            if (product.UplineStatuses.DisableNG2)
            {
                model.DisableStatusSetting("EnableNG2");
            }

            if (product.UplineStatuses.DisableFishingWorld)
            {
                model.DisableStatusSetting("EnableFishingWorld");
            }

            if (product.UplineStatuses.DisableLottery)
            {
                model.DisableStatusSetting("EnableLottery");
            }

            if (product.UplineStatuses.DisableLucky3)
            {
                model.DisableStatusSetting("EnableLucky3");
            }
        }

        private static CustomerModel RemoveProductSetting(this CustomerModel model, string key)
        {
            foreach (var detail in model.Setting.Details)
            {
                if (detail.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    detail.Visibled = false;

                    break;
                }
            }

            return model;
        }

        private static CustomerModel RemoveCrossLevelProductSetting(this CustomerModel model, string key)
        {
            foreach (var superPt in model.Setting.SuperPts)
            {
                if (superPt.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    superPt.Visibled = false;

                    break;
                }
            }

            return model;
        }

        private static CustomerModel RemoveProductStatusSetting(this CustomerModel model, string key)
        {
            foreach (var enableItem in model.Setting.EnableItems)
            {
                if(enableItem.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    enableItem.Visibled = false;

                    break;
                }
            }

            foreach (var editItem in model.Setting.EditItems)
            {
                if (editItem.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    editItem.Visibled = false;

                    break;
                }
            }

            return model;
        }

        private static CustomerModel DisableStatusSetting(this CustomerModel model, string key)
        {
            foreach (var enableItem in model.Setting.EnableItems)
            {
                if (enableItem.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    enableItem.Disabled = true;
                    enableItem.Tooltip = key.Equals("Closed", StringComparison.OrdinalIgnoreCase) 
                            ? "Upline has been closed" 
                            : key.Equals("Suspended", StringComparison.OrdinalIgnoreCase) 
                                ? "Upline has been suspended" 
                                : "Upline has been disabled";
                }
            }

            return model;
        }

        public static ProductStatuses Get3rdPartyProducts(this CustomerModel model, int userId, string groupName, string username, int uplineId = 0)
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
                    result = new SuperService().GetData(userId, custId, null, null, null, groupName);
                }
                else if (roleId == 3)
                {
                    result = new MasterService().GetData(userId, srecommend, custId);
                    upperInfo = new SuperService().GetData(userId, uplineId, null, null, null, groupName);
                }
                else if (roleId == 2)
                {
                    result = new AgentService().GetData(userId, mrecommend, custId);
                    upperInfo = new MasterService().GetData(userId, srecommend, uplineId);
                }
                else
                {
                    result = new MemberService().GetData(userId, recommend, custId);
                    upperInfo = new AgentService().GetData(userId, mrecommend, uplineId);
                }

                return model.GetProductStatusDetail(roleId, result, upperInfo);
            }

            return null;
        }

        private static ProductStatuses GetProductStatusDetail(this CustomerModel model, int roleId, DataTable result, DataTable upperInfo)
        {
            var product = new ProductStatuses
            {
                UplineStatuses = new ProductStatuses(),
                RoleId = roleId
            };

            if (result.Rows.Count == 0)
            {
                return product;
            }

            var athenticationService = new ExternalAccountAuthenticateAttribute();

            product.SyncCasino = result.Rows[0].ToBoolean("syncCasino");
            product.SyncLiveCasino = result.Rows[0].ToBoolean("syncLiveCasino");
            product.DisableTransfer = roleId != 4 && result.Rows[0].ToBoolean("disableTransfer");
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
            product.HideCricket = result.Rows[0].ToBoolean("HideCricket")
                || ConfigurationManager.AppSettings["IsShowCricket"] == "0" || !result.Rows[0].ToBoolean("IsIMBC");
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
            model.Setting.SuperPts = model.Setting.MasterPts = model.Setting.Details.Where(o => o.Key == "PT" || o.Key == "Casino"
                || o.Key == "LiveCasino" || o.Key == "VirtualSports" || o.Key == "Racing").ToList();

            if (roleId < 4 && upperInfo.Rows.Count > 0)
            {
                product.UplineStatuses.RoleId = roleId + 1;
                product.UplineStatuses.SyncCasino = upperInfo.Rows[0].ToBoolean("syncCasino");
                product.UplineStatuses.SyncLiveCasino = upperInfo.Rows[0].ToBoolean("syncLiveCasino");
                product.UplineStatuses.DisableTransfer = roleId + 1 != 4 && upperInfo.Rows[0].ToBoolean("disableTransfer");
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



        public static CustomerModel AddEnableAndDetailProduct(
            this CustomerModel model, 
            DataTable table3rdDCS, 
            int productId, 
            string defaultProductName, 
            string editBetLimitFunc = "", 
            bool statusSettingIsCheckbox = true, 
            bool getFromRemotingRef = false)
        {

            var productNameNoSpace = defaultProductName.Replace(" ", string.Empty);

            if (IsProductVisibledToSite(table3rdDCS, productId, defaultProductName))
            {
                var productDataRow = table3rdDCS.Select($"ProductID = {productId}")[0];
                var isDisabled = Utils.ToBoolean(productDataRow["isDisabled"]);
                string productName = GetProductName(productId, defaultProductName, getFromRemotingRef);

                AddEditSettingLink(model, productNameNoSpace, productName);

                if (statusSettingIsCheckbox)
                {
                    AddEnableCheckboxItem(model, productNameNoSpace, isDisabled, productName);
                }
                else
                {
                    AddEnableLinkItem(model, productNameNoSpace, productName);
                }
            }

            return model;
        }

        private static void AddEditSettingLink(CustomerModel model, string productNameNoSpace, string productName)
        => model.Setting.Details.Add(
                                new ObjectInfo
                                {
                                    Key = productNameNoSpace,
                                    Value = $"Edit {productName}"
                                });

        private static bool IsProductVisibledToSite(DataTable table3rdDCS, int productId, string defaultProductName)
         => table3rdDCS.Select("ProductID = " + productId).Length > 0
                && Utils.ToBoolean(ConfigurationManager.AppSettings["IsShow" + defaultProductName.Replace(" ", string.Empty)]);

        private static string GetProductName(int productId, string productName, bool getFromRemotingRef)
        => getFromRemotingRef
                ? RefData.GetResourceDataByRefIdAndResourceId(
                        "Product", 
                        productId.ToString(), 
                        "en-US", 
                        productName)
                : productName;

        private static void AddEnableLinkItem(CustomerModel model, string productNameNoSpace, string name)
            => model.Setting.EditItems.Add(new ObjectInfo
            {
                Key = $"Enable{productNameNoSpace}",
                Text = $"Enable {name}"
            });

        private static void AddEnableCheckboxItem(
            CustomerModel model, 
            string productNameNoSpace, 
            bool isDisabled, 
            string name)
            => model.Setting.EnableItems.Add(new ObjectInfo
            {
                Key = $"Enable{productNameNoSpace}",
                Value = !isDisabled,
                Text = name,
                Tooltip = $"Enable {name}"
            });
    }
}