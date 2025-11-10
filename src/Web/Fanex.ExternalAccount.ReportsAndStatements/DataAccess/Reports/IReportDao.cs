using Fanex.ExternalAccount.ReportsAndStatements.Models.DTOs.Reports;
using System.Collections.Generic;

namespace Fanex.ExternalAccount.ReportsAndStatements.DataAccess.Reports
{
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

    public interface IReportDao
    {
        LineInfo GetCustomerInfo(object criteria);
        IEnumerable<SuggestedUserName> EAS_CustomerSelUserSearch(object criteria);
        IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_Com(object criteria);
        IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_SAg(object criteria);
        IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_MAg(object criteria);
        IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_Ag(object criteria);
        // More methods would be here...
    }
}
