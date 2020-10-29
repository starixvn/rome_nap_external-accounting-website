namespace Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT
{
    using Fanex.NAP.Framework.Data;
    using System;

    public class SuperWinLossDetailByPTCriteria : CriteriaBase
    {
        public SuperWinLossDetailByPTCriteria(
            string groupName,
            DateTime fromDate,
            DateTime toDate,
            DateTime balanceUpDay,
            DateTime moveBetsDay,
            string productIds,
            string productDetails,
            int pageSize,
            int pageIndex)
        {
            GroupName = groupName;
            FromDate = fromDate;
            ToDate = toDate;
            BalanceUpDay = balanceUpDay;
            MoveBetsDay = moveBetsDay;
            ProductDetails = productDetails;
            ProductIds = productIds;
            PageSize = pageSize;
            PageIndex = pageIndex;
        }

        public string GroupName { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public DateTime BalanceUpDay { get; }

        public DateTime MoveBetsDay { get; }

        public string ProductIds { get; }

        public string ProductDetails { get; }

        public int PageSize { get; }

        public int PageIndex { get; }

        public override string GetSettingKey() => "Age_Rpt_WinLossByProduct_Master";
    }
}
