namespace Fanex.ExternalAccount.Module.LeftPanel.Services
{
    using Core.Authorization;
    using Core.Shared;
    using Extensions;
    using Fanex.ExternalAccount.Core.Context;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using ViewModels;

    public interface CustomerSettingQueryService
    {
        Setting GetSetting(DataSet customerStatusInfoDataset);
    }

    public class CustomerSettingServiceImpl : CustomerSettingQueryService
    {
        private readonly IEnumerable<ProductSettingElement> productSettingElements;
        private readonly Func<DataRow, DataTable, List<ObjectInfo>> hiddenAndSyncItemsCreator;
        private readonly AuthorizationService authorizationService;
        private readonly CurrentSession currentSession;
        private readonly int _roleId;
        protected readonly int _userId;

        public CustomerSettingServiceImpl(int userId, int roleId,
            IEnumerable<ProductSettingElement> productSettingElements = null,
            AuthorizationService authorizationService = null,
            CurrentSession currentSession = null,
            Func<DataRow, DataTable, List<ObjectInfo>> hiddenAndSyncItemsCreator = null)
        {
            _roleId = roleId;
            _userId = userId;
            this.productSettingElements = productSettingElements ?? AccountInfoProductConfiguration.ProductSettingParamList;
            this.hiddenAndSyncItemsCreator = hiddenAndSyncItemsCreator ?? AccountInfoProductConfiguration.CreateSyncAndHiddenItems;
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
            this.currentSession = currentSession ?? new CurrentSession();
        }

        public virtual Setting GetSetting(DataSet customerStatusInfoDataset)
        {
            var nonDCSProductStatusRow = customerStatusInfoDataset.Tables[1].Rows[0];
            var table3rdDCS = customerStatusInfoDataset.Tables[2];
            var setting = new Setting
            (
                EnableItems(customerStatusInfoDataset),
                hiddenAndSyncItemsCreator(nonDCSProductStatusRow, table3rdDCS),
                EnableKenoItem(nonDCSProductStatusRow),
                Details(customerStatusInfoDataset)
            );

            foreach (var productSettingParam in productSettingElements)
            {
                setting = AddProductSetting(setting, table3rdDCS, productSettingParam);
            }

            AddLiveCasinoStatus(table3rdDCS, setting.EditItems);
            AddLiveCasinoLink(table3rdDCS, setting.Details);

            return setting;
        }

        internal List<ObjectInfo> EnableItems(DataSet customerStatusInfoDataset)
        {
            var nonProductStatusRow = customerStatusInfoDataset.Tables[0].Rows[0];
            var nonDCSProductStatusRow = customerStatusInfoDataset.Tables[1].Rows[0];
            var table3rdDCS = customerStatusInfoDataset.Tables[2];
            var enableItems = new List<ObjectInfo>
            {
                ClosedStatus(nonProductStatusRow),
                SuspendedStatus(nonProductStatusRow),
                OutrightStatus(nonProductStatusRow)
            };

            if (!this.currentSession.IsBelongToSpecialGroups)
            {
                AddCasinoStatus(nonDCSProductStatusRow, enableItems);
                AddNumberGameStatus(nonDCSProductStatusRow, enableItems);
                AddVirtualSportsStatus(table3rdDCS, enableItems);
            }

            AddDoubleCommStatus(nonProductStatusRow, enableItems);
            AddESportsStatus(table3rdDCS, enableItems);
            if (!this.currentSession.IsBelongToSpecialGroups)
            {
                AddGoldDeluxeStatus(nonDCSProductStatusRow, enableItems);
            }

            return enableItems;
        }

        private static void AddVirtualSportsStatus(DataTable table3rdDCS, ICollection<ObjectInfo> enableItems)
        {
            if (table3rdDCS.IsProductHidden(ProductConstants.VirtualSports))
            {
                return;
            }

            var virtualSportsStatus = new ObjectInfo("EnableVirtualSports",
               !table3rdDCS.IsProductDisabled(ProductConstants.VirtualSports), "Virtual Sports");

            if (table3rdDCS.IsProductDisabledUpline(ProductConstants.VirtualSports))
            {
                virtualSportsStatus = DisableStatus(virtualSportsStatus);
            }

            enableItems.Add(virtualSportsStatus);
        }

