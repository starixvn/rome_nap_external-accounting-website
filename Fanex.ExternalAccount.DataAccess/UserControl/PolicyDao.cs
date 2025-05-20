namespace Fanex.ExternalAccount.DataAccess.UserControl
{
    using System.Collections.Generic;
    using Data;
    using DTO.UserControl;
    using Fanex.Data.Repository;
    using DTO;
    using Fanex.ExternalAccount.DataAccess.Criteria.UserControl;
    
    public interface IPolicyDao
    {
        IEnumerable<GroupPolicy> GetGroupPolicies(GetGroupPolicyCriteria criteria);

        IEnumerable<UserPolicy> GetUserPolicies(GetUserPolicyCriteria criteria);

        Result UpdateMultipleUserPolicy(UpdateMultipleUserPolicyCriteria criteria);

        Result UpdateMultipleGroupPolicy(UpdateMultipleGroupPolicyCriteria criteria);
    }
    
    public class PolicyDao : IPolicyDao
    {
        private readonly IDynamicRepository dynamicRepository;

        public PolicyDao(IDynamicRepository dynamicRepository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<GroupPolicy> GetGroupPolicies(GetGroupPolicyCriteria criteria)
        {
            return dynamicRepository.Fetch<GroupPolicy>(criteria);
        }

        public IEnumerable<UserPolicy> GetUserPolicies(GetUserPolicyCriteria criteria)
        {
            return dynamicRepository.Fetch<UserPolicy>(criteria);
        }

        public Result UpdateMultipleUserPolicy(UpdateMultipleUserPolicyCriteria criteria)
        {
            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public Result UpdateMultipleGroupPolicy(UpdateMultipleGroupPolicyCriteria criteria)
        {
            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }
    }
}