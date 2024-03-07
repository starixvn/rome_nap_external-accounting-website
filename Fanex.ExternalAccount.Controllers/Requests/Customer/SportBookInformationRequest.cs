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

        public int Currency { get; set; }

        private object BetSettingTab => new
        {
            name = "BetSetting",
            id = "BetSettingTab",
            napUrl = "",
            isActive = TabIndex == 0,
        };


        private object MaxPayoutSettingTab => new
        {
            name = "MaxPayout",
            id = "MaxPayoutSettingTab",
            napUrl = "/Customer/GetMaxPayoutSettingInformation?site=" + Site + "&isSpecialPlatform=false",
            index = 1,
            isActive = TabIndex == 1,
        };


        private object PTSettingTab => new
        {
            name = "PositionTaking",
            id = "PTSettingTab",
            napUrl = "/Customer/GetPTSettingInformation?custId=" + CustId + "&site=" + Site,
            isActive = TabIndex == 2,
        };


        private object CommissionSettingTab => new
        {
            name = "Commission",
            id = "CommissionHintTab",
            napUrl = "/Customer/GetCommissionHint?custId=" + CustId + "&site=" + Site,
            isActive = TabIndex == 3,
        };


        private object ProductCategorySettingTab => new
        {
            name = "ProductCategories",
            id = "ProductCategorySettingTab",
            napUrl = "/Customer/GetProductCategories?custId=" + CustId + "&site=" + Site + "&currency=" + Currency,
            isActive = TabIndex == 4,
        };


        private object WinLossLimitTab => new
        {
            name = "WinLossLimit",
            id = "WinLossLimitTab",
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
                WinLossLimitTab
            },
            is789y = false,
            roleId = RoleId,
            site = Site
        }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}