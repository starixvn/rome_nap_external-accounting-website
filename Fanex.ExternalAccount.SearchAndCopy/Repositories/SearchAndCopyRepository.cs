namespace Fanex.ExternalAccount.SearchAndCopy.Repositories
{
    using Fanex.Data;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.SearchAndCopy.Criteria;
    using Fanex.ExternalAccount.SearchAndCopy.Models;
    using Fanex.NAP.Framework.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ISearchAndCopyRepository
    {
        Result CloneSettingComm(CloneSettingComCriteria criteria, ref int Case, ref int HeadlineID_Old);

        Result CloneSettingExtraPT(CloneSettingExtraPTCriteria criteria);

        IEnumerable<SearchCustomerModel> SearchCustomer(string username, string groupName);
        void UpdateCRMHeadlineInfo(UpdateCRMHeadlineInfoCriteria criteria);
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

        public Result CloneSettingComm(CloneSettingComCriteria criteria, ref int Case, ref int HeadlineID_Old)
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
                Case = Convert.ToInt32(outputs["case"]);
                HeadlineID_Old = Convert.ToInt32(outputs["headlineid_old"]);
                return new Result
                {
                    ErrorCode = outputs["errcode"].ToString(),
                    ErrorMessage = outputs["errmsg"].ToString(),
                    ResultData = outputs["custid"].ToString()
                };
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
                return new Result
                {
                    ErrorCode = outputs["errcode"].ToString(),
                    ErrorMessage = outputs["errmsg"].ToString()
                };
            }
        }

        public IEnumerable<SearchCustomerModel> SearchCustomer(string username, string groupName)
        {
            var criteria = new SearchCustomerCriteria(username, groupName);
            if (!criteria.IsValid())
            {
                return Enumerable.Empty<SearchCustomerModel>();
            }

            return dynamicRepository.Fetch<SearchCustomerModel>(criteria);
        }
        public void UpdateCRMHeadlineInfo(UpdateCRMHeadlineInfoCriteria criteria)
        {            
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AssignParamValues(criteria.GetSpParams());
                objDb.ExecuteNonQuery();
            }
        }
    }
}
