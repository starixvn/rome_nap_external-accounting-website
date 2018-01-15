namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;

    public class CustomerModel
    {
        public CustomerModel()
        {
            BalanceInfo = new BalanceInfo();
            DownlineList = new DownlineList();
            Login = new List<ObjectInfo>();
            OnlineList = new OnlineList();
            PersonalInfo = new PersonalInfo();
            Setting = new Setting();
        }

        public PersonalInfo PersonalInfo { get; set; }

        public BalanceInfo BalanceInfo { get; set; }

        public DownlineList DownlineList { get; set; }

        public Setting Setting { get; set; }

        public List<ObjectInfo> Login { get; set; }

        public OnlineList OnlineList { get; set; }

        public bool HasDownline { get; set; }

        public int CurrencyId { get; set; }

        public bool UplineHasSecurityCode { get; set; }
    }
}