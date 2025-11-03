namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;
    using Models;

    public class CustomerStatus
    {
        public CustomerStatus()
        {
            ProductStatusDic = new Dictionary<int, ProductStatus>();
        }

        public CustomerStatus UplineStatus { get; set; }

        public int RoleId { get; set; }

        public bool AllowDoubleComm { get; set; }

        public bool DisableTransfer { get; set; }

        public bool AllowOutright { get; set; }

        public bool DisableCasino { get; set; }

        public bool Closed { get; set; }

        public bool Suspended { get; set; }

        public Dictionary<int, ProductStatus> ProductStatusDic { get; set; }
    }
}