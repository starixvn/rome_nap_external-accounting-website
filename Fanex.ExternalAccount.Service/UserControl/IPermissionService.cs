namespace Fanex.ExternalAccount.Service.UserControl
{
    using System.Collections.Generic;
    using DTO.UserControl;

    public interface IPermissionService
    {
        IEnumerable<GroupPolicy> GetGroupPolicies(int functionId);

        IEnumerable<UserPolicy> GetUserPolicies(int functionId);
    }
}
