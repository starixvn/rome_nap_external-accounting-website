namespace Fanex.ExternalAccount.Module.LeftPanel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Core.Authorization;
    using Core.Shared;
    using Extensions;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Context;
    using Models;
    using ViewModels;

    public interface CustomerSettingQueryService
    {
        Setting GetSetting(DataSet customerStatusInfoDataset);
    }

    public class CustomerSettingServiceImpl : CustomerSettingQueryService
    {
        private readonly IEnumerable<ProductSettingElement> productSettingElements;
        private readonly Func<DataTable, List<ObjectInfo>> hiddenAndSyncItemsCreator;
        private readonly AuthorizationService authorizationService;
        private readonly CurrentSession currentSession;
        private readonly int _roleId;
        protected readonly int _userId;

        public CustomerSettingServiceImpl(int userId, int roleId,
            IEnumerable<ProductSettingElement> productSettingElements = null,
            AuthorizationService authorizationService = null,
            CurrentSession currentSession = null,
            Func<DataTable, List<ObjectInfo>> hiddenAndSyncItemsCreator = null)
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
            var table3rdDCS = customerStatusInfoDataset.Tables[1];
            var setting = new Setting
            (
                EnableItems(customerStatusInfoDataset),
                hiddenAndSyncItemsCreator(table3rdDCS),
                new List<ObjectInfo>(),
                Details(customerStatusInfoDataset)
            );

            if (!currentSession.IsBelongToSpecialGroups)
            {
                AddProductGroupStatus(
                    table3rdDCS,
                    setting.EditItems,
                    setting.EnableItems,
                    ProductGroupConstants.LiveAndRNGCasino,
                    new ObjectInfo("EnableLiveAndRNGCasino", "LiveAndRNGCasino", "Live & RNG Casino"),
                    true);

                AddProductGroupStatus(
                   table3rdDCS,
                   setting.EditItems,
                   setting.EnableItems,
                   ProductGroupConstants.LiveAndRNGCasino,
                   new ObjectInfo("EnableRNGGames", "RNGGames", "RNG Games"),
                   true);
            }

            foreach (var productSettingParam in productSettingElements)
            {
                setting = AddProductSetting(setting, table3rdDCS, productSettingParam);
            }

            return setting;
        }

        internal List<ObjectInfo> EnableItems(DataSet customerStatusInfoDataset)
        {
            var nonProductStatusRow = customerStatusInfoDataset.Tables[0].Rows[0];
            var enableItems = new List<ObjectInfo>
            {
                ClosedStatus(nonProductStatusRow),
                LockedAccess(nonProductStatusRow)
            };

            if (_roleId.IsNotMember()
                && authorizationService.IsAuthorized(currentSession.UserId, eFunctions.ProtectCustomer))
            {
                enableItems.Add(ProtectedStatus(nonProductStatusRow));
            }

            enableItems.Add(SuspendedStatus(nonProductStatusRow));
            enableItems.Add(OutrightStatus(nonProductStatusRow));

            AddDoubleCommStatus(nonProductStatusRow, enableItems);

            return enableItems;
        }

        private static void AddProductGroupStatus(DataTable table3rdDCS, List<ObjectInfo> editItems, List<ObjectInfo> enableItems, int productGroupId, ObjectInfo productStatus, bool isLink)
        {
            var rows = table3rdDCS.Select($"GroupID = {productGroupId} AND isHidden = 0");
            if (rows != null && rows.Length > 0)
            {
                if (isLink)
                {
                    editItems.Add(productStatus);
                }
                else
                {
                    enableItems.Add(productStatus);
                }
            }
        }

        private static void AddProductGroupLink(DataTable table3rdDCS, List<ObjectInfo> editLinks, int productGroupId, ObjectInfo productSetting)
        {
            var rows = table3rdDCS.Select($"GroupID = {productGroupId} AND isHidden = 0");
            if (rows != null && rows.Length > 0)
            {
                editLinks.Add(productSetting);
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
                if (!nonProductStatusRow.IsDirectClosed())
                {
                    closedStatus.Tooltip = "This account is indirectly closed by upline";
                }
            }

            return closedStatus;
        }

        private static ObjectInfo LockedAccess(DataRow nonProductStatusRow)
        {
            var locked = nonProductStatusRow.Locked();
            var isLocked = locked != 0;

            var lockedAccess = new ObjectInfo($"UnlockAccess{locked}", isLocked, "Locked Access")
            {
                Disabled = !isLocked
            };

            return lockedAccess;
        }

        private ObjectInfo ProtectedStatus(DataRow nonProductStatusRow)
        {
            var protectedStatus = new ObjectInfo("ProtectedStatus", nonProductStatusRow.IsProtectedMode(), "Protected")
            {
                Disabled = !authorizationService.IsAuthorized(
                currentSession.UserId,
                eFunctions.ProtectCustomer,
                AuthenticateAction.AuthenticationType.Host)
            };

            return protectedStatus;
        }

        internal List<ObjectInfo> Details(DataSet customerStatusInfoDataset)
        {
            var table3rdDCS = customerStatusInfoDataset.Tables[1];
            var details = new List<ObjectInfo>
            {
                new ObjectInfo("Info", "Info"),
                new ObjectInfo("PT", "Sportsbook")
            };

            if (_roleId.IsNotMember())
            {
                details.Add(new ObjectInfo("AddNew", "Add New"));
                details.Add(new ObjectInfo("Transfer", "Transfer Condition"));
            }

            AddProductGroupLink(table3rdDCS, details, ProductGroupConstants.LiveAndRNGCasino, new ObjectInfo("LiveAndRNGCasino", "Live & RNG Casino"));
            AddProductGroupLink(table3rdDCS, details, ProductGroupConstants.RNGGames, new ObjectInfo("RNGGames", "RNG Games"));

            return details;
        }

#pragma warning disable S1541 // Methods and properties should not be too complex

        internal Setting AddProductSetting(
#pragma warning restore S1541 // Methods and properties should not be too complex
            Setting setting,
            DataTable productStatusTable,
            ProductSettingElement productElement)
        {
            if (!IsProductVisibledToSite(productStatusTable, productElement.Id) ||
                productElement.Id == ProductConstants.RNGLottery)
            {
                return setting;
            }

            var hide = productStatusTable.IsProductHidden(productElement.Id);
            var hideUpline = productStatusTable.IsProductHiddenUpline(productElement.Id);
            var isUplineDisabled = productStatusTable.IsProductDisabledUpline(productElement.Id);
           

            if (IsHiddenProduct(_roleId, hideUpline, hide))
            {
                return setting;
            }

            if (_roleId.IsSuper()
                && productElement.Id != ProductConstants.SabaVirtualSports
                && productElement.Id != ProductConstants.Cashout)
            {
                setting.Details.Add(
                    GetEditSettingLink(productElement.Key, productElement.Name));
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