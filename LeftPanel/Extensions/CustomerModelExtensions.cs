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
            var product = model.Get3rdPartyProducts(userId, groupName, username, model.PersonalInfo.UplineId == null ? 0 : (int)model.PersonalInfo.UplineId.Value);
            model.GetSuperMasterPtsOnMember();
            model.RemoveEnableAndPtforProducts(product);

            if (product.RoleId.IsSuper())
            {
                model.CheckConfig();

                return model;
            }

            model.DisableUplineStatuses(product);

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
                model.PersonalInfo.IsShowProducts[showProductIndex + 1].Value = false;
            }

            if ((roleId.IsNotMember() && hide)
                || (roleId.IsMember() && hideUpline))
            {
                model.RemoveEnableProduct("Enable" + productName);
                model.RemoveProduct(productName);

                model.PersonalInfo.IsShowProducts[showProductIndex].Value = false;

                if (roleId.IsMember())
                {
                    model.RemoveCrossLevelProduct(productName);
                }

                return model;
            }

            if (hideUpline || !syncUpline)
            {
                model.RemoveEnableProduct("Enable" + productName);
                if ((roleId.IsNotSuper())
                    || (roleId.IsSuper() && hideUpline))
                {
                    model.RemoveProduct(productName);
                }

                model.PersonalInfo.IsShowProducts[showProductIndex].Value = false;

                if (roleId.IsMember())
                {
                    model.RemoveCrossLevelProduct(productName);
                }
            }
            else if ((!sync && roleId.IsMember())
                || (!hide && !sync)
                || (isCheckSyncDownlineField && !syncDownline))
            {
                model.RemoveEnableProduct("Enable" + productName);
                model.PersonalInfo.IsShowProducts[showProductIndex].Value = false;
                if (roleId.IsMember())
                {
                    model.RemoveCrossLevelProduct(productName);
                }
            }

            return model;
        }


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

        private static CustomerModel RemoveProduct(this CustomerModel model, string key)
        {
            int index = -1;
            for (int i = 0; i < model.Setting.Details.Count; i++)
            {
                if (model.Setting.Details[i].Key.Equals(key, StringComparison.OrdinalIgnoreCase))
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
                if (model.Setting.SuperPts[i].Key.Equals(key, StringComparison.OrdinalIgnoreCase))
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

        private static CustomerModel RemoveEnableProduct(this CustomerModel model, string key)
        {
            int index = -1;
            for (int i = 0; i < model.Setting.EnableItems.Count; i++)
            {
                if (model.Setting.EnableItems[i].Key.Equals(key, StringComparison.OrdinalIgnoreCase))
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
                    if (model.Setting.EditItems[i].Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        index = i;
                        model.Setting.EditItems.RemoveAt(index);
                        break;
                    }
                }
            }

            return model;
        }

        private static CustomerModel DisableProduct(this CustomerModel model, string key)
        {
            int index = -1;
            for (int i = 0; i < model.Setting.EnableItems.Count; i++)
            {
                if (model.Setting.EnableItems[i].Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                model.Setting.EnableItems[index].Disabled = true;
                model.Setting.EnableItems[index].Tooltip = key.Equals("Closed", StringComparison.OrdinalIgnoreCase) ?
                    "Upline has been closed" : key.Equals("Suspended", StringComparison.OrdinalIgnoreCase) ? "Upline has been suspended" : "Upline has been disabled";
            }

            return model;
        }

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
                    result = new MasterService().GetData(userId, srecommend, custId);
                    upperInfo = new SuperService().GetData(userId, null, uplineId, null, null, null, groupName);
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

        

        public static CustomerModel AddEnableAndDetailProduct(this CustomerModel model, DataTable table3rdDCS, int productId, string productName, string editBetLimitFunc = "", bool checkboxType = true, bool getFromRemotingRef = false)
        {

            var productNameNoSpace = productName.Replace(" ", string.Empty);

            model
                .PersonalInfo
                .IsShowProducts
                .Add(new ObjectInfo
                {
                    Key = "isShow" + productNameNoSpace,
                    Value = true
                });
            model
                .PersonalInfo
                .IsShowProducts
                .Add(new ObjectInfo
                {
                    Key = "isShow" + productNameNoSpace + "Betting",
                    Value = !string.IsNullOrEmpty(editBetLimitFunc)
                            && IsAuthorized(CurrentSession.Instance.UserId, editBetLimitFunc, AuthenticateAction.AuthenticationType.Read)
                });

            if (table3rdDCS.Select("ProductID = " + productId).Length > 0
                && Utils.ToBoolean(ConfigurationManager.AppSettings["IsShow" + productName.Replace(" ", string.Empty)]))
            {
                var productDataRow = table3rdDCS.Select("ProductID = " + productId)[0];
                var isDisabled = Utils.ToBoolean(productDataRow["isDisabled"]);

                var name = getFromRemotingRef
                    ? RefData.GetResourceDataByRefIdAndResourceId("Product", productId.ToString(), "en-US", productName)
                    : productName;

                model.Setting.Details.Add(
                    new ObjectInfo
                    {
                        Key = productNameNoSpace, Value = "Edit " + name
                    });

                if (checkboxType)
                {
                    model.Setting.EnableItems.Add(new ObjectInfo
                    {
                        Key = "Enable" + productNameNoSpace,
                        Value = !isDisabled,
                        Text = name,
                        Tooltip = "Enable " + name
                    });
                }
                else
                {
                    model.Setting.EditItems.Add(new ObjectInfo
                    {
                        Key = "Enable" + productNameNoSpace,
                        Text = "Enable " + name
                    });
                }
            }

            return model;
        }
    }
}