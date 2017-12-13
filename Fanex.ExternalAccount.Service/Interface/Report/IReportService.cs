namespace Fanex.ExternalAccount.Service.Interface.Report
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.ExternalAccount.DTO.Report;

    public interface IReportService
    {
        DateTime BalanceUpDate();

        DateTime MoveBetsDate();

        DataTable GetCustomerInfo(string groupname, int custId, string userName, bool isGetUplineInfo);

        List<SuggestedUserName> GetSuggestedUserName(int custId, string username, string groupName);

        List<DailyWinLoss> GetDailyWinLoss(DateTime winlostDate, int custId, string groupName);

        List<DailyWinLoss> GetDailyWinLossSuper(DateTime winlostDate, int custId, int masterId);

        List<DailyWinLoss> GetDailyWinLossMaster(DateTime winlostDate, int custId, int agentId);

        List<DailyWinLoss> GetDailyWinLossAgent(DateTime winlostDate, int custId, int memberId);

        List<WinLossDetail> GetWinLossDetail(string groupname, int custId, DateTime fromDate, DateTime toDate, string productIds);

        List<WinLossDetail> GetWinLossDetailSuper(int custId, int masterId, DateTime fromDate, DateTime toDate, string productIds);

        List<WinLossDetailMAg> GetWinLossDetailMAg(int custId, int agentId, DateTime fromDate, DateTime toDate, string productIds);

        List<WinLossDetailAg> GetWinLossDetailAg(int custId, int memberId, DateTime fromDate, DateTime toDate, string productIds);

        ArrayList GetSortingOrderList(string groupname);
    }
}