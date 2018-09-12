namespace Fanex.ExternalAccount.Service.Impl.Report
{
    using System;
    using System.Collections;
    using Core.Shared;
    using DTO.Customer;
    using Interface.Report;

    public class PokerGamesWinLossDetailServiceImpl : PokerGamesWinLossDetailService
    {
        private readonly IReportService reportService;

        public PokerGamesWinLossDetailServiceImpl(IReportService reportService = null)
        {
            this.reportService = reportService ?? new ReportService();
        }

        private Func<int, int, DateTime, DateTime, object> GetPokerGamesWinLossDetailFunction(int roleId)
        {
            switch (roleId)
            {
                case Role.Super:
                    return reportService.GetMasterPokerGamesWinLossDetail;

                case Role.Master:
                    return reportService.GetAgentPokerGamesWinLossDetail;

                case Role.Agent:
                    return reportService.GetMemberPokerGamesWinLossDetail;

                default:
                    throw new InvalidOperationException(nameof(roleId));
            }
        }

        public dynamic DrillDownPokerGamesWinLossDetail(IDrillDownPokerGamesWinLossDetailRequest request)
        {
            if (request.IsNotValid)
            {
                throw new InvalidOperationException(nameof(request.IsValid));
            }
            var sortingOrderList = reportService.GetSortingOrderList(request.GroupName);
            var getWinLossDetail = GetPokerGamesWinLossDetailFunction(request.ViewLevel);

            var winLossDetailData = getWinLossDetail(
                request.CustId,
                0,
                request.FromDate,
                request.ToDate);

            return
                new
                {
                    viewlevel = request.ViewLevel.DirectDownlineLevel(),
                    srecommend = request.SuperId,
                    mrecommend = request.MasterId,
                    recommend = request.AgentId,
                    rows = winLossDetailData,
                    sortingorderlist = sortingOrderList
                };
        }

        public dynamic SearchPokerGamesWinLossDetail(ISearchPokerGamesWinLossDetailRequest request)
        {
            var sortingOrderList = reportService.GetSortingOrderList(request.GroupName);

            var customerInfo = new LineInfo { RoleId = Role.Super };

            if (!string.IsNullOrWhiteSpace(request.Username))
            {
                customerInfo = reportService.SearchCustomerInfo(request.GroupName, 0, request.Username, true);

                if (customerInfo == null)
                {
                    return NoInfoResult(request.Username, sortingOrderList);
                }
            }

            var winLossDetailData = SearchPokerGamesWinLossDetailData(request.GroupName, request.FromDate, request.ToDate, customerInfo);

            return
                new
                {
                    viewlevel = customerInfo.RoleId,
                    supername = customerInfo.SuperName,
                    mastername = customerInfo.MasterName,
                    agentname = customerInfo.AgentName,
                    srecommend = customerInfo.SuperId,
                    mrecommend = customerInfo.MasterId,
                    recommend = customerInfo.AgentId,
                    rows = winLossDetailData,
                    sortingorderlist = sortingOrderList
                };
        }

        private dynamic SearchPokerGamesWinLossDetailData(
            string groupname,
            DateTime fromDate,
            DateTime toDate,
            LineInfo customerInfo)
        {
            if (customerInfo.RoleId >= Role.Super)
            {
                return reportService.GetSuperPokerGamesWinLossDetail(
                    groupname,
                    customerInfo.CustId,
                    fromDate,
                    toDate);
            }

            var getWinLossDetail = GetPokerGamesWinLossDetailFunction(customerInfo.RoleId.DirectUplineLevel());

            return getWinLossDetail(
                customerInfo.GetUplineId(),
                customerInfo.CustId,
                fromDate,
                toDate);
        }

        private static dynamic NoInfoResult(string username, ArrayList sortingOrderList) => new
        {
            viewlevel = Role.Super,
            supername = username,
            mastername = string.Empty,
            agentname = string.Empty,
            rows = string.Empty,
            sortingorderlist = sortingOrderList
        };
    }
}