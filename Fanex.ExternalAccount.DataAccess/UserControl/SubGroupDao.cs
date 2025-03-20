namespace Fanex.ExternalAccount.DataAccess.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Criteria.SubGroup;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.SubGroup;
    using Fanex.ExternalAccount.ViewModel.SubGroup;
    using Fanex.Data.Repository;
    using System.Data;

    public interface ISubGroupDao
    {
        SubGroupViewModel GetSubGroupCustomer(GetSubGroupCustomerCriteria criteria);

        Result UpdateSubGroupCustomer(UpdateCustomerSubGroupCriteria criteria);

        IEnumerable<SubGroupAdmin> GetSubGroupAdmin(GetSubGroupAdminCriteria criteria);

        Result UpdateSubGroupAdminPassword(UpdateSubGroupAdminPwdCriteria criteria);

        void ForceKickout(ForceKickoutAdminSubGroupCriteria criteria);
    }

    public class SubGroupDao : ISubGroupDao
    {
        private readonly IDynamicRepository dynamicRepository;

        public SubGroupDao(IDynamicRepository dynamicRepository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public void ForceKickout(ForceKickoutAdminSubGroupCriteria criteria)
        {
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     db.ExecuteNonQuery(criteria.GetSpParams());
            // }
            dynamicRepository.Execute(criteria);
        }

        public IEnumerable<SubGroupAdmin> GetSubGroupAdmin(GetSubGroupAdminCriteria criteria)
        {
            return dynamicRepository.Fetch<SubGroupAdmin>(criteria);
        }

        public SubGroupViewModel GetSubGroupCustomer(GetSubGroupCustomerCriteria criteria)
        {
            var result = new List<SubGroupCustomer>();
            var subGroups = criteria.SubGroup.Split(',').Select(groupName => new SubGroupStatus { GroupName = groupName, IsAssigned = false });

            var dataSet = dynamicRepository.FetchDataSet(criteria);
            if (dataSet != null)
            {
                var subGroupCustomers = dataSet.Tables[0].AsEnumerable();
                var assignedSubGroupCustomers = dataSet.Tables[1].AsEnumerable();
                
                foreach (var row in subGroupCustomers)
                {
                    var customer = new SubGroupCustomer
                    {
                        CustId = row.Field<int>("custid"),
                        Username = row.Field<string>("username"),
                        IsClosed = row.Field<bool>("closed"),
                        IsSuspended = row.Field<bool>("suspended"),
                        IsEnabled = row.Field<bool>("enabled"),
                        SubGroup = row.Field<string>("subgroup"),
                        SubGroupStatusList = subGroups.ToList()
                    };
                    result.Add(customer);
                }

                foreach (var row in assignedSubGroupCustomers)
                {
                    var customerId = row.Field<int>("custid");
                    var groupName = row.Field<string>("groupname");

                    var customer = result.FirstOrDefault(cust => cust.CustId == customerId);
                    if (customer != null)
                    {
                        customer.SubGroupStatusList.ForEach(subgroup =>
                            subgroup.IsAssigned = (subgroup.GroupName == groupName) || subgroup.IsAssigned);
                    }
                }
            }

            return new SubGroupViewModel
            {
                SubGroupCustomers = result,
                SubGroups = subGroups.Select(subGroup => subGroup.GroupName),
                PageIndex = criteria.PageIndex,
                TotalItems = criteria.TotalRows
            };
        }

        public Result UpdateSubGroupAdminPassword(UpdateSubGroupAdminPwdCriteria criteria)
        {
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     db.ExecuteNonQuery(criteria.GetSpParams());
            //     var outputParams = db.GetOuputParams();
            //
            //     return new Result(outputParams["errcode"], outputParams["errmsg"]);
            // }
            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public Result UpdateSubGroupCustomer(UpdateCustomerSubGroupCriteria criteria)
        {
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     db.ExecuteNonQuery(criteria.GetSpParams());
            //     var outputParams = db.GetOuputParams();
            //
            //     return new Result(outputParams["errcode"], outputParams["errmsg"]);
            // }
            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }
    }
}