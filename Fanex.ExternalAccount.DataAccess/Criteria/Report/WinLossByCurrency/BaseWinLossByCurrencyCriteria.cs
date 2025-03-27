using Fanex.Data.Repository;
using System;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    public abstract class BaseWinLossByCurrencyCriteria : ICriteria
    {
        public string GroupName { get; set; }
        public int? CustId { get; set; }
        public string BaseCurrency { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ProductIds { get; set; }
        public string ProductDetails { get; set; }
        public DateTime BalanceUpDay { get; set; }
        public DateTime MoveBetsDay { get; set; }

        public abstract string GetSettingKey();

        public virtual bool IsValid() => !string.IsNullOrEmpty(GroupName);
    }
} 