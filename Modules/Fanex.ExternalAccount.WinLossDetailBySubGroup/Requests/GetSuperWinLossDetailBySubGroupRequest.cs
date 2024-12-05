namespace Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Requests
{
    using System;

    public class GetSuperWinLossDetailBySubGroupRequest
    {
        public string ProductIds { get; set; }

        public string ProductDetails { get; set; }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string SubGroup { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
