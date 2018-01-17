namespace Fanex.ExternalAccount.Module.LeftPanel.DataAccesses
{
    using System.Collections.Generic;
    using System.Data;
    using Criteria;
    using Data.DNAide;
    using DTO;
    using ViewModels;

    public interface CustomerDataAccesses
    {
        DataSet GetCustomerInfo(string username, string groupName = "ACCDDD");

        Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit);

        DbResult GetDownlineList(int custid, int pageSize, int pageIndex);

        DataRowCollection GetOnlineList(int custid);

        IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int role);

        IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId);

        Result ResetMemberCredit(int custId, int agentId, bool closedOnly);

        Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip);

        DataTable CustomerSelectClearBalance(int custId);

        Result CloseCutomer(int custId, bool close, bool clearCredit, string custName);

        Result SuspendCustomer(int custId, bool suspend);

        Result AllowOutright(int custId, bool allow);

        Result EnableCasino(int id, bool enable, bool enableDownline, bool sync, string custName);

        Result EnableNumberGame(int id, bool enable, bool enableDownline);

        Result EnableLiveCasino(int id, bool enable, bool enableDownline);

        Result EnableESports(int id, bool enable, bool enableDownline);

        Result AutoMasterPT(int id, bool allow);

        Result ResetSecurityCode(int id, string code, bool force);

        Result EnableHRFixedOdds(int id, bool enable, bool enableDownline);

        Result UpdateGoldDeluxe(int id, bool enable, bool enableDownline, string custName, int userId);

        Result RemoveOTP(RemoveOtpCriteria criteria);
    }
}