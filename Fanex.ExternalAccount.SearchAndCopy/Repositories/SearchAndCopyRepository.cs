namespace Fanex.ExternalAccount.SearchAndCopy.Repositories
{
    using Common.Extensions;
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
        Result CloneSettingComm(CloneSettingComCriteria criteria, ref int Case, ref int HeadlineID_Old, ref int HeadlineID_New);

        Result CloneSettingExtraPT(CloneSettingExtraPTCriteria criteria);

        IEnumerable<SearchCustomerModel> SearchCustomer(string username, string groupName);

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

        public Result CloneSettingComm(CloneSettingComCriteria criteria, ref int Case, ref int HeadlineID_Old, ref int HeadlineID_New)
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
                HeadlineID_Old = Convert.ToInt32(outputs["old_headline"]);
                HeadlineID_New = Convert.ToInt32(outputs["new_headline"]);
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

        public Result UpdateCRMHeadlineInfo(UpdateCrmHeadlineInfoCriteria criteria)
        {
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