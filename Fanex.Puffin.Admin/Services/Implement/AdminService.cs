using System;
using Fanex.Puffin.Admin.Criteria;
using Fanex.Puffin.Admin.Repositories.Impl;
using Fanex.Puffin.Admin.Repositories.Interface;
using Fanex.Puffin.Admin.Services.Interface;
using Fanex.Puffin.Core.Services.Impl;

namespace Fanex.Puffin.Admin.Services.Implement
{
    public class AdminService: IAdminService
    {
        IAdminRepository _adminRepository;

        public AdminService()
        {
            _adminRepository = new AdminRepository();
        }

        public bool CheckUserOnlineStatus(AccountingUserCheckOnlineStatusCriteria criteria)
        {
            var onlineStatus = false;
            try
            {
                onlineStatus = _adminRepository.CheckUserOnlineStatus(criteria);
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }
            return onlineStatus;
        }
    }
}
