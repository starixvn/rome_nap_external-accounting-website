namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class SportBookInformationRequest
    {
        public int CustId { get; set; }

        public int RoleId { get; set; }

        public string TabName { get; set; }

        public string Site { get; set; }

        public string SiteLabel { get; set; }

        public int Currency { get; set; }

        public string ApplicationPath { get; set; }

        private object BetSettingTab => new
        {
            name = "BetSetting",
            id = "BetSettingTab",
            url = ApplicationPath + "Customer/GetBetLimitInformation?site=" + Site + "&siteLabel=" + SiteLabel,
            isActive = TabName == "BetSettingTab",
        };

        private object PotentialWinLimitTab => new
        {
            name = "PotentialWinLimit",
            id = "PotentialWinLimitSettingTab",
            isActive = TabName == "PotentialWinLimitSettingTab",
        };

        private object MaxPayoutSettingTab => new
        {
            name = "MaxPayout",
            id = "MaxPayoutSettingTab",
            url = ApplicationPath + "Customer/GetMaxPayoutSettingInformation?site=" + Site + "&siteLabel=" + SiteLabel,
            isActive = TabName == "MaxPayoutSettingTab",
        };

        private object MaxWinSettingTab => new
        {
            name = "MaxWinSettingTabTitle",
            id = "MaxWinSettingTab",
            url = "",
            isActive = TabName == "MaxWinSettingTab",
        };


        private object PTSettingTab => new
        {
            name = "PositionTaking",
            id = "PTSettingTab",
            url = ApplicationPath + "Customer/GetPTSettingInformation?custId=" + CustId + "&site=" + Site,
            isActive = TabName == "PTSettingTab",
        };


        private object CommissionSettingTab => new
        {
            name = "Commission",
            id = "CommissionHintTab",
            url = ApplicationPath + "Customer/GetCommissionHint?custId=" + CustId + "&site=" + Site,
            isActive = TabName == "CommissionHintTab",
        };


        private object ProductCategorySettingTab => new
        {
            name = "ProductCategories",
            id = "ProductCategorySettingTab",
            url = ApplicationPath + "Customer/GetProductCategories?custId=" + CustId + "&site=" + Site + "&currency=" + Currency,
            isActive = TabName == "ProductCategorySettingTab",
        };


        private object WinLossLimitTab => new
        {
            name = "WinLossLimit",
            id = "WinLossLimitTab",
            url = ApplicationPath + "Customer/GetWinLossLimitInformation?custId=" + CustId + "&site=" + Site + "&currency=" + Currency,
            isActive = TabName == "WinLossLimitTab",
        };

        internal string ToJson() => JsonConvert.SerializeObject(new
        {
            tabs = new[]
            {
                BetSettingTab,
                MaxPayoutSettingTab,
                MaxWinSettingTab,
                PTSettingTab,
                CommissionSettingTab,
                ProductCategorySettingTab,
                WinLossLimitTab,
                PotentialWinLimitTab
            },
            roleId = RoleId,
            site = Site,
            siteLabel = SiteLabel
        }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}