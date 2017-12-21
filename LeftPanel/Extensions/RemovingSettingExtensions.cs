namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System;
    using System.Configuration;
    using CustomerSetting.Module.Keno.KenoSetting.Configurations;
    using Shared;
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

        public static CustomerModel RemoveProductSetting(this CustomerModel model, CustomerStatus product)
        {
            RemoveCasinoSetting(model, product);
            RemoveLiveCasinoSetting(model, product);
            RemoveVirtualSportSetting(model, product);
            RemoveESportsSetting(model, product);
            RemoveGoldDeluxeSetting(model, product);
            RemoveColossusBetsSetting(model, product);
            RemoveSportsbook2Setting(model, product);
            RemoveCockfightingSetting(model, product);
            RemoveCricketSetting(model, product);
            RemoveAllbetSetting(model, product);
            RemoveMacauGamesSetting(model, product);
            RemoveNG2Setting(model, product);
            RemoveFishingWorldSetting(model, product);
            RemoveLotterySetting(model, product);
            RemoveLucky3Setting(model, product);

            return model;
        }

        private static void RemoveLucky3Setting(CustomerModel model, CustomerStatus product)
         => model.RemoveEnableAndEditProductDCS(
                            product.RoleId,
                            product.RoleId.IsNotSuper() ? product.UplineStatus.HideLucky3 : product.HideLucky3,
                            product.RoleId.IsNotSuper() ? product.UplineStatus.SyncLucky3 : product.SyncLucky3,
                            product.HideLucky3,
                            product.SyncLucky3,
                            product.SyncLucky3Downlines,
                            "Lucky3");

        private static void RemoveLotterySetting(CustomerModel model, CustomerStatus product)
        => model.RemoveEnableAndEditProductDCS(
                            product.RoleId,
                            product.RoleId.IsNotSuper() ? product.UplineStatus.HideLottery : product.HideLottery,
                            product.RoleId.IsNotSuper() ? product.UplineStatus.SyncLottery : product.SyncLottery,
                            product.HideLottery,
                            product.SyncLottery,
                            product.SyncLotteryDownlines,
                            "Lottery");

        private static void RemoveFishingWorldSetting(CustomerModel model, CustomerStatus product)
        => model.RemoveEnableAndEditProductDCS(
                            product.RoleId,
                            product.RoleId.IsNotSuper() ? product.UplineStatus.HideFishingWorld : product.HideFishingWorld,
                            product.RoleId.IsNotSuper() ? product.UplineStatus.SyncFishingWorld : product.SyncFishingWorld,
                            product.HideFishingWorld,
                            product.SyncFishingWorld,
                            product.SyncFishingWorldDownlines,
                            "FishingWorld");

        private static void RemoveNG2Setting(CustomerModel model, CustomerStatus product)
        => model.RemoveEnableAndEditProductDCS(
                            product.RoleId,
                            product.RoleId.IsNotSuper() ? product.UplineStatus.HideNG2 : product.HideNG2,
                            product.RoleId.IsNotSuper() ? product.UplineStatus.SyncNG2 : product.SyncNG2,
                            product.HideNG2,
                            product.SyncNG2,
                            true,
                            "NG2");

        private static void RemoveMacauGamesSetting(CustomerModel model, CustomerStatus product)
        => model.RemoveEnableAndEditProductDCS(
                            product.RoleId,
                            product.RoleId.IsNotSuper() ? product.UplineStatus.HideVoidbridge : product.HideVoidbridge,
                            product.RoleId.IsNotSuper() ? product.UplineStatus.SyncVoidbridge : product.SyncVoidbridge,
                            product.HideVoidbridge,
                            product.SyncVoidbridge,
                            product.SyncVoidbridgeDownlines,
                            "MacauGames",
                            false);

        private static void RemoveAllbetSetting(CustomerModel model, CustomerStatus product)
        => model.RemoveEnableAndEditProductDCS(
                            product.RoleId,
                            product.RoleId.IsNotSuper() ? product.UplineStatus.HideAllbet : product.HideAllbet,
                            product.RoleId.IsNotSuper() ? product.UplineStatus.SyncAllbet : product.SyncAllbet,
                            product.HideAllbet,
                            product.SyncAllbet,
                            product.SyncAllbetDownlines,
                            "Allbet",
                            false);

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

            if ((!sync && roleId.IsMember())
                || (!hide && !sync)
                || (isCheckSyncDownlineField && !syncDownline))
            {
                return model.RemoveProduct(roleId, productName, false);
            }

            return model;
        }


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

                bool hideVirtualSports = product.RoleId == 1 ?
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
