namespace Fanex.ExternalAccount.Controllers.Requests
{
    using System;
    using Service.Interface.Report;

    public class SearchWinLossDetailRequest : ISearchWinLossDetailRequest
    {
        public SearchWinLossDetailRequest(WinLossDetailRequest request, dynamic toolTipsProducts)
            : this(request)
        {
            ToolTipsProducts = toolTipsProducts;
        }

        public SearchWinLossDetailRequest(WinLossDetailRequest request)
        {
            MainGroupName = request.MainGroupName;
            GroupId = request.GroupId;
            GroupName = request.GroupName;
            Username = request.Username;
            FromDate = request.Fromdate;
            ToDate = request.Todate;
            ProductIds = request.ProductIds;
            ResourceIds = request.ResourceIds;
            UserId = request.UserId;
            Currencies = request.Currencies;
            BaseCurrencyName = request.BaseCurrencyName;
            IsMultipleCurrency = !string.IsNullOrWhiteSpace(request.Currencies);
        }

        public string MainGroupName { get; }
        public int GroupId { get; }
        public string GroupName { get; }
        public string Username { get; }
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }
        public string ProductIds { get; }
        public dynamic ToolTipsProducts { get; }
        public string ResourceIds { get; }
        public int UserId { get; }
        public string Currencies { get; }
        public string BaseCurrencyName { get; }
        public bool IsMultipleCurrency { get; }
        public bool IsValid => (ToDate >= FromDate);
    }
}
