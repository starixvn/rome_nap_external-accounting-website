namespace Fanex.CustomerSetting.Module.BetSetting.ViewModels
{
    using System.Collections.Generic;
    using Fanex.LiveCasinoSetting.Model;

    public class LiveCasinoModel
    {
        public MemberWinLimit MemberWinLimit
        {
            get;
            set;
        }

        public List<GameType> BettingLimits
        {
            get;
            set;
        }
    }
}