namespace Fanex.ExternalAccount.ViewModel.Customer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PTModel
    {
        public decimal AutoPresetPT { get; set; }

        public decimal Comm { get; set; }

        public bool IsAutoPresetPT { get; set; }

        public bool IsAutoPresetPTChanged { get; set; }

        public bool IsCommChanged { get; set; }

        public bool IsDirty { get; set; }

        public bool IsIsAutoPresetPTChanged { get; set; }

        public bool IsMaxPTChanged { get; set; }

        public bool IsMinPTChanged { get; set; }

        public bool IsPresetPTChanged { get; set; }

        public bool IsUpdateAllMember { get; set; }

        public bool IsValid { get; set; }

        public decimal MaxAutoPresetPT { get; set; }

        public decimal MaxPT { get; set; }

        public decimal MinPT { get; set; }

        public decimal NewMemberPT { get; set; }

        public decimal PresetPT { get; set; }
    }
}