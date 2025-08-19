namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class SportBookInformationRequest
    {
        public int CustId { get; set; }

        public int RoleId { get; set; }

        public int TabIndex { get; set; }

        public string Site { get; set; }

        public string SiteLabel { get; set; }

        public int Currency { get; set; }

        public string ApplicationPath { get; set; }

        private object BetSettingTab => new
        {
            name = "BetSetting",
            id = "BetSettingTab",
            url = ApplicationPath + "Customer/GetBetLimitInformation?site=" + Site + "&siteLabel=" + SiteLabel,
            isActive = TabIndex == 0,
        };

        private object PotentialWinLimitTab => new
        {
            name = "PotentialWinLimit",
            id = "PotentialWinLimitSettingTab",
            isActive = TabIndex == 6,
        };

        private object MaxPayoutSettingTab => new
        {
            name = "MaxPayout",
            id = "MaxPayoutSettingTab",
            url = ApplicationPath + "Customer/GetMaxPayoutSettingInformation?site=" + Site + "&siteLabel=" + SiteLabel,
            isActive = TabIndex == 1,
        };


        private object PTSettingTab => new
        {
            name = "PositionTaking",
            id = "PTSettingTab",
            url = ApplicationPath + "Customer/GetPTSettingInformation?custId=" + CustId + "&site=" + Site,
            isActive = TabIndex == 2,
        };


        private object CommissionSettingTab => new
        {
            name = "Commission",
            id = "CommissionHintTab",
            url = ApplicationPath + "Customer/GetCommissionHint?custId=" + CustId + "&site=" + Site,
            isActive = TabIndex == 3,
        };


        private object ProductCategorySettingTab => new
        {
            name = "ProductCategories",
            id = "ProductCategorySettingTab",
            url = ApplicationPath + "Customer/GetProductCategories?custId=" + CustId + "&site=" + Site + "&currency=" + Currency,
            isActive = TabIndex == 4,
        };


        private object WinLossLimitTab => new
        {
            name = "WinLossLimit",
            id = "WinLossLimitTab",
            url = ApplicationPath + "Customer/GetWinLossLimitInformation?custId=" + CustId + "&site=" + Site + "&currency=" + Currency,
            isActive = TabIndex == 5,
        };

        internal string ToJson() => JsonConvert.SerializeObject(new
        {
            tabs = new[]
            {
                BetSettingTab,
                MaxPayoutSettingTab,
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