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
        LineInfo SearchCustomerInfo(string groupname, int custId, string userName, bool isGetUplineInfo);

        IEnumerable<SuggestedUserName> GetSuggestedUserName(int custId, string username, string groupName);

        IEnumerable<DailyWinLoss> GetDailyWinLoss(DateTime winlostDate, int custId, string groupName);

        IEnumerable<DailyWinLoss> GetDailyWinLossSuper(DateTime winlostDate, int custId, int masterId, int memberId);

        IEnumerable<DailyWinLoss> GetDailyWinLossMaster(DateTime winlostDate, int custId, int agentId, int memberId);

        IEnumerable<DailyWinLoss> GetDailyWinLossAgent(DateTime winlostDate, int custId, int memberId);

        IEnumerable<WinLossDetailExtra> GetSuperWinLossDetail(string groupname, int? custId, DateTime fromDate, DateTime toDate, string productIds, string productDetailsXML);

        IEnumerable<WinLossDetail> GetMasterWinLossDetail(int custId, int? masterId, DateTime fromDate, DateTime toDate, string productIds, string productDetailsXML);

        IEnumerable<WinLossDetail> GetDirectMemberOfSuperWinLossDetail(int custId, int? memberId, DateTime fromDate, DateTime toDate, string productIds, string productDetailsXML);
        
        IEnumerable<WinLossDetailMAg> GetAgentWinLossDetail(int custId, int? agentId, DateTime fromDate, DateTime toDate, string productIds, string productDetailsXML);

        IEnumerable<WinLossDetailMAg> GetDirectMemberOfMasterWinLossDetail(int custId, int? memberId, DateTime fromDate, DateTime toDate, string productIds, string productDetailsXML);

        IEnumerable<WinLossDetailAg> GetMemberWinLossDetail(int custId, int? memberId, DateTime fromDate, DateTime toDate, string productIds, string productDetailsXML);

        IEnumerable<int> GetSortingOrderList(string groupname);

        IEnumerable<ExtraPTModel> GetExtraPT(string groupName);

        string BuildTooltipsForLiveCasino(int userId,string groupName, int groupId);

        string BuildTooltipsForRNGCasino(int userId,string groupName, int groupId);

        string ConcatProductName(string productIds, string resourceIds, string groupName);
    }
}