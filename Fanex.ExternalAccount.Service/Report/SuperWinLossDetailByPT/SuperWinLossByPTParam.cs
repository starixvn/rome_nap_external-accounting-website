namespace Fanex.ExternalAccount.Service.Report.SuperWinLossDetailByPT
{
    using System;

    public class SuperWinLossDetailByPTParam
    {
        public SuperWinLossDetailByPTParam(
            int groupId,
            string groupName,
            DateTime fromDate,
            DateTime toDate,
            string userSelectedProductIds,
            string resourceIds,
            string tooltipsForLiveCasino,
            string tooltipsForRNGCasino,
            int pageSize,
            int pageIndex,
            int userId)
        {
            GroupId = groupId;
            GroupName = groupName;
            UserSelectedProductIds = userSelectedProductIds;

            FromDate = fromDate == DateTime.MinValue ? DateTime.Now : fromDate;
            ToDate = toDate == DateTime.MinValue ? DateTime.Now : toDate;

            ResourceIds = resourceIds;
            TooltipsForLiveCasino = tooltipsForLiveCasino;
            TooltipsForRNGCasino = tooltipsForRNGCasino;

            PageSize = pageSize;
            PageIndex = pageIndex;

            UserId = userId;
        }

        public int GroupId { get; }

        public string GroupName { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public string UserSelectedProductIds { get; }

        public string ResourceIds { get; }

        public string Currencies { get; set; }

        public int BaseCurrency { get; set; }

        public string BaseCurrencyName { get; set; }

        public string TooltipsForLiveCasino { get; }

        public string TooltipsForRNGCasino { get; }

        public int PageSize { get; }

        public int PageIndex { get; }

        public int UserId { get; }
        
        public bool IsValid() => FromDate <= ToDate;
    }
}