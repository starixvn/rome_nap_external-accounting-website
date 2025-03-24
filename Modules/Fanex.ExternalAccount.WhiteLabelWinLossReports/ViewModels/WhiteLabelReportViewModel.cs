namespace Fanex.ExternalAccount.WhiteLabelWinLossReports.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.Core.Shared.Extensions;
    using Fanex.ExternalAccount.WhiteLabelWinLossReports.Models;

    public class WhiteLabelReportViewModel
    {
        public WhiteLabelReportViewModel(IEnumerable<WhiteLabelModel> whiteLabels)
        {
            WhiteLabels = whiteLabels;
        }

        public string FormatedToday { get; } = DateTime.Today.ToShortDateWithSlash();

        public string FormatedMinDate { get; } = DateTime.Today.AddMonths(-3).FirstDayOfMonth().ToShortDateWithSlash();

        public IEnumerable<WhiteLabelModel> WhiteLabels { get; }
    }
}