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

        List<DailyWinLoss> GetDailyWinLossSuper(DateTime winlostDate, int custId, int masterId, int memberId);

        List<DailyWinLoss> GetDailyWinLossMaster(DateTime winlostDate, int custId, int agentId, int memberId);

        List<DailyWinLoss> GetDailyWinLossAgent(DateTime winlostDate, int custId, int memberId);

        List<WinLossDetailExtra> GetSuperWinLossDetail(string groupname, int custId, DateTime fromDate, DateTime toDate, string productIds, string productDetailsXML);

        List<WinLossDetail> GetMasterWinLossDetail(int custId, int masterId, DateTime fromDate, DateTime toDate, string productIds, string productDetailsXML);
        
        List<WinLossDetail> GetDirectMemberOfSuperWinLossDetail(int custId, int memberId, DateTime fromDate, DateTime toDate, string productIds, string productDetailsXML);
        
        List<WinLossDetailMAg> GetAgentWinLossDetail(int custId, int agentId, DateTime fromDate, DateTime toDate, string productIds, string productDetailsXML);
        
        List<WinLossDetailMAg> GetDirectMemberOfMasterWinLossDetail(int custId, int memberId, DateTime fromDate, DateTime toDate, string productIds, string productDetailsXML);

        List<WinLossDetailAg> GetMemberWinLossDetail(int custId, int memberId, DateTime fromDate, DateTime toDate, string productIds, string productDetailsXML);

        List<WinLossDetailIBCBetLiveCasino> GetSuperWinLossDetailIBCBetLiveCasino(string groupname, DateTime fromDate, DateTime toDate);

        ArrayList GetSortingOrderList(string groupname);

        List<ExtraPTModel> GetExtraPT(string groupName);

        string BuildTooltipsForLiveCasino(int userId,string groupName, int groupId);

        string BuildTooltipsForRNGCasino(int userId,string groupName, int groupId);

        string ConcatProductName(string productIds, string resourceIds, string groupName);
    }
}