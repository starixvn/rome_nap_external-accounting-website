namespace Fanex.ExternalAccount.DataAccess.UserControl
{
    using System.Collections.Generic;
    using DTO.UserControl;
    using NAP.Framework.Data;

    public interface IPolicyDao
    {
        IEnumerable<GroupPolicy> GetGroupPolicies(ICriteria criteria);

        IEnumerable<UserPolicy> GetUserPolicies(ICriteria criteria);

        DTO.Result UpdatePolicies(ICriteria criteria);
    }
}
