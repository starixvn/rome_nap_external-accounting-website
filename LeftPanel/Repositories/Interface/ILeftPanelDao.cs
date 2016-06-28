// --------------------------------------------------------------------------
// <copyright file="ILeftPanelDao.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.7.29">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.Repositories.Interface
{
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ILeftPanelDao
    {
        DataSet GetCustomerInfo(string username, string groupName = "ACCDDD");

        Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit);

        DbResult GetDownlineList(int custid, int pageSize, int pageIndex);

        DataRowCollection GetOnlineList(int custid);

        IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId);

        IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId);

        Result ResetMemberCredit(int custId, int agentId, bool closedOnly);

        Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip);

        DataTable CustomerSelectClearBalance(int custId);

        Result CloseCutomer(int custId, bool close, bool clearCredit, string custName);

        Result SuspendCustomer(int custId, bool suspend);

        Result AllowOutright(int custId, bool allow);

        Result EnableCasino(int id, bool enable, bool enableDownline, bool sync, string custName);

        Result EnableBingo(int id, bool enable, bool sync, bool enableDownline);

        Result EnableNumberGame(int id, bool enable, bool enableDownline);

        Result EnableLiveCasino(int id, bool enable, bool enableDownline);

        Result EnableVirtualSports(int id, bool enable, bool enableDownline);

        Result EnableESports(int id, bool enable, bool enableDownline);

        Result AutoMasterPT(int id, bool allow);

        Result ResetSecurityCode(int id, string code, bool force);

        Result EnableHRFixedOdds(int id, bool enable, bool enableDownline);

        Result UpdateGoldDeluxe(int id, bool enable, bool enableDownline, string custName, int userId);
    }
}
