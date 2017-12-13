namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Impl
{
    using System.Collections.Generic;
    using System.Data;
    using DTO;
    using Interface;
    using ViewModels;

    public abstract class BaseService : ILeftPanelService
    {
        public abstract CustomerModel GetCustomerInfo(string username, string groupName = "ACCDDD", int pageIndex = 1);

        public abstract DataTable GetDownlineList(int custid, int pageSize, int pageIndex);

        public abstract DataRowCollection GetOnlineList(int custid);

        public abstract Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit);

        public abstract IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId);

        public abstract IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId);

        public abstract Result ResetMemberCredit(int custId, int agentId, bool closedOnly);

        public abstract Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip);

        public abstract DataTable CustomerSelectClearBalance(int custId);

        public abstract Result CloseCutomer(int custId, bool close, bool clearCredit, string custName);

        public abstract Result SuspendCustomer(int custId, bool suspend);

        public abstract Result AllowOutright(int custId, bool allow);

        public abstract Result EnableCasino(int id, bool enable, bool enableDownline, bool sync, string custName);

        public abstract Result EnableNumberGame(int id, bool enable, bool enableDownline);

        public abstract Result EnableLiveCasino(int id, bool enable, bool enableDownline);

        public abstract Result EnableESports(int id, bool enable, bool enableDownline);

        public abstract Result AutoMasterPt(int id, bool allow, int uplineId);

        public abstract Result ResetSecurityCode(int id, string code, bool force);

        public abstract Result EnableHrFixedOdds(int id, bool enable, bool enableDownline);

        public abstract Result UpdateGoldDeluxe(int id, bool enable, bool enableDownline, string custName, int userId);

        public abstract Result RemoveOtp(int custId);
    }
}