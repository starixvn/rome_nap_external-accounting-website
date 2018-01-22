namespace Fanex.ExternalAccount.Module.LeftPanel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Core.Shared;
    using Extensions;
    using Models;
    using ViewModels;

    public interface SettingService
    {
        Setting GetSetting(int actorId, string custName, int uplineId, string groupName,
            DataSet customerInfoDataset);
    }

    public class SettingServiceImpl : SettingService
    {
        private readonly CustomerStatusService customerStatusService;

        public SettingServiceImpl(CustomerStatusService customerStatusService = null)
        {
            this.customerStatusService = customerStatusService ?? new CustomerStatusServiceImpl();
        }

        public virtual Setting GetSetting(int actorId, string custName, int uplineId, string groupName,
            DataSet customerInfoDataset)
        {
            var custInfoTable = customerInfoDataset.Tables[0];
            var productStatusTable = customerInfoDataset.Tables[4];
            var roleId = custInfoTable.Rows[0].RoleId();

            var customerStatus = customerStatusService.GetCustomerStatus(
                actorId,
                groupName,
                custName,
                uplineId);

            return GetCustomerSetting(roleId, custInfoTable, productStatusTable, customerStatus);
        }

        internal Setting GetCustomerSetting(int role, DataTable custTable, DataTable productStatusTable,
            CustomerStatus customerStatus)
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
                    new ObjectInfo {Key = "MinSB", Value = "Sportsbook"},
                    new ObjectInfo {Key = "MinRB", Value = "Racing"}
                }
            };

            foreach (var productSettingParam in LeftPanelProductIntegration.ProductSettingParamList)
            {
                setting = AddProductSetting(setting, productStatusTable, productSettingParam, customerStatus);
            }

            return setting;
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

        private static void AddGoldDeluxeStatus(DataRow customerRow, CustomerStatus customerStatus,
            ICollection<ObjectInfo> enableItems)
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

        private static void AddColossusBetsStatus(DataRow customerRow, CustomerStatus customerStatus,
            ICollection<ObjectInfo> enableItems)
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

        private static void AddESportsStatus(DataRow customerRow, CustomerStatus customerStatus,
            List<ObjectInfo> enableItems)
        {
            if (customerStatus.HideEsports)
            {
                return;
            }

            var eSportsStatus =
                new ObjectInfo("EnableESports", customerRow.EnableESports(), "E-Sports", "Enable E-Sports");

            if (customerStatus.RoleId.IsNotSuper()
                && customerStatus.UplineStatus.DisableESports)
            {
                eSportsStatus = DisableStatus(eSportsStatus);
            }

            enableItems.Add(eSportsStatus);
        }

        private static void AddHRFixedOddsStatus(DataRow customerRow, CustomerStatus customerStatus,
            ICollection<ObjectInfo> enableItems)
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

        private static void AddNumberGameStatus(DataRow customerRow, CustomerStatus customerStatus,
            ICollection<ObjectInfo> enableItems)
        {
            var numberGameStatus = new ObjectInfo("EnableNumberGame", customerRow.EnableNumberGame(), "Enable Num Game",
                "Enable Number Game");

            if (customerStatus.RoleId.IsNotSuper() && customerStatus.UplineStatus.DisableNumberGame)
            {
                numberGameStatus = DisableStatus(numberGameStatus);
            }

            enableItems.Add(numberGameStatus);
        }

        private static void AddCasinoStatus(DataRow customerRow, CustomerStatus customerStatus,
            ICollection<ObjectInfo> enableItems)
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
                new ObjectInfo("Info", "Edit Info"),
                new ObjectInfo("PT", "Edit Position Taking")
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
            if (ProductSettings.IsShowKeno)
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
                new ObjectInfo {Key = "EnableRacing", Text = "Enable Racing"},
                new ObjectInfo {Key = "EnableKeno", Text = "Enable Keno"},
                new ObjectInfo {Key = "EnableCockfighting", Text = "Enable Cockfighting"}
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
            => roleId.IsNotMember() && hide
               || roleId.IsMember() && hideUpline;

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
            => productStatusTable.Select($"ProductID = {productId}").Length > 0
            && ProductSettings.IsShowProduct(productId);

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