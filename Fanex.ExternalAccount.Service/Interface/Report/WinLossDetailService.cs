namespace Fanex.ExternalAccount.Service.Interface.Report
{
    using System;

    public interface WinLossDetailService
    {
        dynamic DrillDownWinLossDetail(IDrillDownRequest request, dynamic toolTipsProducts);

        dynamic SearchWinLossDetail(
            string username,
            string groupname,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            dynamic toolTipsProducts);
    }
}