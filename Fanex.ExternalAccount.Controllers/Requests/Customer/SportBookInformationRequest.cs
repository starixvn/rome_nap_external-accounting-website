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
        
        public int Currency { get; set; }
        
        public object BetSettingTab
        {
            get => new
            {
                name = "BetSetting",
                id = "BetSettingTab",
                napUrl = "",
                index = 0,
                isActive = TabIndex == 0,
            };
        }

        public object MaxPayoutSettingTab
        {
            get => new
            {
                name = "MaxPayoutSetting", // chua dich
                id = "MaxPayoutSettingTab", 
                napUrl = "/Customer/GetMaxPayoutSettingInformation?site=" + Site + "&isSpecialPlatform=false",
                index = 1,
                isActive = TabIndex == 1,
            };
        }

        public object PTSettingTab
        {
            get => new
            {
                name = "PositionTaking",
                id = "PTSettingTab",
                napUrl = "/Customer/GetPTSettingInformation?custId=" + CustId + "&site=" + Site,
                index = 2,
                isActive = TabIndex == 2,
            };
        }

        public object CommissionSettingTab
        {
            get => new
            {
                name = "Commission",
                id = "CommissionHintTab",
                napUrl = "/Customer/GetCommissionHint?custId=" + CustId + "&site=" + Site,
                index = 3,
                isActive = TabIndex == 3,
            };
        }

        public object ProductCategorySettingTab
        {
            get => new
            {
                name = "ProductCategories",
                id = "ProductCategorySettingTab",
                napUrl = "/Customer/GetProductCategories?custId=" + CustId + "&site=" + Site + "&currency=" + Currency,
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