namespace Fanex.ExternalAccount.Module.LeftPanel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Core.Authorization;
    using Core.Shared;
    using Customers;
    using DataAccesses;
    using Utilities;
    using ViewModels;

    public interface CustomerQueryService
    {
        CustomerModel SearchCustomer(int actorId, string custName, string groupName = "ACCDDD");

        OnlineList GetOnlineList(int custId);

        DataTable GetDownlineList(int custid, int pageSize, int pageIndex, out int totalPages);

        IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId);

        IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId);
    }

    public class CustomerQueryServiceImpl : CustomerQueryService
    {
        private static readonly IReadOnlyDictionary<int, CustomerInfoQueryService> queryServiceFactory =
            new Dictionary<int, CustomerInfoQueryService>
            {
                [Role.Super] = new SuperInfoQueryServiceImpl(),
                [Role.Master] = new MasterInfoQueryServiceImpl(),
                [Role.Agent] = new AgentInfoQueryServiceImpl(),
                [Role.Member] = new MemberInfoQueryServiceImpl()
            };

        private readonly AuthorizationService authorizationService;
        private readonly CustomerDataAccesses dataAccess;
        private readonly IReadOnlyDictionary<int, CustomerInfoQueryService> queryServices;

        public CustomerQueryServiceImpl(
            CustomerDataAccesses dataAccess = null,
            AuthorizationService authorizationService = null,
            IReadOnlyDictionary<int, CustomerInfoQueryService> queryServices = null)
        {
            this.dataAccess = dataAccess ?? new CustomerDataAccessesImpl();
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
            this.queryServices = queryServices ?? queryServiceFactory;
        }

        public CustomerModel SearchCustomer(int actorId, string custName, string groupName = "ACCDDD")
        {
            var customerInfo = dataAccess.GetCustomerInfo(
                string.IsNullOrWhiteSpace(custName)
                    ? string.Empty
                    : custName,
                groupName);

            if (customerInfo == null
                || customerInfo.Tables.Count <= 0)
            {
                return null;
            }

            var roleId = Utils.ToInt32(customerInfo.Tables[0].Rows[0]["roleid"]);

            if (!queryServices.ContainsKey(roleId))
            {
                throw new ArgumentOutOfRangeException($"RoleId {roleId} is out of range");
            }

            return queryServices[roleId].GetCustomer(actorId, custName, groupName, customerInfo);
        }

        public OnlineList GetOnlineList(int custId)
        {
            var model = new OnlineList();

            foreach (DataRow row in dataAccess.GetOnlineList(custId))
            {
                var onlineIp = row.ToString("ip");
                model.Onlines.Add(new OnlineInfo
                (
                    new ObjectInfo {Text = authorizationService.CanViewVNIP(onlineIp) ? onlineIp : string.Empty},
                    new ObjectInfo {Text = Utils.ToDateTime(row["reload_time"]).ToStringExt()}
                ));
            }

            return model;
        }

        public DataTable GetDownlineList(int custid, int pageSize, int pageIndex, out int totalPages)
        {
            totalPages = 1;
            var dr = dataAccess.GetDownlineList(custid, pageSize, pageIndex);

            if (dr?.ResultSet == null || dr.ResultSet.Tables.Count <= 0)
            {
                return null;
            }

            totalPages = Utils.ToInt32(dr.Outputs["totalpages"]);

            return dr.ResultSet.Tables[0];
        }

        public IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId) =>
            dataAccess.MasterAgentUsedCreditItems(custId, roleId);

        public IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId) =>
            dataAccess.MemberUsedCreditItems(custId);
    }
}