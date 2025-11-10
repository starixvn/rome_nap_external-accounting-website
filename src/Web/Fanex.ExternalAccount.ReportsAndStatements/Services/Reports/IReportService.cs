using Fanex.ExternalAccount.ReportsAndStatements.Models.DTOs.Reports;
using System;
using System.Collections.Generic;

namespace Fanex.ExternalAccount.ReportsAndStatements.Services.Reports
{
    public interface IReportService
    {
        LineInfo SearchCustomerInfo(string groupname, int custId, string userName, bool isGetUplineInfo);

        IEnumerable<SuggestedUserName> GetSuggestedUserName(int custId, string username, string groupName);

        IEnumerable<DailyWinLoss> GetDailyWinLoss(DateTime winlostDate, int custId, string groupName);

        IEnumerable<DailyWinLoss> GetDailyWinLossSuper(DateTime winlostDate, int custId, int masterId, int memberId);

        IEnumerable<DailyWinLoss> GetDailyWinLossMaster(DateTime winlostDate, int custId, int agentId, int memberId);

        IEnumerable<DailyWinLoss> GetDailyWinLossAgent(DateTime winlostDate, int custId, int memberId);

        CashFee GetCashFee(int custId, DateTime fromDate, DateTime toDate);

        string BuildTooltipsForLiveCasino(int userId, string groupName, int groupId);

        string BuildTooltipsForRNGCasino(int userId, string groupName, int groupId);

        string ConcatProductName(string productIds, string resourceIds, string groupName);
    }

    // Placeholder classes
    public class LineInfo
    {
        public int CustId { get; set; }
        public int RoleId { get; set; }
        public int SuperId { get; set; }
        public int MasterId { get; set; }
        public int AgentId { get; set; }
    }

    public class SuggestedUserName
    {
        public string Username { get; set; }
    }

    public class CashFee
    {
        public double Amount { get; set; }
    }
}