        private static void AddGoldDeluxeStatus(DataRow nonDCSProductStatusRow, ICollection<ObjectInfo> enableItems)
        {
            if (nonDCSProductStatusRow.HideGoldDeluxe() || !ProductSettings.IsShowProduct(ProductConstants.GoldDeluxe))
            {
                return;
            }

            var goldDeluxeStatus =
                new ObjectInfo("EnableGoldDeluxe", nonDCSProductStatusRow.EnableGoldDeluxe(), "Gold Deluxe");

            if (nonDCSProductStatusRow.DisableGoldDeluxeUpline())
            {
                goldDeluxeStatus = DisableStatus(goldDeluxeStatus);
            }

            enableItems.Add(goldDeluxeStatus);
        }

        private void AddESportsStatus(DataTable table3rdDCS, List<ObjectInfo> enableItems)
        {
            if (!authorizationService.IsAuthorized(_userId, eFunctions.ShowEnableEsports)
                || ProductSettings.HideEsports)
            {
                return;
            }

            var eSportsStatus =
                new ObjectInfo("EnableESports", !table3rdDCS.IsProductDisabled(ProductConstants.Sportsbook2), "E-Sports", "Enable E-Sports");

            if (table3rdDCS.IsProductDisabledUpline(ProductConstants.Sportsbook2))
            {
                eSportsStatus = DisableStatus(eSportsStatus);
            }

            enableItems.Add(eSportsStatus);
        }

        private void AddLiveCasinoStatus(DataTable table3rdDCS, List<ObjectInfo> editItems)
        {
            if (!ProductSettings.IsShowLiveCasino)
            {
                return;
            }
            var disable = false;
            var disableUpline = false;
            foreach (var sporttype in table3rdDCS.Select($"ProductID = {ProductConstants.LiveCasino}"))
            {
                disable = disable || sporttype.IsDisabled();
                disableUpline = disableUpline || sporttype.IsDisabledUpline();
            }

            var eSportsStatus =
                new ObjectInfo("EnableLiveCasino", "Enable Live Casino", "Enable Live Casino");

            if (disableUpline)
            {
                eSportsStatus = DisableStatus(eSportsStatus);
            }

            editItems.Add(eSportsStatus);
        }

        private void AddLiveCasinoLink(DataTable table3rdDCS, List<ObjectInfo> editLinks)
        {
            if (!ProductSettings.IsShowLiveCasino)
            {
                return;
            }
            var isHideUpline = false;
            foreach (var sporttype in table3rdDCS.Select($"ProductID = {ProductConstants.LiveCasino}"))
            {
                isHideUpline = isHideUpline || sporttype.IsHiddenUpline();
            }

            var eSportsStatus =
                new ObjectInfo("LiveCasino", "Live Casino");

            if (_roleId.IsSuper() || isHideUpline)
            {
                editLinks.Add(eSportsStatus);
            }            
        }

        private static void AddDoubleCommStatus(DataRow nonProductStatusRow, ICollection<ObjectInfo> enableItems)
        {
            var doubleCommStatus = new ObjectInfo("DoubleComm", nonProductStatusRow.EnableDoubleComm(), "Double Comm",
                "Enable Double Commission");

            if (!nonProductStatusRow.EnableDoubleCommUpline())
            {
                doubleCommStatus = DisableStatus(doubleCommStatus);
            }

            enableItems.Add(doubleCommStatus);
        }

        private static void AddNumberGameStatus(DataRow nonDCSProductStatusRow, ICollection<ObjectInfo> enableItems)
        {
            var numberGameStatus = new ObjectInfo("EnableNumberGame", nonDCSProductStatusRow.EnableNumberGame(), "Enable Num Game",
                "Enable Number Game");

            if (nonDCSProductStatusRow.DisableNumberGameUpline())
            {
                numberGameStatus = DisableStatus(numberGameStatus);
            }

            enableItems.Add(numberGameStatus);
        }

        private static void AddCasinoStatus(DataRow nonDCSProductStatusRow, ICollection<ObjectInfo> enableItems)
        {
            if (!nonDCSProductStatusRow.SyncCasino())
            {
                return;
            }

            var casinoStatus = new ObjectInfo("EnableCasino", nonDCSProductStatusRow.EnableCasino(), "Enable Casino");

            if (nonDCSProductStatusRow.DisableCasinoUpline())
            {
                casinoStatus = DisableStatus(casinoStatus);
            }

            enableItems.Add(casinoStatus);
        }

