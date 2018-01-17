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

        DownlineList GetDownlineList(int custid, int pageSize, int pageIndex);

        IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId);

        IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId);
    }

    public class CustomerQueryServiceImpl : CustomerQueryService
    {
        private static readonly IReadOnlyDictionary<int, CustomerInfoQueryService> queryServiceInstances =
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
            this.queryServices = queryServices ?? queryServiceInstances;
        }

        public CustomerModel SearchCustomer(int actorId, string custName, string groupName = "ACCDDD")
        {
            var customerInfoDataSet = dataAccess.GetCustomerInfo(
                string.IsNullOrWhiteSpace(custName)
                    ? string.Empty
                    : custName,
                groupName);

            if (customerInfoDataSet == null || customerInfoDataSet.Tables.Count <= 0)
            {
                return null;
            }

            var roleId = Utils.ToInt32(customerInfoDataSet.Tables[0].Rows[0]["roleid"]);

            if (!queryServices.ContainsKey(roleId))
            {
                throw new ArgumentException($"RoleId {roleId} is out of range");
            }

            // TODO: queryService instance just need to call SetCustomSetting

            return queryServices[roleId].GetCustomerModel(actorId, custName, groupName, customerInfoDataSet);
        }

        public OnlineList GetOnlineList(int custId)
        {
            var onlineList = new OnlineList();

            foreach (DataRow row in dataAccess.GetOnlineList(custId))
            {
                var onlineIp = row.ToString("ip");
                onlineList.Onlines.Add(new OnlineInfo
                (
                    new ObjectInfo {Text = authorizationService.CanViewVNIP(onlineIp) ? onlineIp : string.Empty},
                    new ObjectInfo {Text = Utils.ToDateTime(row["reload_time"]).ToStringExt()}
                ));
            }

            return onlineList;
        }

        public DownlineList GetDownlineList(int custid, int pageSize, int pageIndex)
        {
            var dr = dataAccess.GetDownlineList(custid, pageSize, pageIndex);

            if (dr?.ResultSet == null || dr.ResultSet.Tables.Count <= 0)
            {
                return null;
            }

            var totalPages = Utils.ToInt32(dr.Outputs["totalpages"]);
            var downlines = dr.ResultSet.Tables[0];
            var model = new DownlineList();

            if (downlines?.Rows.Count <= 0)
            {
                return model;
            }

            AdDownlines(downlines, model);

            model.CurrentPage = pageIndex;

            AddPageItems(totalPages, model);

            return model;
        }

        public IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId) =>
            dataAccess.MasterAgentUsedCreditItems(custId, roleId);

        public IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId) =>
            dataAccess.MemberUsedCreditItems(custId);

        private static void AddPageItems(int totalPages, DownlineList model)
        {
            if (totalPages > 1)
            {
                for (var i = 1; i <= totalPages; i++)
                {
                    model.Pages.Add(new ObjectInfo {Value = i, Text = i.ToString()});
                }
            }
            else
            {
                model.Pages.Add(new ObjectInfo {Value = 1, Text = "1"});
            }
        }

        private static void AdDownlines(DataTable downlines, DownlineList model)
        {
            if (downlines?.Rows == null)
            {
                return;
            }

            foreach (DataRow item in downlines?.Rows)
            {
                model.Downlines.Add(new ObjectInfo {Text = item["username"].ToString()});
            }
        }
    }
}