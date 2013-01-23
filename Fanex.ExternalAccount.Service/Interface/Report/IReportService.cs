// -----------------------------------------------------------------------
// <copyright file="IReportService.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Service.Interface.Report
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Collections;
    using Fanex.ExternalAccount.DTO.Report;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IReportService
    {
        DataTable GetCustomerInfo(string groupname, int custId, string userName, Boolean isGetUplineInfo);

        List<SuggestedUserName> GetSuggestedUserName(int custId, string username, string groupName);

        List<DailyWinLoss> GetDailyWinLoss(DateTime winlostDate, int custId, string groupName);

        List<DailyWinLoss> GetDailyWinLossSuper(DateTime winlostDate, int custId, int masterId);

        List<DailyWinLoss> GetDailyWinLossMaster(DateTime winlostDate, int custId, int agentId);

        List<DailyWinLoss> GetDailyWinLossAgent(DateTime winlostDate, int custId, int memberId);


        List<WinLossDetail> GetWinLossDetail(string groupname, int custId, DateTime fromDate, DateTime toDate, ICollection<KeyValuePair<string, bool>> filterParams);

        List<WinLossDetail> GetWinLossDetailSuper(int custId, int masterId, DateTime fromDate, DateTime toDate, ICollection<KeyValuePair<string, bool>> filterParams);

        List<WinLossDetailMAg> GetWinLossDetailMAg(int custId, int agentId, DateTime fromDate, DateTime toDate, ICollection<KeyValuePair<string, bool>> filterParams);

        List<WinLossDetailAg> GetWinLossDetailAg(int custId, int memberId, DateTime fromDate, DateTime toDate, ICollection<KeyValuePair<string, bool>> filterParams);

        ArrayList GetSortingOrderList(string groupname);
    }
}