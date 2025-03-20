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
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     return db.Query<GroupPolicy>(criteria.GetSpParams());
            // }

            return dynamicRepository.Fetch<GroupPolicy>(criteria);
        }

        public IEnumerable<UserPolicy> GetUserPolicies(GetUserPolicyCriteria criteria)
        {
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     return db.Query<UserPolicy>(criteria.GetSpParams());
            // }
            return dynamicRepository.Fetch<UserPolicy>(criteria);
        }

        public Result UpdateMultipleUserPolicy(UpdateMultipleUserPolicyCriteria criteria)
        {
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     db.ExecuteNonQuery(criteria.GetSpParams());
            //     var outputParams = db.GetOuputParams();
            //
            //     return new DTO.Result(outputParams["errcode"], outputParams["errmsg"]);
            // }

            dynamicRepository.Fetch<UserPolicy>(criteria);

            return new Result(criteria);
        }

        public Result UpdateMultipleGroupPolicy(UpdateMultipleGroupPolicyCriteria criteria)
        {
            dynamicRepository.Fetch<UserPolicy>(criteria);

            return new Result(criteria);
        }
    }
}