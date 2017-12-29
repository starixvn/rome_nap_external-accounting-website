namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System;
    using System.Configuration;
    using Core.Shared;
    using CustomerSetting.Module.Keno.KenoSetting.Configurations;
    using Utilities;
    using ViewModels;

    public static class RemovingSettingExtensions
    {
        public static CustomerModel RemoveProduct(
            this CustomerModel model,
            int roleId,
            string productName,
            bool isRemoveProductSetting)
        {
            model.RemoveProductStatusSetting($"Enable{productName}");

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

        public static CustomerModel RemoveProductSetting(this CustomerModel model, string key)
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

        public static CustomerModel RemoveCrossLevelProductSetting(this CustomerModel model, string key)
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

        public static CustomerModel RemoveProductStatusSetting(this CustomerModel model, string key)
        {
            foreach (var enableItem in model.Setting.EnableItems)
            {
                if (enableItem.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
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

        public static void RemoveProductSetting(this CustomerModel model, CustomerStatus custStatus)
        {
            RemoveCasinoSetting(model, custStatus);
            RemoveLiveCasinoSetting(model, custStatus);
            RemoveVirtualSportSetting(model, custStatus);
            RemoveESportsSetting(model, custStatus);
            RemoveGoldDeluxeSetting(model, custStatus);
            RemoveColossusBetsSetting(model, custStatus);
            RemoveSportsbook2Setting(model, custStatus);
            RemoveCockfightingSetting(model, custStatus);
            RemoveCricketSetting(model, custStatus);

            foreach (var statusItem in custStatus.ProductStatusDic)
            {
                var productStatus = statusItem.Value;
                model.RemoveProduct(
                            custStatus.RoleId,
                            custStatus.RoleId.IsNotSuper()
                                ? custStatus.UplineStatus.ProductStatusDic[statusItem.Key].IsHidden
                                : productStatus.IsHidden,
                            custStatus.RoleId.IsNotSuper()
                                ? custStatus.UplineStatus.ProductStatusDic[statusItem.Key].IsSynced
                                : productStatus.IsSynced,
                            productStatus.IsHidden,
                            productStatus.IsSynced,
                            productStatus.IsSyncedDownlines,
                            productStatus.ProductKey,
                            productStatus.CheckSyncedDownlines);
            }
        }

        private static CustomerModel RemoveProduct(
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
            if (IsHideProduct(roleId, hideUpline, hide))
            {
                return model.RemoveProduct(roleId, productName, true);
            }

            if (!syncUpline)
            {
                if (roleId.IsNotSuper()
                    || (roleId.IsSuper() && hideUpline))
                {
                    model.RemoveProductSetting(productName);
                }

                return model.RemoveProduct(roleId, productName, false);
            }

            if (IsSyncedProduct(roleId, hide, sync, syncDownline, isCheckSyncDownlineField))
            {
                return model.RemoveProduct(roleId, productName, false);
            }

            return model;
        }

        private static bool IsSyncedProduct(int roleId, bool hide, bool sync, bool syncDownline, bool isCheckSyncDownlineField)
            => (!sync && roleId.IsMember())
                            || (!hide && !sync)
                            || (isCheckSyncDownlineField && !syncDownline);

        private static bool IsHideProduct(int roleId, bool hideUpline, bool hide)
            => (roleId.IsNotMember() && hide)
                            || (roleId.IsMember() && hideUpline);

        private static void RemoveCricketSetting(CustomerModel model, CustomerStatus product)
        {
            if (product.HideCricket)
            {
                model.RemoveProductSetting("Cricket")
                    .RemoveCrossLevelProductSetting("Cricket");
            }
        }

        private static void RemoveCockfightingSetting(CustomerModel model, CustomerStatus product)
        {
            if (product.HideCockfighting)
            {
                model.RemoveProductStatusSetting("EnableCockfighting")
                    .RemoveProductSetting("Cockfighting")
                    .RemoveCrossLevelProductSetting("Cockfighting");
            }
        }

        private static void RemoveSportsbook2Setting(CustomerModel model, CustomerStatus product)
        {
            if (product.HideSportsbook2)
            {
                model.RemoveProductSetting("Sportsbook2")
                    .RemoveCrossLevelProductSetting("Sportsbook2");
            }
        }

        private static void RemoveColossusBetsSetting(CustomerModel model, CustomerStatus product)
        {
            if (product.HideColossusBets)
            {
                model.RemoveProductStatusSetting("EnableColossusBets")
                    .RemoveProductSetting("ColossusBets");
            }
        }

        private static void RemoveGoldDeluxeSetting(CustomerModel model, CustomerStatus product)
        {
            if (product.HideGoldDeluxe)
            {
                model.RemoveProductStatusSetting("EnableGoldDeluxe");
            }

            if (product.UplineStatus.HideGoldDeluxe)
            {
                model.RemoveProductSetting("GoldDeluxe")
                    .RemoveCrossLevelProductSetting("GoldDeluxe");
            }
        }

        private static void RemoveESportsSetting(CustomerModel model, CustomerStatus product)
        {
            if (product.HideEsports)
            {
                model.RemoveProductStatusSetting("EnableESports")
                    .RemoveProductSetting("ESports")
                    .RemoveCrossLevelProductSetting("ESports");
            }
        }

        private static void RemoveVirtualSportSetting(CustomerModel model, CustomerStatus product)
        {
            if (product.HideVirtualSports)
            {
                model.RemoveProductStatusSetting("EnableVirtualSports");

                bool hideVirtualSports = product.RoleId.IsMember()?
                    product.UplineStatus.HideVirtualSports :
                    product.HideVirtualSports;

                if (hideVirtualSports)
                {
                    model.RemoveProductSetting("VirtualSports")
                        .RemoveCrossLevelProductSetting("VirtualSports");
                }
            }
        }

        private static void RemoveLiveCasinoSetting(CustomerModel model, CustomerStatus product)
        {
            if (!product.SyncLiveCasino)
            {
                model.RemoveProductStatusSetting("EnableLiveCasino");

                if (!product.UplineStatus.SyncLiveCasino)
                {
                    model.RemoveProductSetting("LiveCasino")
                        .RemoveCrossLevelProductSetting("LiveCasino");
                }
            }
        }

        private static void RemoveCasinoSetting(CustomerModel model, CustomerStatus product)
        {
            if (!product.SyncCasino)
            {
                model.RemoveProductStatusSetting("EnableCasino");

                if (!product.UplineStatus.SyncCasino)
                {
                    model.RemoveProductSetting("Casino")
                        .RemoveCrossLevelProductSetting("Casino");
                }
            }
        }

        public static CustomerModel RemoveProductNotVisibleToSite(this CustomerModel model)
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
    }
}