namespace Fanex.ExternalAccount.Core.Shared
{
    using System.Configuration;
    using CustomerSetting.Module.Keno.KenoSetting.Configurations;
    using Utilities;

    public static class AppSettings
    {
        public static bool IsShowGoldDeluxeSettingForCredit => ConfigurationManager.AppSettings["IsShowGoldDeluxeSettingForCredit"] == "1";

        public static bool EnableKeno => ConfigurationManager.AppSettings["EnableKeno"] == "true";

        public static bool ShowKenoSetting => KenoSection.Instance.ShowSetting;

        public static bool IsShowColossusBets => ConfigurationManager.AppSettings["IsShowColossusBets"] == "1";

        public static bool IsShowSportsbook2 => ConfigurationManager.AppSettings["IsShowSportsbook2"] == "1";

        public static bool IsShowCockfighting => ConfigurationManager.AppSettings["IsShowCockfighting"] == "1";

        public static bool IsShowCricket => ConfigurationManager.AppSettings["IsShowCricket"] == "1";

        public static bool IsShowAllbet => ConfigurationManager.AppSettings["IsShowAllbet"] == "1";

        public static bool IsShowMacauGames => ConfigurationManager.AppSettings["IsShowMacauGames"] == "1";

        public static bool IsShowNG2 => ConfigurationManager.AppSettings["IsShowNG2"] == "1";

        public static bool IsShowFishingWorld => ConfigurationManager.AppSettings["IsShowFishingWorld"] == "1";

        public static bool IsShowLottery => ConfigurationManager.AppSettings["IsShowLottery"] == "1";

        public static bool IsShowLucky3 => ConfigurationManager.AppSettings["IsShowLucky3"] == "1";

        public static bool IsShowVoidbridge => ConfigurationManager.AppSettings["IsShowVoidbridge"] == "1";

        public static bool HideEsports => Utils.ToBoolean(ConfigurationManager.AppSettings["HideEsports"]);

        public static bool IsShowBullfighting => Utils.ToBoolean(ConfigurationManager.AppSettings["IsShowBullfighting"]);

        public static bool IsShowProduct(string productName)
            => Utils.ToBoolean(ConfigurationManager.AppSettings["IsShow" + productName.Replace(" ", string.Empty)]);
    }
}