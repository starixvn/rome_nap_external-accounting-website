namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using Core.Authorization;
    using Core.Shared;
    using DTO.Customer;
    using Extensions;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Utilities;
    using ViewModels;

    public interface CustomerInfoQueryService
    {
        CustomerModel GetCustomer(int actorId, string custName, string groupName, DataSet customerInfoDataset);
    }

    public abstract class CustomerInfoQueryBaseService : CustomerInfoQueryService
    {
        private const string DateTimeFormat = "M/d/yyyy H:mm tt";
        private const string ThisAccountWas = "This account was ";

        protected readonly AuthorizationService authorizationHandler;
        protected readonly CustomerStatusService customerStatusService;

        protected CustomerInfoQueryBaseService(
            AuthorizationService authorizationHandler = null,
            CustomerStatusService customerStatusService = null)
        {
            this.authorizationHandler = authorizationHandler ?? new AuthorizationServiceImpl();
            this.customerStatusService = customerStatusService ?? new CustomerStatusServiceImpl();
        }

        public CustomerModel GetCustomer(int actorId, string custName, string groupName, DataSet customerInfoDataset)
        {
            var custInfoTable = customerInfoDataset.Tables[0];
            var custInfoRow = custInfoTable.Rows[0];
            var cashRow = customerInfoDataset.Tables[2].Rows[0];
            var productStatusTable = customerInfoDataset.Tables[4];
            var lineRow = customerInfoDataset.Tables[1].Rows[0];
            var roleId = Utils.ToInt32(custInfoRow["roleid"]);

            var customerModel = new CustomerModel
            {
                HasDownline = Utils.ToBoolean(custInfoRow["HasDownline"]),
                UplineHasSecurityCode = Utils.ToBoolean(custInfoRow["UplineHasSecurityCode"]),
                CurrencyId = custInfoRow.ToInt32("exchangeid"),
                BalanceInfo = GetBalanceInfo(actorId, custInfoRow, cashRow),
                PersonalInfo = GetPersonalInfo(custInfoRow, lineRow),
                Login = GetLoginInfo(cashRow, custInfoRow)
            };

            var customerStatus = customerStatusService.GetCustomerStatus(
                actorId,
                groupName,
                custName,
                (int?)customerModel.PersonalInfo.UplineId?.Value ?? 0);

            customerModel.Setting = GetCustomerSetting(roleId, custInfoTable, productStatusTable, customerStatus);

            SetCustomSettingByLevel(customerModel.Setting, actorId, customerInfoDataset);

            return customerModel;
        }

        internal virtual BalanceInfo GetBalanceInfo(int userId, DataRow customerRow, DataRow cashRow)
        {
            var roleId = Utils.ToInt32(customerRow["roleid"]);

            var balanceInfo = new BalanceInfo
            {
                CurrencyId = new ObjectInfo { Value = Utils.ToInt32(customerRow["exchangeid"]), Text = "Currency Id" },
                Currency = new ObjectInfo { Value = Utils.ToString(customerRow["currency"]), Text = "Currency" },
                Balances = new List<ObjectInfo>
                {
                    new ObjectInfo { Value = Utils.ToDecimal(cashRow[GetCashColumn(roleId)]), Text = "Total Cash" },
                    new ObjectInfo { Value = Utils.ToDecimal(cashRow[GetYesterdayCashColumn(roleId)]), Text = "Yesterday Cash" },
                    new ObjectInfo { Value = Utils.ToDecimal(cashRow[GetOutstandingColumn(roleId)]), Text = "Outstanding" },
                    new ObjectInfo { Value = Utils.ToDecimal(customerRow["credit"]), Text = "Given Credit" },
                },
                IsUpdateCredit = authorizationHandler.HasUpdateCreditPermission(userId)
            };

            if (roleId.IsNotMember())
            {
                balanceInfo.Balances.Add(new ObjectInfo { Value = cashRow[GetUsedCreditColumn(roleId)], Text = "Used Credit" });
            }

            return balanceInfo;
        }

        internal static string GetOutstandingColumn(int roleId) => roleId.IsMember() ? "outstanding" : $"{roleId.DisplayRoleName()}PositionTaking";

        internal static string GetUsedCreditColumn(int roleId)
        {
            if (roleId.IsMember())
            {
                throw new ArgumentOutOfRangeException(nameof(roleId), "there is no UsedCredit on member level");
            }

            return $"{roleId.DisplayRoleName()}UsedCredit";
        }

        internal static string GetYesterdayCashColumn(int roleId)
            => roleId.IsMember() ? "yesterday_cash" : $"yesterday_{roleId.DisplayRoleName()}Cash";

        internal static string GetCashColumn(int roleId)
            => roleId.IsMember() ? "cash" : $"{roleId.DisplayRoleName()}Cash";

        internal List<ObjectInfo> GetLoginInfo(DataRow cashRow, DataRow customerRow)
        {
            var roleId = Utils.ToInt32(customerRow["roleid"]);
            var lastLoginIp = customerRow.ToString("lastloginip");

            var loginInfos = new List<ObjectInfo>
                {
                    new ObjectInfo { Value = roleId.IsNotMember() ? Utils.ToString(customerRow["site"]) : Utils.ToString(customerRow["membersite"]), Text = "Site" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["createdate"]).ToStringExt(DateTimeFormat), Text = "Created Date" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["lastlogged"]).ToStringExt(DateTimeFormat), Text = "Last Login" },
                    new ObjectInfo { Value = Utils.ToDateTime(cashRow["lastTxn"]).ToStringExt(DateTimeFormat), Text = "Last Txn", Tooltip = "Last Transaction" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["pwdexpiry"]).ToStringExt(DateTimeFormat), Text = "Password" },
                    new ObjectInfo { Value = authorizationHandler.CanViewVNIP(lastLoginIp) ? lastLoginIp : string.Empty, Text = "Last Login IP" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["LastChangedSecurityCodeDate"]).ToStringExt(DateTimeFormat), Text = "Security Code" }
                };

            if (roleId.IsNotMember())
            {
                loginInfos.Add(new ObjectInfo
                {
                    Value = Utils.ToBoolean(customerRow["NicknameLoginOnly"]) ? "Nickname Only" : "Username/Nickname",
                    Text = "Login Options"
                });

                loginInfos.Add(new ObjectInfo
                {
                    Value = Utils.ToDateTime(customerRow["OTPCreateDate"]).ToStringExt(DateTimeFormat),
                    Text = "OTP"
                });
            }

            return loginInfos;
        }

        internal virtual PersonalInfo GetPersonalInfo(DataRow customerRow, DataRow lineRow)
        {
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

            var personalInfo = new PersonalInfo
            {
                CustId = new ObjectInfo { Value = Utils.ToInt32(customerRow["custid"]), Text = "Cust Id" },
                Fax = new ObjectInfo { Value = Utils.ToString(customerRow["fax"]), Text = "Fax" },
                Mobile = new ObjectInfo { Value = Utils.ToString(customerRow["mobilephone"]), Text = "Mobile" },
                Name = new ObjectInfo { Value = Utils.ToString(customerRow["firstname"]) + " " + Utils.ToString(customerRow["lastname"]), Text = "Name" },
                Nickname = new ObjectInfo { Value = Utils.ToString(customerRow["username2"]), Text = "Nickname" },
                Phone = new ObjectInfo { Value = Utils.ToString(customerRow["phone"]), Text = "Phone" },
                RoleId = new ObjectInfo { Value = roleId, Text = "Role Id" },
                UplineRoleId = new ObjectInfo { Value = roleId + 1, Text = "Upline Role Id" },
                CustList = new ObjectInfo { Key = "CustList", Value = roleId.IsSuper() ? "Master List" : roleId.IsMaster() ? "Agent List" : "Member List" },
                Status = new ObjectInfo { Tooltip = !enabled ? status + "disabled" : closed ? status : suspended ? status + "suspended" : string.Empty },
                BackgroundColor = new ObjectInfo { Value = !enabled ? "disabled" : closed ? closedReason : suspended ? "suspended" : string.Empty },
                IsInternal = new ObjectInfo { Value = Utils.ToBoolean(customerRow["isinternal"]) }
            };

            SetLineInfo(personalInfo, lineRow);

            return personalInfo;
        }

        internal static List<ObjectInfo> GetDownlines(DataRow lineRow, int roleId)
        {
            if (roleId.IsSuper())
            {
                return new List<ObjectInfo>
                {
                    new ObjectInfo(lineRow.SuperId().ToString(), lineRow.SuperName(), "Super")
                };
            }

            if (roleId.IsMaster())
            {
                return new List<ObjectInfo>
                {
                    new ObjectInfo(lineRow.SuperId().ToString(), lineRow.SuperName(),  "Super"),
                    new ObjectInfo(lineRow.MasterId().ToString(), lineRow.MasterName(), "Master")
                };
            }

            return new List<ObjectInfo>
            {
                new ObjectInfo(lineRow.SuperId().ToString(), lineRow.SuperName(),  "Super"),
                new ObjectInfo(lineRow.MasterId().ToString(), lineRow.MasterName(),  "Master"),
                new ObjectInfo(lineRow.AgentId().ToString(), lineRow.AgentName(),  "Agent")
            };
        }

        internal abstract void SetLineInfo(PersonalInfo personalInfo, DataRow lineRow);

        internal Setting GetCustomerSetting(int role, DataTable custTable, DataTable productStatusTable, CustomerStatus customerStatus)
        {
            var customerRow = custTable.Rows[0];

            var setting = new Setting
            {
                EnableItems = EnableItems(customerRow, productStatusTable, customerStatus),
                SyncItems = LeftPanelProductIntegration.CreateSyncItems(customerRow, productStatusTable),
                EditItems = EditItems(),
                Details = Details(role, customerStatus),
                MinMaxPts = new List<ObjectInfo>
                {
                    new ObjectInfo { Key = "MinSB", Value = "Sportsbook" },
                    new ObjectInfo { Key = "MinRB", Value = "Racing" }
                }
            };

            foreach (var productSettingParam in LeftPanelProductIntegration.ProductSettingParamList)
            {
                setting = AddProductSetting(setting, productStatusTable, productSettingParam, customerStatus);
            }

            return setting;
        }

        /// <summary>
        /// this method should be overrided by each customer query service
        /// </summary>
        internal virtual void SetCustomSettingByLevel(Setting setting, int userId, DataSet dataSet)
        {
        }

        internal static List<ObjectInfo> EnableItems(
            DataRow customerRow,
            DataTable productStatusTable,
            CustomerStatus customerStatus)
        {
            var enableItems = new List<ObjectInfo>
            {
                ClosedStatus(customerRow, customerStatus),
                SuspendedStatus(customerRow, customerStatus),
                OutrightStatus(customerRow, customerStatus)
            };

            AddCasinoStatus(customerRow, customerStatus, enableItems);
            AddNumberGameStatus(customerRow, customerStatus, enableItems);
            AddLiveCasinoStatus(customerRow, customerStatus, enableItems);
            AddVirtualSportsStatus(productStatusTable, customerStatus, enableItems);
            AddDoubleCommStatus(customerRow, customerStatus, enableItems);
            AddHRFixedOddsStatus(customerRow, customerStatus, enableItems);
            AddESportsStatus(customerRow, customerStatus, enableItems);
            AddColossusBetsStatus(customerRow, customerStatus, enableItems);
            AddGoldDeluxeStatus(customerRow, customerStatus, enableItems);

            return enableItems;
        }

        private static void AddGoldDeluxeStatus(DataRow customerRow, CustomerStatus customerStatus, ICollection<ObjectInfo> enableItems)
        {
            if (customerStatus.HideGoldDeluxe)
            {
                return;
            }

            var goldDeluxeStatus =
                new ObjectInfo("EnableGoldDeluxe", customerRow.EnableGoldDeluxe(), "Gold Deluxe");

            if (customerStatus.RoleId.IsNotSuper()
                && customerStatus.UplineStatus.DisableGoldDeluxe)
            {
                goldDeluxeStatus = DisableStatus(goldDeluxeStatus);
            }

            enableItems.Add(goldDeluxeStatus);
        }

        private static void AddColossusBetsStatus(DataRow customerRow, CustomerStatus customerStatus, ICollection<ObjectInfo> enableItems)
        {
            if (customerStatus.HideColossusBets)
            {
                return;
            }

            var colossusBetsStatus = new ObjectInfo("EnableColossusBets", customerRow.EnableColossusBet(),
                "Colossus Bets",
                "Enable Colossus Bets");

            if (customerStatus.RoleId.IsNotSuper()
                && customerStatus.UplineStatus.DisableColossusBets)
            {
                colossusBetsStatus = DisableStatus(colossusBetsStatus);
            }

            enableItems.Add(colossusBetsStatus);
        }

        private static void AddESportsStatus(DataRow customerRow, CustomerStatus customerStatus, List<ObjectInfo> enableItems)
        {
            if (customerStatus.HideEsports)
            {
                return;
            }

            var eSportsStatus = new ObjectInfo("EnableESports", customerRow.EnableESports(), "E-Sports", "Enable E-Sports");

            if (customerStatus.RoleId.IsNotSuper()
                && customerStatus.UplineStatus.DisableESports)
            {
                eSportsStatus = DisableStatus(eSportsStatus);
            }

            enableItems.Add(eSportsStatus);
        }

        private static void AddHRFixedOddsStatus(DataRow customerRow, CustomerStatus customerStatus, ICollection<ObjectInfo> enableItems)
        {
            var hrFixedOddsStatus = new ObjectInfo(
                "EnableHRFixedOdds",
                customerRow.EnableHorseRacingFixedOdds(),
                "HR Fixed Odds",
                "Enable Horse Racing Fixed Odds");

            if (customerStatus.RoleId.IsNotSuper()
                && customerStatus.UplineStatus.DisableHRFixedOdds)
            {
                hrFixedOddsStatus = DisableStatus(hrFixedOddsStatus);
            }

            enableItems.Add(hrFixedOddsStatus);
        }

        private static void AddDoubleCommStatus(
            DataRow customerRow,
            CustomerStatus customerStatus,
            ICollection<ObjectInfo> enableItems)
        {
            var doubleCommStatus = new ObjectInfo("DoubleComm", customerRow.EnableDoubleComm(), "Double Comm",
                "Enable Double Commission");

            if (customerStatus.RoleId.IsNotSuper()
                && !customerStatus.UplineStatus.AllowDoubleComm)
            {
                doubleCommStatus = DisableStatus(doubleCommStatus);
            }

            enableItems.Add(doubleCommStatus);
        }

        private static void AddVirtualSportsStatus(DataTable productStatusTable, CustomerStatus customerStatus,
            ICollection<ObjectInfo> enableItems)
        {
            if (customerStatus.HideVirtualSports)
            {
                return;
            }

            var virtualSportsStatus = new ObjectInfo("EnableVirtualSports",
                productStatusTable.GetVirtualSportsStatus(), "Virtual Sports");

            if (customerStatus.RoleId.IsNotSuper()
                && customerStatus.UplineStatus.DisableVirtualSports)
            {
                virtualSportsStatus = DisableStatus(virtualSportsStatus);
            }

            enableItems.Add(virtualSportsStatus);
        }

        private static void AddLiveCasinoStatus(
            DataRow customerRow,
            CustomerStatus customerStatus,
            ICollection<ObjectInfo> enableItems)
        {
            if (!customerStatus.SyncLiveCasino)
            {
                return;
            }

            var liveCasinoStatus = new ObjectInfo("EnableLiveCasino", customerRow.EnableLiveCasino(),
                "Enable Live Casino");

            if (customerStatus.RoleId.IsNotSuper()
                && customerStatus.UplineStatus.DisableLiveCasino)
            {
                liveCasinoStatus = DisableStatus(liveCasinoStatus);
            }

            enableItems.Add(liveCasinoStatus);
        }

        private static void AddNumberGameStatus(DataRow customerRow, CustomerStatus customerStatus, ICollection<ObjectInfo> enableItems)
        {
            var numberGameStatus = new ObjectInfo("EnableNumberGame", customerRow.EnableNumberGame(), "Enable Num Game",
                "Enable Number Game");

            if (customerStatus.RoleId.IsNotSuper() && customerStatus.UplineStatus.DisableNumberGame)
            {
                numberGameStatus = DisableStatus(numberGameStatus);
            }

            enableItems.Add(numberGameStatus);
        }

        private static void AddCasinoStatus(DataRow customerRow, CustomerStatus customerStatus, ICollection<ObjectInfo> enableItems)
        {
            if (!customerStatus.SyncCasino)
            {
                return;
            }

            var casinoStatus = new ObjectInfo("EnableCasino", customerRow.EnableCasino(), "Enable Casino");

            if (customerStatus.RoleId.IsNotSuper() && customerStatus.UplineStatus.DisableCasino)
            {
                casinoStatus = DisableStatus(casinoStatus);
            }

            enableItems.Add(casinoStatus);
        }

        private static ObjectInfo OutrightStatus(DataRow customerRow, CustomerStatus customerStatus)
        {
            var outrightStatus = new ObjectInfo("Outright", customerRow.EnableOutright(), "Allow Outright");

            if (customerStatus.RoleId.IsNotSuper() && !customerStatus.UplineStatus.AllowOutright)
            {
                outrightStatus = DisableStatus(outrightStatus);
            }

            return outrightStatus;
        }

        private static ObjectInfo SuspendedStatus(DataRow customerRow, CustomerStatus customerStatus)
        {
            var suspendedStatus = new ObjectInfo("Suspended", customerRow.Suspended(), "Suspened");

            if (customerStatus.RoleId.IsNotSuper() && customerStatus.UplineStatus.Suspended)
            {
                suspendedStatus = DisableStatus(suspendedStatus);
            }

            return suspendedStatus;
        }

        private static ObjectInfo ClosedStatus(DataRow customerRow, CustomerStatus customerStatus)
        {
            var closedStatus = new ObjectInfo("Closed", customerRow.Closed(), "Closed");

            if (customerStatus.RoleId.IsNotSuper() && customerStatus.UplineStatus.Closed)
            {
                closedStatus = DisableStatus(closedStatus);
            }

            return closedStatus;
        }

        internal static List<ObjectInfo> Details(int roleId, CustomerStatus customerStatus)
        {
            var details = new List<ObjectInfo>
            {
                new ObjectInfo ("Info", "Edit Info"),
                new ObjectInfo ("PT", "Edit Position Taking")
            };

            if (roleId.IsNotMember())
            {
                details.Add(new ObjectInfo("AddNew", "Add New"));
                details.Add(new ObjectInfo("Transfer", "Edit Transfer Condition"));
            }

            details.Add(new ObjectInfo("Racing", "Edit Racing"));

            AddCasinoEditLink(customerStatus, details);
            AddLiveCasinoEditLink(customerStatus, details);
            AddVirtualSportsEditLink(customerStatus, details);
            AddKenoEditLink(details);
            AddGoldDeluxeEditLink(customerStatus, details);
            AddColossusBetsEditLink(customerStatus, details);
            AddSportsbook2EditLink(customerStatus, details);
            AddCockfightingEditLink(customerStatus, details);
            AddCricketEditLink(customerStatus, details);

            return details;
        }

        private static void AddCricketEditLink(CustomerStatus customerStatus, ICollection<ObjectInfo> details)
        {
            if (!customerStatus.HideCricket)
            {
                details.Add(new ObjectInfo("Cricket", "Edit Cricket"));
            }
        }

        private static void AddCockfightingEditLink(CustomerStatus customerStatus, ICollection<ObjectInfo> details)
        {
            if (!customerStatus.HideCockfighting)
            {
                details.Add(new ObjectInfo("Cockfighting", "Edit Cockfighting"));
            }
        }

        private static void AddKenoEditLink(List<ObjectInfo> details)
        {
            if (AppSettings.EnableKeno)
            {
                details.Add(new ObjectInfo("Keno", "Edit Keno"));
            }
        }

        private static void AddSportsbook2EditLink(CustomerStatus customerStatus, ICollection<ObjectInfo> details)
        {
            if (!customerStatus.DisableSportsbook2)
            {
                details.Add(new ObjectInfo("Sportsbook2", "Edit Sportsbook 2"));
            }
        }

        private static void AddColossusBetsEditLink(CustomerStatus customerStatus, ICollection<ObjectInfo> details)
        {
            if (!customerStatus.HideColossusBets)
            {
                details.Add(new ObjectInfo("ColossusBets", "Edit Colossus Bets"));
            }
        }

        private static void AddGoldDeluxeEditLink(CustomerStatus customerStatus, ICollection<ObjectInfo> details)
        {
            if (!customerStatus.UplineStatus.HideGoldDeluxe)
            {
                details.Add(new ObjectInfo("GoldDeluxe", "Edit Gold Deluxe"));
            }
        }

        private static void AddLiveCasinoEditLink(CustomerStatus customerStatus, ICollection<ObjectInfo> details)
        {
            if (customerStatus.UplineStatus.SyncLiveCasino
                || customerStatus.RoleId.IsSuper())
            {
                details.Add(new ObjectInfo("LiveCasino", "Edit Live Casino"));
            }
        }

        private static void AddCasinoEditLink(CustomerStatus customerStatus, ICollection<ObjectInfo> details)
        {
            if (customerStatus.UplineStatus.SyncCasino
                || customerStatus.RoleId.IsSuper())
            {
                details.Add(new ObjectInfo("Casino", "Edit Casino"));
            }
        }

        private static void AddVirtualSportsEditLink(CustomerStatus customerStatus, ICollection<ObjectInfo> details)
        {
            var hideVirtualSports = customerStatus.RoleId.IsMember()
                ? customerStatus.UplineStatus.HideVirtualSports
                : customerStatus.HideVirtualSports;

            if (!hideVirtualSports)
            {
                details.Add(new ObjectInfo("VirtualSports", "Edit Virtual Sports"));
            }
        }

        internal static List<ObjectInfo> EditItems()
            => new List<ObjectInfo>
            {
                new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                new ObjectInfo { Key = "EnableKeno", Text = "Enable Keno" },
                new ObjectInfo { Key = "EnableCockfighting", Text = "Enable Cockfighting" }
            };

        internal static Setting AddProductSetting(
            Setting setting,
            DataTable productStatusTable,
            ProductSettingParam settingParams,
            CustomerStatus customerStatus)
        {
            if (!IsProductVisibledToSite(productStatusTable, settingParams.ProductId))
            {
                return setting;
            }

            var roleId = customerStatus.RoleId;
            var hide = customerStatus.ProductStatusDic[settingParams.ProductId].IsHidden;
            var sync = customerStatus.ProductStatusDic[settingParams.ProductId].IsSynced;
            var isSyncDownlines = customerStatus.ProductStatusDic[settingParams.ProductId].IsSyncedDownlines;
            var isCheckSyncDownlines = customerStatus.ProductStatusDic[settingParams.ProductId].CheckSyncedDownlines;
            var hideUpline = customerStatus.RoleId.IsNotSuper()
                ? customerStatus.UplineStatus.ProductStatusDic[settingParams.ProductId].IsHidden
                : hide;
            var isUplineDisabled = customerStatus.RoleId.IsNotSuper()
                                    && customerStatus.UplineStatus.ProductStatusDic[settingParams.ProductId].IsDisabled;
            var syncUpline = customerStatus.RoleId.IsNotSuper()
                ? customerStatus.UplineStatus.ProductStatusDic[settingParams.ProductId].IsSynced
                : sync;

            if (IsHiddenProduct(customerStatus.RoleId, hideUpline, hide))
            {
                return setting;
            }

            if (syncUpline || roleId.IsSuper())
            {
                setting.Details.Add(
                    GetEditSettingLink(settingParams.ProductKey, settingParams.ProductName));
            }

            if (!syncUpline)
            {
                return setting;
            }

            if (sync
                || roleId.IsNotMember()
                || !isCheckSyncDownlines
                || isSyncDownlines)
            {
                AddEditStatusItem(setting, productStatusTable, settingParams, isUplineDisabled);
            }

            return setting;
        }

        private static bool IsHiddenProduct(int roleId, bool hideUpline, bool hide)
            => (roleId.IsNotMember() && hide)
               || (roleId.IsMember() && hideUpline);

        private static void AddEditStatusItem(
            Setting setting,
            DataTable productStatusTable,
            ProductSettingParam settingParams,
            bool isUplineDisabled)
        {
            if (settingParams.StatusType == StatusType.Checkbox)
            {
                var enableItem = CreateEnableCheckboxItem(settingParams.ProductKey,
                    !productStatusTable.IsProductEnabled(settingParams.ProductId), settingParams.ProductName);

                if (isUplineDisabled)
                {
                    enableItem = DisableStatus(enableItem);
                }

                setting.EnableItems.Add(enableItem);
            }
            else
            {
                setting.EditItems.Add(CreateEnableLinkItem(settingParams.ProductKey, settingParams.ProductName));
            }
        }

        private static ObjectInfo DisableStatus(ObjectInfo enableItem)
        {
            enableItem.Disabled = true;
            enableItem.Tooltip = enableItem.Key.Equals("Closed", StringComparison.OrdinalIgnoreCase)
                ? "Upline has been closed"
                : enableItem.Key.Equals("Suspended", StringComparison.OrdinalIgnoreCase)
                    ? "Upline has been suspended"
                    : "Upline has been disabled";

            return enableItem;
        }

        internal static ObjectInfo GetEditSettingLink(string productNameNoSpace, string productName)
            => new ObjectInfo { Key = productNameNoSpace, Value = $"Edit {productName}" };

        internal static bool IsProductVisibledToSite(DataTable productStatusTable, int productId)
            => productStatusTable.Select($"ProductID = {productId}").Length > 0 && AppSettings.IsShowProduct(productId);

        internal static ObjectInfo CreateEnableLinkItem(string productNameNoSpace, string name)
            => new ObjectInfo
            {
                Key = $"Enable{productNameNoSpace}",
                Text = $"Enable {name}"
            };

        internal static ObjectInfo CreateEnableCheckboxItem(
            string productNameNoSpace,
            bool isDisabled,
            string name)
            => new ObjectInfo
            {
                Key = $"Enable{productNameNoSpace}",
                Value = !isDisabled,
                Text = name,
                Tooltip = $"Enable {name}"
            };
    }
}