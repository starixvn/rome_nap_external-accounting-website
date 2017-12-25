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

            foreach (var uplineProductStatus in product.UplineStatus.ProductStatusDic.Values)
            {
                if (uplineProductStatus.IsDisabled)
                {
                    model.DisableStatusSetting($"Enable{uplineProductStatus.ProductKey}");
                }
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