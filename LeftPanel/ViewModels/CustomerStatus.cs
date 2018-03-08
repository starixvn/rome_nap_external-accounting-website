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

        public bool SyncCasino { get; set; }

        public bool SyncLiveCasino { get; set; }

        public bool AllowDoubleComm { get; set; }

        public bool DisableTransfer { get; set; }

        public bool DisableVirtualSports { get; set; }

        public bool DisableESports { get; set; }

        public bool AllowOutright { get; set; }

        public bool DisableNumberGame { get; set; }

        public bool DisableCasino { get; set; }

        public bool DisableLiveCasino { get; set; }

        public bool Closed { get; set; }

        public bool Suspended { get; set; }

        public bool HideVirtualSports { get; set; }

        public bool DisableHRFixedOdds { get; set; }

        public bool HideKeno { get; set; }

        public bool HideEsports { get; set; }

        public bool DisableKeno { get; set; }

        public bool DisableGoldDeluxe { get; set; }

        public bool HideGoldDeluxe { get; set; }

        public bool DisableColossusBets { get; set; }

        public bool HideColossusBets { get; set; }

        public bool DisableSportsbook2 { get; set; }

        public bool HideSportsbook2 { get; set; }

        public bool DisableCockfighting { get; set; }

        public bool HideCockfighting { get; set; }

        public bool HideCricket { get; set; }

        public bool SyncGoldDeluxeDownlines { get; set; }

        public bool SyncBullfighting { get; set; }

        public bool SyncBullfightingDownlines { get; set; }

        public Dictionary<int, ProductStatus> ProductStatusDic { get; set; }
    }
}