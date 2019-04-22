namespace Fanex.ExternalAccount.Service.UserControl
{
    using System.Collections.Generic;
    using DTO.UserControl;

    public interface IPolicyService
    {
        IEnumerable<HierarchicalFunction> GetHierarchicalFunctions();

        IEnumerable<GroupPolicy> GetGroupPolicies(int functionId);

        IEnumerable<UserPolicy> GetUserPolicies(int functionId);

        DTO.Result UpdateMultipleGroupPolicies(int functionId, string policy, int userId, string ip);

        DTO.Result UpdateMultipleUserPolicies(int functionId, string policy, int userId, string ip);
    }
}
