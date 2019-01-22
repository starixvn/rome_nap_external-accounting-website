namespace Fanex.ExternalAccount.Service.UserControl
{
    using System.Collections.Generic;
    using DataAccess.Criteria.UserControl;
    using DataAccess.UserControl;
    using DTO.UserControl;

    public class PermissionService : IPermissionService
    {
        private readonly IPermissionDao permissionDAO;

        public PermissionService(IPermissionDao permissionDAO = null)
        {
            this.permissionDAO = permissionDAO ?? new PermissionDao();
        }

        public IEnumerable<GroupPolicy> GetGroupPolicies(int functionId)
            => permissionDAO.GetGroupPolicies(new GetGroupPolicyCriteria(functionId));

        public IEnumerable<UserPolicy> GetUserPolicies(int functionId)
            => permissionDAO.GetUserPolicies(new GetUserPolicyCriteria(functionId));
    }
}
