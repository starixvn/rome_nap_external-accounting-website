namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;

    public class BalanceInfo
    {
        public BalanceInfo()
        {
            Balances = new List<ObjectInfo>();
            Statuses = new List<Status>();
        }

        public ObjectInfo CurrencyId { get; set; }

        public ObjectInfo Currency { get; set; }

        public List<ObjectInfo> Balances { get; set; }

        public List<Status> Statuses { get; set; }

        public bool IsUpdateCredit { get; set; }
    }
}
