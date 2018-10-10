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

        List<DailyWinLoss> GetDailyWinLoss(DateTime winlostDate, int custId, string groupName, string excludeProductIds);

        List<DailyWinLoss> GetDailyWinLossSuper(DateTime winlostDate, int custId, int masterId, string excludeProductIds);

        List<DailyWinLoss> GetDailyWinLossMaster(DateTime winlostDate, int custId, int agentId, string excludeProductIds);

        List<DailyWinLoss> GetDailyWinLossAgent(DateTime winlostDate, int custId, int memberId, string excludeProductIds);

        List<WinLossDetail> GetSuperWinLossDetail(string groupname, int custId, DateTime fromDate, DateTime toDate, string productIds);

        List<WinLossDetail> GetMasterWinLossDetail(int custId, int masterId, DateTime fromDate, DateTime toDate, string productIds);

        List<WinLossDetailMAg> GetAgentWinLossDetail(int custId, int agentId, DateTime fromDate, DateTime toDate, string productIds);

        List<WinLossDetailAg> GetMemberWinLossDetail(int custId, int memberId, DateTime fromDate, DateTime toDate, string productIds);

        ArrayList GetSortingOrderList(string groupname);

        List<PokerGamesWinLossDetail> GetSuperPokerGamesWinLossDetail(string groupname, int custId, DateTime fromDate, DateTime toDate);

        List<PokerGamesWinLossDetail> GetMasterPokerGamesWinLossDetail(int custId, int masterId, DateTime fromDate, DateTime toDate);

        List<PokerGamesWinLossDetailMAg> GetAgentPokerGamesWinLossDetail(int custId, int agentId, DateTime fromDate, DateTime toDate);

        List<PokerGamesWinLossDetailAg> GetMemberPokerGamesWinLossDetail(int custId, int memberId, DateTime fromDate, DateTime toDate);

        IEnumerable<int> GetExcludeProductIds(string groupName);

        bool IsShowIncludePockerGames(string groupName);
    }
}