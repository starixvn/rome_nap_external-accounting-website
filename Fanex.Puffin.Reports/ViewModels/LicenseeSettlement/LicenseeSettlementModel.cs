// -----------------------------------------------------------------------
// <copyright file="SportBookModel.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Fanex.Puffin.Report.ViewModels.LicenseeSettlement
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class LicenseeSettlementModel
    {
        public LicenseeInformation LicenseeInfor { get; set; }

        public Report Report { get; set; }

        public List<ExchangeRate> ExchangeList { get; set; }

        public LicenseeSettlementModel()
        {
            ExchangeList = new List<ExchangeRate>();
            Report = new Report();
            LicenseeInfor = new LicenseeInformation();
        }
    }

    [Serializable]
    public class Report
    {
        public List<ReportEntity> EntityList { get; set; }

        public ReportEntity GrandTotal { get; set; }

        public Report()
        {
            EntityList = new List<ReportEntity>();
            GrandTotal = new ReportEntity();
        }
    }

    [Serializable]
    public class ReportEntity
    {
        public DateTime WinlostDate { get; set; }

        public int Currency { get; set; }

        public string CurrencyName { get; set; }

        public double Turnover { get; set; }

        public double TurnoverBase { get; set; }

        public double Betcount { get; set; }

        public double MemberWinloss { get; set; }

        public double MemberWinlossBase { get; set; }

        public double MemberComm { get; set; }

        public double MemberTotal { get; set; }

        public double LicenseeWL { get; set; }

        public double LicenseeWLBase { get; set; }

        public double LicenseeComm { get; set; }

        public double LicenseeCommBase { get; set; }

        public double LicenseeTotal { get; set; }

        public double Company { get; set; }

        public double CompanyBaseCurrency { get; set; }

        public double BaseCurrencyExchangeRate { get; set; }
    }
}