        private static ObjectInfo OutrightStatus(DataRow nonProductStatusRow)
        {
            var outrightStatus = new ObjectInfo("Outright", nonProductStatusRow.EnableOutright(), "Allow Outright");

            if (nonProductStatusRow.DisableOutrightUpline())
            {
                outrightStatus = DisableStatus(outrightStatus);
            }

            return outrightStatus;
        }

        private static ObjectInfo SuspendedStatus(DataRow nonProductStatusRow)
        {
            var suspendedStatus = new ObjectInfo("Suspended", nonProductStatusRow.Suspended(), "Suspended");

            if (nonProductStatusRow.SuspendedUpline())
            {
                suspendedStatus = DisableStatus(suspendedStatus);
                if (!nonProductStatusRow.IsDirectSuspended())
                {
                    suspendedStatus.Tooltip = "This account was indirectly suspended by upline";
                }
            }

            return suspendedStatus;
        }

        private static ObjectInfo ClosedStatus(DataRow nonProductStatusRow)
        {
            var closedStatus = new ObjectInfo("Closed", nonProductStatusRow.Closed(), "Closed");

            if (nonProductStatusRow.ClosedUpline())
            {
                closedStatus = DisableStatus(closedStatus);
            }

            return closedStatus;
        }

        internal List<ObjectInfo> Details(DataSet customerStatusInfoDataset)
        {
            var nonDCSProductStatusRow = customerStatusInfoDataset.Tables[1].Rows[0];
            var table3rdDCS = customerStatusInfoDataset.Tables[2];
            var details = new List<ObjectInfo>
            {
                new ObjectInfo("Info", "Info"),
                new ObjectInfo("PT", "Position Taking")
            };

            if (_roleId.IsNotMember())
            {
                details.Add(new ObjectInfo("AddNew", "Add New"));
                details.Add(new ObjectInfo("Transfer", "Transfer Condition"));
            }

            AddCasinoEditLink(nonDCSProductStatusRow, details);
            AddVirtualSportsEditLink(table3rdDCS, details);
            AddKenoEditLink(details);
            AddGoldDeluxeEditLink(nonDCSProductStatusRow, details);
            return details;
        }

        private void AddVirtualSportsEditLink(DataTable table3rdDCS, ICollection<ObjectInfo> details)
        {
            if (_roleId.IsMember() ?
                !table3rdDCS.IsProductHiddenUpline(ProductConstants.VirtualSports) :
                !table3rdDCS.IsProductHidden(ProductConstants.VirtualSports))
            {
                details.Add(new ObjectInfo("VirtualSports", "Virtual Sports"));
            }
        }

        private static void AddKenoEditLink(List<ObjectInfo> details)
        {
            if (ProductSettings.IsShowKeno)
            {
                details.Add(new ObjectInfo("Keno", "RNG Keno"));
            }
        }

        private static void AddGoldDeluxeEditLink(DataRow nonDCSProductStatusRow, ICollection<ObjectInfo> details)
        {
            if (!nonDCSProductStatusRow.HideGoldDeluxeUpline())
            {
                details.Add(new ObjectInfo("GoldDeluxe", "Gold Deluxe"));
            }
        }

        private void AddCasinoEditLink(DataRow nonProductStatusRow, ICollection<ObjectInfo> details)
        {
            if (_roleId.IsSuper() || nonProductStatusRow.SyncCasinoUpline())
            {
                details.Add(new ObjectInfo("Casino", "Casino"));
            }
        }

