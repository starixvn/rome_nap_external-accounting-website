namespace Fanex.ExternalAccount.MatchWinLossReports.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Models;

    public class MatchWinLossReportsViewModels
    {
        public DateTime Today { get; set; }
        
        public DateTime MinDate { get; set; }

        public string TodayString { get; set; }
        
        public string MinDateString { get; set; }

        public string HistoricPivotDate { get; set; }

        public IEnumerable<SportModel> Sports { get; set; }

        public IEnumerable<LeagueModel> Leagues { get; set; }
        
        public IEnumerable<MatchModel> Events { get; set; }

        public IEnumerable<BetTypeModel> BetTypes { get; set; }

        public IEnumerable<CurrencyModel> Currencies { get; set; }

        public bool IsMY { get; set; }
        
        public string ReportGuide { get; set; }
    }
}