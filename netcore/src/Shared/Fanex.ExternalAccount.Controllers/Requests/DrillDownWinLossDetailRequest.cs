namespace Fanex.ExternalAccount.Controllers.Requests
{
    using System;
    using Core.Shared;
    using Service.Interface.Report;
    using Utilities;

    public class DrillDownWinLossDetailRequest : IDrillDownWinLossDetailRequest
    {
        public DrillDownWinLossDetailRequest(WinLossDetailRequest request, dynamic toolTipsProducts)
            : this(request)
        {
            ToolTipsProducts = toolTipsProducts;
        }

        public DrillDownWinLossDetailRequest(WinLossDetailRequest request)
        {
            MainGroupName = request.MainGroupName;
            GroupId = request.GroupId;
            GroupName = request.GroupName;
            CustId = request.CustId;
            FromDate = request.Fromdate;
            ToDate = request.Todate;
            ViewLevel = request.Viewlevel;
            ProductIds = request.ProductIds;
            ResourceIds = request.ResourceIds;
            Currencies = request.Currencies;
            BaseCurrencyName = request.BaseCurrencyName;
            IsMultipleCurrency = !string.IsNullOrWhiteSpace(request.Currencies);

            switch (ViewLevel)
            {
                case Role.Super:
                    SuperId = CustId;
                    MasterId = 0;
                    AgentId = 0;
                    break;

                case Role.Master:
                    SuperId = CheckRequest.GetIntRequest("srecommend", 0);
                    MasterId = CustId;
                    AgentId = 0;
                    break;

                default:
                    SuperId = CheckRequest.GetIntRequest("srecommend", 0);
                    MasterId = CheckRequest.GetIntRequest("mrecommend", 0);
                    AgentId = CustId;
                    break;
            }
        }

        public string MainGroupName { get; }
        public int GroupId { get; }
        public string GroupName { get; }
        public int SuperId { get; }
        public int MasterId { get; }
        public int AgentId { get; }
        public int CustId { get; }
        public int ViewLevel { get; }
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }
        public string ProductIds { get; }
        public dynamic ToolTipsProducts { get; }
        public string ResourceIds { get; }
        public int UserId { get; }

        public bool IsValid => !ViewLevel.IsMember()
                               && ViewLevel <= Role.Group
                               && (ToDate >= FromDate)
                               && CustId > 0;

        public bool IsNotValid => !IsValid;
        public string Currencies { get; }
        public string BaseCurrencyName { get; }
        public bool IsMultipleCurrency { get; }
    }
}
