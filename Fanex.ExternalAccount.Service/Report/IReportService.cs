namespace Fanex.ExternalAccount.Service.Interface.Report
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using DTO.Customer;
    using DTO.Report;

    public interface IReportService
    {
        DataTable GetCustomerInfo(string groupname, int custId, string userName, bool isGetUplineInfo);

        LineInfo SearchCustomerInfo(string groupname, int custId, string userName, bool isGetUplineInfo);

        List<SuggestedUserName> GetSuggestedUserName(int custId, string username, string groupName);

        List<DailyWinLoss> GetDailyWinLoss(DateTime winlostDate, int custId, string groupName);

        List<DailyWinLoss> GetDailyWinLossSuper(DateTime winlostDate, int custId, int masterId);

        List<DailyWinLoss> GetDailyWinLossMaster(DateTime winlostDate, int custId, int agentId);

        List<DailyWinLoss> GetDailyWinLossAgent(DateTime winlostDate, int custId, int memberId);

        List<WinLossDetail> GetSuperWinLossDetail(string groupname, int custId, DateTime fromDate, DateTime toDate, string productIds);

        List<WinLossDetail> GetMasterWinLossDetail(int custId, int masterId, DateTime fromDate, DateTime toDate, string productIds);

        List<WinLossDetailMAg> GetAgentWinLossDetail(int custId, int agentId, DateTime fromDate, DateTime toDate, string productIds);

        List<WinLossDetailAg> GetMemberWinLossDetail(int custId, int memberId, DateTime fromDate, DateTime toDate, string productIds);

        ArrayList GetSortingOrderList(string groupname);
    }
}