        internal List<ObjectInfo> EnableKenoItem(DataRow nonDCSProductStatusRow)
            => (!nonDCSProductStatusRow.HideKeno() && !this.currentSession.IsBelongToSpecialGroups) ?
            new List<ObjectInfo> { new ObjectInfo { Key = "EnableKeno", Text = "Enable RNG Keno" } } :
            new List<ObjectInfo>();

#pragma warning disable S1541 // Methods and properties should not be too complex
        internal Setting AddProductSetting(
#pragma warning restore S1541 // Methods and properties should not be too complex
            Setting setting,
            DataTable productStatusTable,
            ProductSettingElement productElement)
        {
            if (!IsProductVisibledToSite(productStatusTable, productElement.Id) ||
                productElement.Id == ProductConstants.LiveCasino)
            {
                return setting;
            }

            var hide = productStatusTable.IsProductHidden(productElement.Id);
            var sync = productStatusTable.IsProductSynced(productElement.Id);
            var hideUpline = productStatusTable.IsProductHiddenUpline(productElement.Id);
            var isUplineDisabled = productStatusTable.IsProductDisabledUpline(productElement.Id);
            var syncUpline = productStatusTable.IsProductSyncedUpline(productElement.Id);

            if (IsHiddenProduct(_roleId, hideUpline, hide))
            {
                return setting;
            }

            if (syncUpline || _roleId.IsSuper())
            {
                setting.Details.Add(
                    GetEditSettingLink(productElement.Key, productElement.Name));
            }

#pragma warning disable S1067 // Expressions should not be too complex
            // Ricky: this code i cannot refactor because of the complexity
            if (!syncUpline
                || (!sync && _roleId.IsMember())
                || (!hide && !sync))
#pragma warning restore S1067 // Expressions should not be too complex
            {
                return setting;
            }

            if (!this.currentSession.IsBelongToSpecialGroups)
            {
                AddEditStatusItem(setting, productStatusTable, productElement, isUplineDisabled);
            }

            return setting;
        }

        private static bool IsHiddenProduct(int roleId, bool hideUpline, bool hide)
            => roleId.IsNotMember() && hide
               || roleId.IsMember() && hideUpline;

        private static void AddEditStatusItem(
            Setting setting,
            DataTable productStatusTable,
            ProductSettingElement productSettingElement,
            bool isUplineDisabled)
        {
            if (productSettingElement.StatusType != StatusType.OnlyDetail)
            {
                if (productSettingElement.StatusType == StatusType.Checkbox)
                {
                    var enableItem = CreateEnableCheckboxItem(productSettingElement.Key,
                    productStatusTable.IsProductDisabled(productSettingElement.Id), productSettingElement.Name);

                    if (isUplineDisabled)
                    {
                        enableItem = DisableStatus(enableItem);
                    }

                    setting.EnableItems.Add(enableItem);
                }
                else
                {
                    setting.EditItems.Add(CreateEnableLinkItem(productSettingElement.Key, productSettingElement.Name));
                }
            }
        }

        private static ObjectInfo DisableStatus(ObjectInfo enableItem)
        {
            enableItem.Disabled = true;
            enableItem.Tooltip = enableItem.Key.Equals("Closed", StringComparison.OrdinalIgnoreCase)
                ? "Upline has been closed"
#pragma warning disable S3358 // Ternary operators should not be nested
                : enableItem.Key.Equals("Suspended", StringComparison.OrdinalIgnoreCase)
                    ? "Upline has been suspended"
                    : "Upline has been disabled";
#pragma warning restore S3358 // Ternary operators should not be nested

            return enableItem;
        }

        internal static ObjectInfo GetEditSettingLink(string productNameNoSpace, string productName)
            => new ObjectInfo { Key = productNameNoSpace.FormatProductName(), Value = $"{productName}" };

        internal static bool IsProductVisibledToSite(DataTable productStatusTable, int productId)
            => productStatusTable.Select($"ProductID = {productId}").Length > 0
            && ProductSettings.IsShowProduct(productId);

        internal static ObjectInfo CreateEnableLinkItem(string productNameNoSpace, string name)
            => new ObjectInfo
            {
                Key = $"Enable{productNameNoSpace.FormatProductName()}",
                Text = $"Enable {name}"
            };

        internal static ObjectInfo CreateEnableCheckboxItem(
            string productNameNoSpace,
            bool isDisabled,
            string name)
            => new ObjectInfo
            {
                Key = $"Enable{productNameNoSpace.FormatProductName()}",
                Value = !isDisabled,
                Text = name,
                Tooltip = $"Enable {name}"
            };
    }

    internal static class FormatProductNameExtension
    {
        public static string FormatProductName(this string productName)
            => productName.Replace(".", string.Empty).Replace("-", string.Empty);
    }
}