namespace Fanex.ExternalAccount.DataAccess.UserControl
{
    using System.Collections.Generic;
    using DTO.UserControl;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.UserControl;
    

    public interface IPolicyDao
    {
        IEnumerable<GroupPolicy> GetGroupPolicies(GetGroupPolicyCriteria criteria);

        IEnumerable<UserPolicy> GetUserPolicies(GetUserPolicyCriteria criteria);

        DTO.Result UpdateMultipleUserPolicy(UpdateMultipleUserPolicyCriteria criteria);

        DTO.Result UpdateMultipleGroupPolicy(UpdateMultipleGroupPolicyCriteria criteria);
    }
}