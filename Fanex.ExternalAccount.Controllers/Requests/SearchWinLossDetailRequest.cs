namespace Fanex.ExternalAccount.Controllers.Requests
{
    using System;
    using Service.Interface.Report;

    public class SearchWinLossDetailRequest : ISearchWinLossDetailRequest
    {
        public SearchWinLossDetailRequest(WinLossDetailRequest request, dynamic toolTipsProducts)
            : this(request)
        {
            ProductIds = request.GetProductIds();
            ToolTipsProducts = toolTipsProducts;
        }

        public SearchWinLossDetailRequest(WinLossDetailRequest request)
        {
            GroupName = request.groupName;
            Username = request.username;
            FromDate = request.fromdate;
            ToDate = request.todate;
        }

        public string GroupName { get; }
        public string Username { get; }
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }
        public string ProductIds { get; }
        public dynamic ToolTipsProducts { get; }
    }
}
