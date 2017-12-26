namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System;
    using ViewModels;

    public static class DisableSettingExtensions
    {
        public static CustomerModel DisableStatusSettingFollowUplineConstrain(this CustomerModel model, CustomerStatus product)
        {
            DisableNonProductStatusSettingFollowUpline(model, product);

            DisableProductStatusSettingFollowUpline(model, product);

            return model;
        }

        private static void DisableNonProductStatusSettingFollowUpline(CustomerModel model, CustomerStatus product)
        {
            if (!product.UplineStatus.AllowDoubleComm)
            {
                model.DisableStatusSetting("DoubleComm");
            }

            if (product.UplineStatus.Closed)
            {
                model.DisableStatusSetting("Closed");
            }

            if (product.UplineStatus.Suspended)
            {
                model.DisableStatusSetting("Suspended");
            }

            if (!product.UplineStatus.AllowOutright)
            {
                model.DisableStatusSetting("Outright");
            }
        }

        private static void DisableProductStatusSettingFollowUpline(CustomerModel model, CustomerStatus product)
        {
            if (product.UplineStatus.DisableNumberGame)
            {
                model.DisableStatusSetting("EnableNumberGame");
            }

            if (product.UplineStatus.DisableVirtualSports)
            {
                model.DisableStatusSetting("EnableVirtualSports");
            }

            if (product.UplineStatus.DisableESports)
            {
                model.DisableStatusSetting("EnableESports");
            }

            if (product.UplineStatus.DisableHRFixedOdds)
            {
                model.DisableStatusSetting("EnableHRFixedOdds");
            }

            if (product.UplineStatus.DisableCasino)
            {
                model.DisableStatusSetting("EnableCasino");
            }

            if (product.UplineStatus.DisableLiveCasino)
            {
                model.DisableStatusSetting("EnableLiveCasino");
            }

            if (product.UplineStatus.DisableGoldDeluxe)
            {
                model.DisableStatusSetting("EnableGoldDeluxe");
            }

            if (product.UplineStatus.DisableColossusBets)
            {
                model.DisableStatusSetting("EnableColossusBets");
            }

            if (product.UplineStatus.DisableAllbet)
            {
                model.DisableStatusSetting("EnableAllbet");
            }

            if (product.UplineStatus.DisableVoidbridge)
            {
                model.DisableStatusSetting("EnableMacauGames");
            }

            if (product.UplineStatus.DisableNG2)
            {
                model.DisableStatusSetting("EnableNG2");
            }

            if (product.UplineStatus.DisableFishingWorld)
            {
                model.DisableStatusSetting("EnableFishingWorld");
            }

            if (product.UplineStatus.DisableLottery)
            {
                model.DisableStatusSetting("EnableLottery");
            }

            if (product.UplineStatus.DisableLucky3)
            {
                model.DisableStatusSetting("EnableLucky3");
            }

            if (product.UplineStatus.DisableBullfighting)
            {
                model.DisableStatusSetting("EnableBullfighting");
            }
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
    }
}