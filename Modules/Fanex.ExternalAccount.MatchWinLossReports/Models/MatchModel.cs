namespace Fanex.ExternalAccount.MatchWinLossReports.Models
{
    using Fanex.ExternalAccount.Core.Shared;
    public class MatchModel
    {
        public int MatchId {get; set;  }
        
        public string MatchCode {get; set;  }
        
        public int HomeId {get; set;  }
        
        public string HomeName => CacheRef.GetTeamName(HomeId.ToString());
        
        public int AwayId {get; set;  }
        
        public string AwayName => CacheRef.GetTeamName(AwayId.ToString());
        
        public bool IsNeutral {get; set;  }
    }
}