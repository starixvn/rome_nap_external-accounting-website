namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.Services.Interface;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;

    public abstract class BaseService : ILeftPanelService
    {
        public int Agent
        {
            get
            {
                return 2;
            }
        }

        public int Master
        {
            get
            {
                return 3;
            }
        }

        public int Member
        {
            get
            {
                return 1;
            }
        }

        public int Super
        {
            get
            {
                return 4;
            }
        }

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

        public abstract Result EnableCasino(int id, bool enable, bool enableDownline, bool synce, string custName);

        public abstract Result EnableNumberGame(int id, bool enable, bool enableDownline);

        public abstract Result EnableLiveCasino(int id, bool enable, bool enableDownline);

        public abstract Result EnableESports(int id, bool enable, bool enableDownline);

        public abstract Result AutoMasterPT(int id, bool allow, int uplineId);

        public abstract Result ResetSecurityCode(int id, string code, bool force);

        public abstract Result EnableHRFixedOdds(int id, bool enable, bool enableDownline);

        public abstract Result UpdateGoldDeluxe(int id, bool enable, bool enableDownline, string custName, int custId);

        public abstract Result RemoveOTP(int custId);
    }
}