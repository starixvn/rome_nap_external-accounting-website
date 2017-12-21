namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
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
                case Role.Super:
                    return "superPositionTaking";

                case Role.Master:
                    return "masterPositionTaking";

                case Role.Agent:
                    return "agentPositionTaking";

                case Role.Member:
                    return "outstanding";

                default:
                    throw new ArgumentOutOfRangeException(nameof(roleId));
            }
        }

        public static string GetUsedCreditColumn(int roleId)
        {
            switch (roleId)
            {
                case Role.Super:
                    return "superUsedCredit";

                case Role.Master:
                    return "masterUsedCredit";

                case Role.Agent:
                    return "agentUsedCredit";

                default:
                    throw new ArgumentOutOfRangeException(nameof(roleId), "there is no UsedCredit on member level");
            }
        }

        public static string GetYesterdayCashColumn(int roleId)
        {
            switch (roleId)
            {
                case Role.Super:
                    return "yesterday_superCash";

                case Role.Master:
                    return "yesterday_masterCash";

                case Role.Agent:
                    return "yesterday_agentCash";

                case Role.Member:
                    return "yesterday_cash";

                default:
                    throw new ArgumentOutOfRangeException(nameof(roleId));
            }
        }

        public static string GetCashColumn(int roleId)
        {
            switch (roleId)
            {
                case Role.Super:
                    return "superCash";

                case Role.Master:
                    return "masterCash";

                case Role.Agent:
                    return "agentCash";

                case Role.Member:
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

        public static CustomerModel BuildCustomerSetting(this CustomerModel model, int userId, string groupName, string username)
        {
            var customerStatus = model.GetCustomerStatus(userId, groupName, username, model.PersonalInfo.UplineId == null ? 0 : (int)model.PersonalInfo.UplineId.Value);
            model.GetSuperMasterSettingOnMember();
            model.RemoveProductSetting(customerStatus);

            if (customerStatus.RoleId.IsSuper())
            {
                model.RemoveProductNotVisibleToSite();

                return model;
            }

            model.DisableStatusSettingFollowUplineConstrain(customerStatus);

            return model;
        }

        public static string CheckVNIP(string ip)
         => string.IsNullOrWhiteSpace(ip)
            || (IpUtils.IsVnIp(ip)
                && !IsAuthorized(CurrentSession.Instance.UserId, eFunctions.ViewVNIP, AuthenticateAction.AuthenticationType.Read))
                    ? string.Empty : ip;

        public static CustomerStatus GetCustomerStatus(this CustomerModel model, int userId, string groupName, string username, int uplineId = 0)
        {
            IReportService report = new ReportService();

            DataTable customerInfo = report.GetCustomerInfo(groupName, 0, username, true);
            DataTable upperInfo = uplineId != 0 ? report.GetCustomerInfo(groupName, uplineId, null, true) : null;
            DataTable custData = null;
            int srecommend = 0,
                mrecommend = 0,
                recommend = 0,
                custId = 0,
                roleId = 0;

            if (customerInfo != null)
            {
                var infoRow = customerInfo.Rows[0];
                roleId = Utils.ToInt32(infoRow["roleid"]);
                srecommend = Utils.ToInt32(infoRow["srecommend"]);
                mrecommend = Utils.ToInt32(infoRow["mrecommend"]);
                recommend = Utils.ToInt32(infoRow["recommend"]);
                custId = Utils.ToInt32(infoRow["custid"]);

                if (roleId.IsSuper())
                {
                    custData = new SuperService().GetData(userId, custId, null, null, null, groupName);
                }
                else if (roleId.IsMaster())
                {
                    custData = new MasterService().GetData(userId, srecommend, custId);
                    upperInfo = new SuperService().GetData(userId, uplineId, null, null, null, groupName);
                }
                else if (roleId.IsAgent())
                {
                    custData = new AgentService().GetData(userId, mrecommend, custId);
                    upperInfo = new MasterService().GetData(userId, srecommend, uplineId);
                }
                else
                {
                    custData = new MemberService().GetData(userId, recommend, custId);
                    upperInfo = new AgentService().GetData(userId, mrecommend, uplineId);
                }

                return model.GetCustStatus(roleId, custData, upperInfo);
            }

            return null;
        }

        private static CustomerStatus GetCustStatus(this CustomerModel model, int roleId, DataTable result, DataTable upperInfo)
        {
            var custStatus = new CustomerStatus
            {
                UplineStatus = new CustomerStatus(),
                RoleId = roleId
            };

            if (result.Rows.Count == 0)
            {
                return custStatus;
            }

            var athenticationService = new ExternalAccountAuthenticateAttribute();
            var statusRow = result.Rows[0];

            SetNonProductStatus(roleId, custStatus, statusRow);

            SetNumberGameStatus(custStatus, statusRow);
            SetCasinoStatus(custStatus, statusRow);
            SetLiveCasinoStatus(custStatus, statusRow);
            SetVirtualSportsStatus(custStatus, statusRow);
            SetKenoStatus(custStatus, statusRow);
            SetEsportsStatus(custStatus, athenticationService, statusRow);
            SetSportsbook2Status(custStatus, statusRow);
            SetCockfightingStatus(custStatus, statusRow);
            SetCricketStatus(custStatus, statusRow);
            SetColossusBetsStatus(custStatus, statusRow);
            SetGoldDeluxeStatus(custStatus, statusRow);
            SetAllbetStatus(custStatus, statusRow);
            SetVoidbridgeStatus(custStatus, statusRow);
            SetNG2Status(custStatus, statusRow);
            SetFishingWorldStatus(custStatus, statusRow);
            SetLotteryStatus(custStatus, statusRow);
            SetLucky3Status(custStatus, statusRow);

            model.Setting.MinMaxPts.Add(new ObjectInfo { Key = "MinSB", Value = "Sportsbook" });
            model.Setting.MinMaxPts.Add(new ObjectInfo { Key = "MinRB", Value = "Racing" });
            model.Setting.SuperPts = model.Setting.MasterPts = model.Setting.Details.Where(
                o => o.Key == "PT"
                || o.Key == "Casino"
                || o.Key == "LiveCasino"
                || o.Key == "VirtualSports"
                || o.Key == "Racing")
                .ToList();

            if (roleId.IsNotSuper()
                && upperInfo.Rows.Count > 0)
            {
                SetUplineStatus(custStatus.UplineStatus, roleId, upperInfo.Rows[0]);
            }

            return custStatus;
        }

        private static void SetNonProductStatus(int roleId, CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.Closed = statusRow.ToBoolean("closed");
            custStatus.Suspended = statusRow.ToBoolean("suspended");
            custStatus.DisableTransfer = roleId != 4 && statusRow.ToBoolean("disableTransfer");
            custStatus.AllowDoubleComm = statusRow.ToBoolean("doubleCommLosing");
            custStatus.AllowOutright = !statusRow.ToBoolean("disableOutright");
        }

        private static void SetNumberGameStatus(CustomerStatus custStatus, DataRow statusRow)
        => custStatus.DisableNumberGame = statusRow.ToBoolean("disableNumber");

        private static void SetColossusBetsStatus(CustomerStatus custStatus, DataRow statusRow)
        => custStatus.HideColossusBets = ConfigurationManager.AppSettings["IsShowColossusBets"] == "0" || statusRow.ToBoolean("HideColossusBet");

        private static void SetCricketStatus(CustomerStatus custStatus, DataRow statusRow)
        => custStatus.HideCricket = statusRow.ToBoolean("HideCricket")
                            || ConfigurationManager.AppSettings["IsShowCricket"] == "0" || !statusRow.ToBoolean("IsIMBC");

        private static void SetCockfightingStatus(CustomerStatus custStatus, DataRow statusRow)
        => custStatus.HideCockfighting = statusRow.ToBoolean("HideCockfighting") || ConfigurationManager.AppSettings["IsShowCockfighting"] == "0";

        private static void SetSportsbook2Status(CustomerStatus custStatus, DataRow statusRow)
        => custStatus.HideSportsbook2 = statusRow.ToBoolean("HideMuayThai") || ConfigurationManager.AppSettings["IsShowSportsbook2"] == "0";

        private static void SetLiveCasinoStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.SyncLiveCasino = statusRow.ToBoolean("syncLiveCasino");
            custStatus.DisableLiveCasino = statusRow.ToBoolean("disableLiveCasino");
        }

        private static void SetCasinoStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.SyncCasino = statusRow.ToBoolean("syncCasino");
            custStatus.DisableCasino = statusRow.ToBoolean("disableCasino");
        }

        private static void SetEsportsStatus(CustomerStatus custStatus, ExternalAccountAuthenticateAttribute athenticationService, DataRow statusRow)
        {
            custStatus.HideEsports = !athenticationService.AllowReadByAuthenticationParam(eFunctions.ShowEnableEsports) || Utils.ToBoolean(ConfigurationManager.AppSettings["HideEsports"]);
            custStatus.DisableESports = statusRow.ToBoolean("DisableESports");
        }

        private static void SetVirtualSportsStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideVirtualSports = statusRow.ToBoolean("hideVirtualSports");
            custStatus.DisableVirtualSports = statusRow.ToBoolean("disableVirtualSports");
            custStatus.DisableHRFixedOdds = statusRow.ToBoolean("disableHRFixedOdds");
        }

        private static void SetKenoStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideKeno = statusRow.ToBoolean("HideKeno") || !KenoSection.Instance.ShowSetting;
            custStatus.DisableKeno = statusRow.ToBoolean("DisableKeno");
        }

        private static void SetGoldDeluxeStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideGoldDeluxe = statusRow.ToBoolean("HideGoldDeluxe") || ConfigurationManager.AppSettings["IsShowGoldDeluxeSettingForCredit"] == "0";
            custStatus.SyncGoldDeluxeDownlines = statusRow.ToBoolean("SyncedGoldDeluxeDownline");
        }

        private static void SetLucky3Status(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideLucky3 = ConfigurationManager.AppSettings["IsShowLucky3"] == "0" || statusRow.ToBoolean("HideLucky3");
            custStatus.SyncLucky3 = true;
            custStatus.SyncLucky3Downlines = true;
            custStatus.DisableLucky3 = statusRow.ToBoolean("DisableLucky3");
        }

        private static void SetLotteryStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideLottery = ConfigurationManager.AppSettings["IsShowLottery"] == "0" || statusRow.ToBoolean("HideLottery");
            custStatus.SyncLottery = true;
            custStatus.SyncLotteryDownlines = true;
            custStatus.DisableLottery = statusRow.ToBoolean("DisableLottery");
        }

        private static void SetFishingWorldStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideFishingWorld = ConfigurationManager.AppSettings["IsShowFishingWorld"] == "0" || statusRow.ToBoolean("HideFishingWorld");
            custStatus.SyncFishingWorld = statusRow.ToBoolean("SyncedFishingWorld");
            custStatus.SyncFishingWorldDownlines = statusRow.ToBoolean("SyncedFishingWorldDownline");
            custStatus.DisableFishingWorld = statusRow.ToBoolean("DisableFishingWorld");
        }

        private static void SetNG2Status(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideNG2 = ConfigurationManager.AppSettings["IsShowNG2"] == "0" || statusRow.ToBoolean("HideNG2");
            custStatus.SyncNG2 = statusRow.ToBoolean("SyncedNG2");
            custStatus.DisableNG2 = statusRow.ToBoolean("DisableNG2");
        }

        private static void SetVoidbridgeStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideVoidbridge = ConfigurationManager.AppSettings["IsShowVoidbridge"] == "0" || statusRow.ToBoolean("HideVoidBridge");
            custStatus.SyncVoidbridge = statusRow.ToBoolean("SyncedVoidBridge");
            custStatus.SyncVoidbridgeDownlines = statusRow.ToBoolean("SyncedVoidBridgeDownline");
            custStatus.DisableVoidbridge = statusRow.ToBoolean("DisableVoidBridge");
        }

        private static void SetAllbetStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideAllbet = ConfigurationManager.AppSettings["IsShowAllbet"] == "0" || statusRow.ToBoolean("HideAllbet");
            custStatus.SyncAllbet = statusRow.ToBoolean("SyncedAllBet");
            custStatus.SyncAllbetDownlines = statusRow.ToBoolean("SyncedAllBetDownline");
            custStatus.DisableAllbet = statusRow.ToBoolean("DisableAllbet");
        }

        private static void SetUplineStatus(CustomerStatus uplineStatus, int roleId, DataRow uplineRow)
        {
            uplineStatus.RoleId = roleId + 1;
            uplineStatus.SyncCasino = uplineRow.ToBoolean("syncCasino");
            uplineStatus.SyncLiveCasino = uplineRow.ToBoolean("syncLiveCasino");
            uplineStatus.DisableTransfer = roleId + 1 != 4 && uplineRow.ToBoolean("disableTransfer");
            uplineStatus.AllowDoubleComm = uplineRow.ToBoolean("doubleCommLosing");
            uplineStatus.DisableVirtualSports = uplineRow.ToBoolean("disableVirtualSports");
            uplineStatus.AllowOutright = !uplineRow.ToBoolean("disableOutright");
            uplineStatus.DisableNumberGame = uplineRow.ToBoolean("disableNumber");
            uplineStatus.DisableCasino = uplineRow.ToBoolean("disableCasino");
            uplineStatus.DisableLiveCasino = uplineRow.ToBoolean("disableLiveCasino");
            uplineStatus.DisableGoldDeluxe = uplineRow.ToBoolean("DisableGoldDeluxe");
            uplineStatus.HideGoldDeluxe = uplineRow.ToBoolean("HideGoldDeluxe") || ConfigurationManager.AppSettings["IsShowGoldDeluxeSettingForCredit"] == "0";

            uplineStatus.DisableColossusBets = uplineRow.ToBoolean("DisableColossusBet");
            uplineStatus.Closed = uplineRow.ToBoolean("closed");
            uplineStatus.Suspended = uplineRow.ToBoolean("suspended");
            uplineStatus.HideVirtualSports = uplineRow.ToBoolean("hideVirtualSports");
            uplineStatus.DisableHRFixedOdds = uplineRow.ToBoolean("disableHRFixedOdds");
            uplineStatus.DisableESports = uplineRow.ToBoolean("DisableESports");

            SetUplineAllBetStatus(uplineStatus, uplineRow);
            SetUplineVoidbridgeStatus(uplineStatus, uplineRow);
            SetUplineNG2Status(uplineStatus, uplineRow);
            SetUplineFishingWorldStatus(uplineStatus, uplineRow);
            SetUplineLotteryStatus(uplineStatus, uplineRow);
            SetUplineLucky3Status(uplineStatus, uplineRow);
        }

        private static void SetUplineLucky3Status(CustomerStatus uplineStatus, DataRow uplineRow)
        {
            uplineStatus.DisableLucky3 = uplineRow.ToBoolean("DisableLucky3");
            uplineStatus.HideLucky3 = uplineRow.ToBoolean("HideLucky3");
            uplineStatus.SyncLucky3 = true;
        }

        private static void SetUplineLotteryStatus(CustomerStatus uplineStatus, DataRow uplineRow)
        {
            uplineStatus.DisableLottery = uplineRow.ToBoolean("DisableLottery");
            uplineStatus.HideLottery = uplineRow.ToBoolean("HideLottery");
            uplineStatus.SyncLottery = true;
        }

        private static void SetUplineFishingWorldStatus(CustomerStatus uplineStatus, DataRow uplineRow)
        {
            uplineStatus.DisableFishingWorld = uplineRow.ToBoolean("DisableFishingWorld");
            uplineStatus.HideFishingWorld = uplineRow.ToBoolean("HideFishingWorld");
            uplineStatus.SyncFishingWorld = uplineRow.ToBoolean("SyncedFishingWorld");
        }

        private static void SetUplineNG2Status(CustomerStatus uplineStatus, DataRow uplineRow)
        {
            uplineStatus.DisableNG2 = uplineRow.ToBoolean("DisableNG2");
            uplineStatus.HideNG2 = uplineRow.ToBoolean("HideNG2");
            uplineStatus.SyncNG2 = uplineRow.ToBoolean("SyncedNG2");
        }

        private static void SetUplineVoidbridgeStatus(CustomerStatus uplineStatus, DataRow uplineRow)
        {
            uplineStatus.DisableVoidbridge = uplineRow.ToBoolean("DisableVoidBridge");
            uplineStatus.HideVoidbridge = uplineRow.ToBoolean("HideVoidBridge");
            uplineStatus.SyncVoidbridge = uplineRow.ToBoolean("SyncedVoidBridge");
        }

        private static void SetUplineAllBetStatus(CustomerStatus uplineStatus, DataRow uplineRow)
        {
            uplineStatus.DisableAllbet = uplineRow.ToBoolean("DisableAllbet");
            uplineStatus.HideAllbet = uplineRow.ToBoolean("HideAllbet");
            uplineStatus.SyncAllbet = uplineRow.ToBoolean("SyncedAllBet");
        }

        public static CustomerModel AddProductSetting(
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