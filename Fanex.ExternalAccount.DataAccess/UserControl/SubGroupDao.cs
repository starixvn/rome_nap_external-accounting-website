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

    public interface ISubGroupDao
    {
        SubGroupViewModel GetSubGroupCustomer(GetSubGroupCustomerCriteria criteria);

        Result UpdateSubGroupCustomer(UpdateCustomerSubGroupCriteria criteria);

        IEnumerable<SubGroupAdmin> GetSubGroupAdmin(GetSubGroupAdminCriteria criteria);

        Result UpdateSubGroupAdminPassword(UpdateSubGroupAdminPwdCriteria criteria);

        void ForceKickout(ForceKickoutAdminSubGroupCriteria criteria);
    }
    
    public class SubGroupDao : DAOBase, ISubGroupDao
    {
        public void ForceKickout(ForceKickoutAdminSubGroupCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
            }
        }

        public IEnumerable<SubGroupAdmin> GetSubGroupAdmin(GetSubGroupAdminCriteria criteria)
        {
            var result = new List<SubGroupAdmin>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    while (reader.Read())
                    {
                        var admin = new SubGroupAdmin
                        {
                            CustId = reader.ToInt("custid"),
                            Username = reader.ToString("username"),
                        };

                        result.Add(admin);
                    }
                }
            }

            return result;
        }

        public SubGroupViewModel GetSubGroupCustomer(GetSubGroupCustomerCriteria criteria)
        {
            var result = new List<SubGroupCustomer>();
            var subGroups = criteria.SubGroup.Split(',').Select(groupName => new SubGroupStatus { GroupName = groupName, IsAssigned = false});

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    while (reader.Read())
                    {
                        var customer = new SubGroupCustomer
                        {
                            CustId = reader.ToInt("custid"),
                            Username = reader.ToString("username"),
                            IsClosed = reader.ToBool("closed"),
                            IsSuspended = reader.ToBool("suspended"),
                            IsEnabled = reader.ToBool("enabled"),
                            SubGroup = reader.ToString("subgroup"),
                            SubGroupStatusList = subGroups.ToList()
                        };
                        result.Add(customer);
                    }

                    reader.NextResult();
                    while (reader.Read())
                    {
                        var customerId = reader.ToInt("custid");
                        var groupName = reader.ToString("groupname");

                        var customer = result.FirstOrDefault(cust => cust.CustId == customerId);
                        customer.SubGroupStatusList.ForEach(subgroup => subgroup.IsAssigned = (subgroup.GroupName == groupName) || subgroup.IsAssigned);
                    }
                }

                var outputParams = db.GetOuputParams();

                return new SubGroupViewModel
                {
                    SubGroupCustomers = result,
                    SubGroups = subGroups.Select(subGroup => subGroup.GroupName),
                    PageIndex = outputParams["pageindex"] is DBNull ? 0 : Convert.ToInt16(outputParams["pageindex"]),
                    TotalItems = outputParams["totalrows"] is DBNull ? 0 : Convert.ToInt16(outputParams["totalrows"])
                };
            }
        }

        public Result UpdateSubGroupAdminPassword(UpdateSubGroupAdminPwdCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                var outputParams = db.GetOuputParams();

                return new Result(outputParams["errcode"], outputParams["errmsg"]);
            }
        }

        public Result UpdateSubGroupCustomer(UpdateCustomerSubGroupCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                var outputParams = db.GetOuputParams();

                return new Result(outputParams["errcode"], outputParams["errmsg"]);
            }
        }
    }
}
