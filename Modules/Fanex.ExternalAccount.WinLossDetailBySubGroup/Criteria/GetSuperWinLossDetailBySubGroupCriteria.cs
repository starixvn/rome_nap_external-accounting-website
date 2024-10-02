namespace Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Criteria
{
    using System;
    using Fanex.NAP.Framework.Data;

    public class GetSuperWinLossDetailBySubGroupCriteria : CriteriaBase
    {
        public GetSuperWinLossDetailBySubGroupCriteria(
            string productIds,
            string productDetails,
            string groupName,
            string subGroup,
            string subGroupList,
            DateTime fromDate,
            DateTime toDate,
            DateTime balanceUpDay,
            DateTime moveBetsDay,
            int pageIndex,
            int pageSize)
        {
            ProductIds = productIds;
            ProductDetails = productDetails;
            GroupName = groupName;
            SubGroup = subGroup;
            SubGroupList = subGroupList;
            FromDate = fromDate;
            ToDate = toDate;
            BalanceUpDay = balanceUpDay;
            MoveBetsDay = moveBetsDay;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public string ProductIds { get; }

        public string ProductDetails { get; }

        public string GroupName { get; }

        public string SubGroup { get; }

        public string SubGroupList { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public DateTime BalanceUpDay { get; }

        public DateTime MoveBetsDay { get; }

        public int PageIndex { get; }

        public int PageSize { get; }

        public int TotalRecords { get; }

        public override string GetSettingKey() => "EAS_Rpt_WinLossDetail_bySubGroup";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(ProductIds)
            && !string.IsNullOrWhiteSpace(GroupName)
            && FromDate <= ToDate;
    }
}
