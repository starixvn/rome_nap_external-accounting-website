namespace Fanex.ExternalAccount.ViewModel.Customer
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoModel
    {
        public CustomerInfo CustomerInfo { get; set; }

        public virtual PTModel SuperPtSetting { get; set; }

        public virtual PTModel MasterPtSetting { get; set; }

        public virtual PTModel UplinePtSetting { get; set; }

        public virtual PTModel PtSetting { get; set; }
    }
}