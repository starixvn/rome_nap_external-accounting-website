namespace Fanex.ExternalAccount.Service.Impl.Report
{
    using System;
    using System.Collections;
    using Core.Shared;
    using DTO.Customer;
    using Interface.Report;

    public class WinLossDetailIBCBetLiveCasinoService : IWinLossDetailIBCBetLiveCasinoService
    {
        private readonly IReportService reportService;

        public WinLossDetailIBCBetLiveCasinoService(IReportService reportService = null)
        {
            this.reportService = reportService ?? new ReportService();
        }

        private Func<int, int, DateTime, DateTime, string, object> GetWinLossDetailIBCBetLiveCasinoFunction(int roleId)
        {
            switch (roleId)
            {
                case Role.Super:
                    return reportService.GetMasterWinLossDetailIBCBetLiveCasino;

                case Role.Master:
                    return reportService.GetAgentWinLossDetailIBCBetLiveCasino;

                case Role.Agent:
                    return reportService.GetMemberWinLossDetailIBCBetLiveCasino;

                default:
                    throw new InvalidOperationException(nameof(roleId));
            }
        }

        public dynamic DrillDownWinLossDetailIBCBetLiveCasino(IDrillDownWinLossDetailIBCBetLiveCasinoRequest request)
        {
            if (request.IsNotValid)
            {
                throw new InvalidOperationException(nameof(request.IsValid));
            }
            var sortingOrderList = reportService.GetSortingOrderList(request.GroupName);
            var getWinLossDetailIBCBetLiveCasino = GetWinLossDetailIBCBetLiveCasinoFunction(request.ViewLevel);

            var winLossDetailIBCBetLiveCasinoData = getWinLossDetailIBCBetLiveCasino(
                request.CustId,
                0,
                request.FromDate,
                request.ToDate,
                request.ProductIds);

            return
                new
                {
                    viewlevel = request.ViewLevel.DirectDownlineLevel(),
                    srecommend = request.SuperId,
                    mrecommend = request.MasterId,
                    recommend = request.AgentId,
                    rows = winLossDetailIBCBetLiveCasinoData,
                    sortingorderlist = sortingOrderList,
                    tooltips = request.ToolTipsProducts
                };
        }

        public dynamic SearchWinLossDetailIBCBetLiveCasino(ISearchWinLossDetailIBCBetLiveCasinoRequest request)
        {
            var sortingOrderList = reportService.GetSortingOrderList(request.GroupName);

            var customerInfo = new LineInfo { RoleId = Role.Super };

            if (!string.IsNullOrWhiteSpace(request.Username))
            {
                customerInfo = reportService.SearchCustomerInfo(request.GroupName, 0, request.Username, true);

                if (customerInfo == null)
                {
                    return NoInfoResult(request.Username, sortingOrderList, request.ToolTipsProducts);
                }
            }

            var winLossDetailIBCBetLiveCasinoData = SearchWinLossDetailIBCBetLiveCasinoData(request.GroupName, request.FromDate, request.ToDate, request.ProductIds, customerInfo);

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
                    rows = winLossDetailIBCBetLiveCasinoData,
                    sortingorderlist = sortingOrderList,
                    tooltips = request.ToolTipsProducts
                };
        }

        private dynamic SearchWinLossDetailIBCBetLiveCasinoData(
            string groupname,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            LineInfo customerInfo)
        {
            if (customerInfo.RoleId >= Role.Super)
            {
                return reportService.GetSuperWinLossDetailIBCBetLiveCasino(
                    groupname,
                    customerInfo.CustId,
                    fromDate,
                    toDate,
                    productIds);
            }

            var getWinLossDetailIBCBetLiveCasino = GetWinLossDetailIBCBetLiveCasinoFunction(customerInfo.RoleId.DirectUplineLevel());

            return getWinLossDetailIBCBetLiveCasino(
                customerInfo.GetUplineId(),
                customerInfo.CustId,
                fromDate,
                toDate,
                productIds);
        }

        private static dynamic NoInfoResult(string username, ArrayList sortingOrderList, dynamic toolTipsProducts) => new
        {
            viewlevel = Role.Super,
            supername = username,
            mastername = string.Empty,
            agentname = string.Empty,
            rows = string.Empty,
            sortingorderlist = sortingOrderList,
            tooltips = toolTipsProducts
        };
    }
}
