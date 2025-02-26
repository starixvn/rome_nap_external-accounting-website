namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using Fanex.Data.Repository;
    using System;

    public class GetSuperWinlossDetailCriteria : CriteriaBase
    {
        public string GroupName { get; }

        public int CustId { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public string ProductIds { get; }

        public string ProductDetailsXML { get; }

        public DateTime BalanceUpDay { get; }

        public DateTime MoveBetsDay { get; }

#pragma warning disable S107 // Methods should not have too many parameters
        public GetSuperWinlossDetailCriteria(
            string groupName,
            int custId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            GroupName = groupName;
            CustId = custId;
            FromDate = fromDate;
            ToDate = toDate;
            ProductIds = productIds;
            ProductDetailsXML = productDetailsXML;
            BalanceUpDay = balanceUpDay;
            MoveBetsDay = moveBetsDay;
        }

        public override string GetSettingKey() => "EAS_Rpt_WinLossDetail_Com";

        public override bool IsValid() => true;
    }
}
