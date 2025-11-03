using Fanex.Data.Repository;
using System;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Report.WinLossDetail
{
    public class GetCashFeeCriteria : ICriteria
    {
        public int CustId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; } 

        public DateTime BalanceUpDay { get; set; }

        public DateTime MoveBetsDay { get; set; } 

        public string GetSettingKey() => "Age_Rpt_CashServiceBilling";

        public bool IsValid() => true;
    }
}
