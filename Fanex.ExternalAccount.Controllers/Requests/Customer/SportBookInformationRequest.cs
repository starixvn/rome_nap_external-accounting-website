namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class SportBookInformationRequest
    {
        public int CustId { get; set; }

        public int RoleId { get; set; }

        public int UplineRoleId { get; set; }

        public int TabIndex { get; set; }

        public string Site { get; set; }
        
        public object BetSettingTab
        {
            get => new
            {
                name = "Bet Setting",
                id = "BetSetting",
                napUrl = "",
                index = 0,
                isActive = TabIndex == 0,
            };
        }

        public object MaxPayoutSettingTab
        {
            get => new
            {
                name = "Max Payout",
                id = "MaxPayoutSetting",
                napUrl = "/Customer/GetMaxPayoutSettingInformation?site=" + Site + "&isSpecialPlatform=false",
                index = 1,
                isActive = TabIndex == 1,
            };
        }

        public object PTSettingTab
        {
            get => new
            {
                name = "Position Taking",
                id = "PTSetting",
                napUrl = "/Customer/GetPTSettingInformation?custId=" + CustId,
                index = 2,
                isActive = TabIndex == 2,
            };
        }

        public object CommissionSettingTab
        {
            get => new
            {
                name = "Commission",
                id = "Commission",
                napUrl = "/Customer/GetCommissionHint?custId=" + CustId,
                index = 3,
                isActive = TabIndex == 3,
            };
        }

        public object ProductCategorySettingTab
        {
            get => new
            {
                name = "Product Categories",
                id = "ProductCategorySetting",
                napUrl = "/Customer/GetProductCategories?custId=" + CustId,
                index = 4,
                isActive = TabIndex == 4,
            };
        }

        internal string ToJson() => JsonConvert.SerializeObject(new
        {
            tabs = new[]
            {
                BetSettingTab,
                MaxPayoutSettingTab,
                PTSettingTab,
                CommissionSettingTab,
                ProductCategorySettingTab,
            },
            is789y = false,
            roleId = RoleId,
            site = Site
        }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}