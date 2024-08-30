namespace Fanex.ExternalAccount.SearchAndCopy.Repositories
{
    using Common.Extensions;
    using Fanex.Data;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.SearchAndCopy.Criteria;
    using Fanex.ExternalAccount.SearchAndCopy.Models;
    using Fanex.NAP.Framework.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface ISearchAndCopyRepository
    {
        Result ValidateSettingComm(ValidateCloneSettingComCriteria criteria);

        Result RegisterCustomer(RegisterCustomerCriteria criteria);

        string GetGetGroupParentNameByGroupName(GetGroupParentNameByGroupNameCriteria criteria);

        void RollBackCustomer(RollbackCustomerInsertCriteria criteria);

        Result CloneSettingComm(CloneSettingComCriteria criteria);

        Result CloneSettingExtraPT(CloneSettingExtraPTCriteria criteria);

        IEnumerable<SearchCustomerModel> SearchCustomer(string username, string groupName, int custDBGroupId);

        Result UpdateCRMHeadlineInfo(UpdateCrmHeadlineInfoCriteria criteria);

        IEnumerable<SuperPrefix> GetSuperPrefixes(ICriteria criteria);
    }

    public class SearchAndCopyRepository : ISearchAndCopyRepository
    {
        private readonly IDynamicRepository dynamicRepository;

        public SearchAndCopyRepository()
            : this(null)
        {
        }

        public SearchAndCopyRepository(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public Result ValidateSettingComm(ValidateCloneSettingComCriteria criteria)
        {
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AssignParamValues(criteria.GetSpParams());
                objDb.ExecuteNonQuery();
                var outputs = objDb.GetOuputParams();
                return new Result(outputs["errcode"].ToString(), outputs["errmsg"].ToString());
            }
        }

        public Result RegisterCustomer(RegisterCustomerCriteria criteria)
        {
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AssignParamValues(criteria.GetSpParams());
                objDb.ExecuteNonQuery();
                var outputs = objDb.GetOuputParams();
                return new Result(outputs["errcode"].ToString(), outputs["errmsg"].ToString(), new { NewCustId = Convert.ToInt32(outputs["custid"]) });
            }
        }

        public string GetGetGroupParentNameByGroupName(GetGroupParentNameByGroupNameCriteria criteria)
        {   
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var reader = objDb.ExecuteReader(criteria.GetSpParams());
                ArrayList groupNames = new ArrayList();
                while (reader.Read())
                {
                    groupNames.Add(reader["GroupName"].ToString());
                }

                return string.Join(",", groupNames.ToArray());
            }
        }

        public Result CloneSettingComm(CloneSettingComCriteria criteria)
        {
            if (!criteria.IsValid())
            {
                return new Result(1, "Invalid operation");
            }
            
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AssignParamValues(criteria.GetSpParams());
                objDb.ExecuteNonQuery();
                var outputs = objDb.GetOuputParams();
                return new Result(
                    outputs["errcode"].ToString(), 
                    outputs["errmsg"].ToString(),
                    new
                    {
                        criteria.CustId,
                        Case = Convert.ToInt32(outputs["case"]),
                        HeadlineID_Old = Convert.ToInt32(outputs["old_headline"]),
                        HeadlineID_New = Convert.ToInt32(outputs["new_headline"])
                    });
            }
        }

        public void RollBackCustomer(RollbackCustomerInsertCriteria criteria)
        {
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AssignParamValues(criteria.GetSpParams());
                objDb.ExecuteNonQuery();
            }
        }

        public Result CloneSettingExtraPT(CloneSettingExtraPTCriteria criteria)
        {
            if (!criteria.IsValid())
            {
                return new Result(1, "Invalid operation");
            }

            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AssignParamValues(criteria.GetSpParams());
                objDb.ExecuteNonQuery();
                var outputs = objDb.GetOuputParams();
                return new Result(outputs["errcode"].ToString(), outputs["errmsg"].ToString());
            }
        }

        public IEnumerable<SearchCustomerModel> SearchCustomer(string username, string groupName, int custDBGroupId)
        {
            var criteria = new SearchCustomerCriteria(username, groupName, custDBGroupId);
            if (!criteria.IsValid())
            {
                return Enumerable.Empty<SearchCustomerModel>();
            }

            return dynamicRepository.Fetch<SearchCustomerModel>(criteria);
        }

        public Result UpdateCRMHeadlineInfo(UpdateCrmHeadlineInfoCriteria criteria)
        {
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AssignParamValues(criteria.GetSpParams());
                objDb.ExecuteNonQuery();
                var outputs = objDb.GetOuputParams();
                return new Result(outputs["errcode"].ToString(), outputs["errmsg"].ToString());
            }
        }

        public IEnumerable<SuperPrefix> GetSuperPrefixes(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    while (reader.Read())
                    {
                        yield return new SuperPrefix
                        {
                            GroupName = reader.ToString("GroupName"),
                            Prefix = reader.ToString("Prefix"),
                            AllowNewSuper = reader.ToBool("AllowNewSuper")
                        };
                    }
                }
            }
        }
    }
}