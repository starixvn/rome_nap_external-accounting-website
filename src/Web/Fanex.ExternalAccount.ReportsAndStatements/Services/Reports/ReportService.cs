using Fanex.ExternalAccount.ReportsAndStatements.Models.DTOs.Reports;
using System;
using System.Collections.Generic;

namespace Fanex.ExternalAccount.ReportsAndStatements.Services.Reports
{
    public class ReportService : IReportService
    {
        public static DateTime BalanceUpDate => new DateTime(2020, 1, 1);
        public static DateTime MoveBetsDate => new DateTime(2020, 1, 1);

        public LineInfo SearchCustomerInfo(string groupname, int custId, string userName, bool isGetUplineInfo)
        {
            // TODO: Implement actual customer search logic
            return new LineInfo();
        }

        public IEnumerable<SuggestedUserName> GetSuggestedUserName(int custId, string username, string groupName)
        {
            // TODO: Implement actual suggested username logic
            return new List<SuggestedUserName>();
        }

        public IEnumerable<DailyWinLoss> GetDailyWinLoss(DateTime winlostDate, int custId, string groupName)
        {
            // TODO: Implement actual daily win loss logic
            return new List<DailyWinLoss>();
        }

        public IEnumerable<DailyWinLoss> GetDailyWinLossSuper(DateTime winlostDate, int custId, int masterId, int memberId)
        {
            // TODO: Implement actual super level daily win loss logic
            return new List<DailyWinLoss>();
        }

        public IEnumerable<DailyWinLoss> GetDailyWinLossMaster(DateTime winlostDate, int custId, int agentId, int memberId)
        {
            // TODO: Implement actual master level daily win loss logic
            return new List<DailyWinLoss>();
        }

        public IEnumerable<DailyWinLoss> GetDailyWinLossAgent(DateTime winlostDate, int custId, int memberId)
        {
            // TODO: Implement actual agent level daily win loss logic
            return new List<DailyWinLoss>();
        }

        public CashFee GetCashFee(int custId, DateTime fromDate, DateTime toDate)
        {
            // TODO: Implement actual cash fee logic
            return new CashFee();
        }

        public string BuildTooltipsForLiveCasino(int userId, string groupName, int groupId)
        {
            // TODO: Implement actual tooltip building logic
            return string.Empty;
        }

        public string BuildTooltipsForRNGCasino(int userId, string groupName, int groupId)
        {
            // TODO: Implement actual tooltip building logic
            return string.Empty;
        }

        public string ConcatProductName(string productIds, string resourceIds, string groupName)
        {
            // TODO: Implement actual product name concatenation logic
            return string.Empty;
        }
    }
}