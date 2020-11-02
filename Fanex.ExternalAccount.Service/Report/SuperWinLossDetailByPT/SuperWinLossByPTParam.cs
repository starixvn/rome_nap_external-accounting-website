namespace Fanex.ExternalAccount.Service.Report.SuperWinLossDetailByPT
{
    using System;
    using System.Collections.Generic;

    public class SuperWinLossDetailByPTParam
    {
        public SuperWinLossDetailByPTParam(
            string groupName,
            DateTime fromDate,
            DateTime toDate,
            string userSelectedProductIds,
            string resourceIds,
            string tooltipsForLiveCasino,
            int pageSize,
            int pageIndex)
        {
            GroupName = groupName;
            UserSelectedProductIds = userSelectedProductIds;

            FromDate = fromDate == DateTime.MinValue ? DateTime.Now : fromDate;
            ToDate = toDate == DateTime.MinValue ? DateTime.Now : toDate;

            ResourceIds = resourceIds;
            TooltipsForLiveCasino = tooltipsForLiveCasino;

            PageSize = pageSize;
            PageIndex = pageIndex;
        }

        public string GroupName { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public string UserSelectedProductIds { get; }

        public string ResourceIds { get; }

        public string TooltipsForLiveCasino { get; }

        public int PageSize { get; }

        public int PageIndex { get; }
    }
}