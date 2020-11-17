namespace Fanex.ExternalAccount.Service.UserControl
{
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DataAccess.Criteria.SubGroup;
    using Fanex.ExternalAccount.DataAccess.UserControl;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.SubGroup;
    using Fanex.ExternalAccount.ViewModel.SubGroup;

    public class SubGroupService : ISubGroupService
    {
        private readonly ISubGroupDao _subGroupDao;

        public SubGroupService()
        {
            _subGroupDao = new SubGroupDao();
        }

        public IEnumerable<SubGroupAdmin> GetListAdminSubGroup(string groupName)
        {
            var criteria = new GetSubGroupAdminCriteria
            {
                SubGroup = groupName
            };

            return _subGroupDao.GetSubGroupAdmin(criteria);
        }

        public SubGroupViewModel GetListCustomerSubGroup(string groupName, string subGroupName, string username, int pageSize, int pageIndex)
        {
            var criteria = new GetSubGroupCustomerCriteria
            {
                GroupName = groupName,
                SubGroup = subGroupName,
                Username = username,
                PageSize = pageSize,
                PageIndex = pageIndex
            };

            return _subGroupDao.GetSubGroupCustomer(criteria);
        }

        public Result UpdateCustomerSubGroup(string subGroupName, string custIds, bool isAssigned, int userId, string ip6Address)
        {
            var criteria = new UpdateCustomerSubGroupCriteria
            {
                GroupName = subGroupName,
                CustList = custIds,
                IsCheck = isAssigned,
                UserId = userId,
                IP = ip6Address
            };

            return _subGroupDao.UpdateSubGroupCustomer(criteria);
        }

        public Result UpdateSubGroupAdminPassword(int custId, string password, int userId, string ipAddress)
        {
            var criteria = new UpdateSubGroupAdminPwdCriteria
            {
                CustID = custId,
                UserPwd = password,
                UserID = userId,
                IP = ipAddress
            };

            return _subGroupDao.UpdateSubGroupAdminPassword(criteria);
        }
    }
}
