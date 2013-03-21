// --------------------------------------------------------------------------
// <copyright file="LiveCasinoModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.3.21">Created</change>
// </history>

namespace Fanex.ExternalAccount.ViewModel.Customer
{
    using System.Collections.Generic;
    using Fanex.LiveCasinoSetting.Model;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoModel
    {
        public List<decimal> AutoPT_Items { get; set; }

        public CustomerInfo CustomerInfo { get; set; }

        public List<GameType> GameSetting { get; set; }

        public List<decimal> MaxPT_Items { get; set; }

        public List<decimal> MinPT_Items { get; set; }

        public List<decimal> PresetPT_Items { get; set; }

        public virtual PTModel SuperPTSetting { get; set; }

        public virtual PTModel MasterPTSetting { get; set; }

        public virtual PTModel AgentPTSetting { get; set; }

        public virtual PTModel MemberPTSetting { get; set; }

        public virtual IMemberWinLimit MemberWinLimit { get; set; }
    }
}