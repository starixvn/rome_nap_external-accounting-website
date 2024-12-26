namespace Fanex.ExternalAccount.WhiteLabelWinLossReports.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Fanex.CustomerSetting.Module.Core.Extensions;
    using Fanex.ExternalAccount.WhiteLabelWinLossReports.Models;

    public class WhiteLabelReportViewModel
    {
        public WhiteLabelReportViewModel(IEnumerable<WhiteLabelModel> whiteLabels)
        {
            WhiteLabels = whiteLabels;
        }

        public string FormatedToday { get; } = DateTime.Today.ToShortFormatedDateWithSlash();

        public string FormatedMinDate { get; } = DateTime.Today.AddMonths(-3).FirstDayOfMonth().ToShortFormatedDateWithSlash();

        public IEnumerable<WhiteLabelModel> WhiteLabels { get; }
    }
}