namespace Fanex.CustomerSetting.Module.BetSetting.ViewModels
{
    using System.Collections.Generic;
    using LiveCasinoSetting.Model;

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