namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Model for left panel.
    /// </summary>
    public class CustomerModel
    {
        public CustomerModel()
        {
            this.BalanceInfo = new BalanceInfo();
            this.DownlineList = new DownlineList();
            this.Login = new List<ObjectInfo>();
            this.OnlineList = new OnlineList();
            this.PersonalInfo = new PersonalInfo();
            this.Setting = new Setting();
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