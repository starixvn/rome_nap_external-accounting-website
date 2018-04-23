namespace Fanex.ExternalAccount.Service.Impl.Login
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Helper;
    using DataAccess.Impl.Login;
    using DataAccess.Interface.Login;
    using DTO;
    using DTO.UserControl;
    using Interface.Login;
    using Interface.UserControl;
    using UserControl;

    public class LoginService : ILoginService
    {
        private readonly ILoginDao _loginDao;
        private readonly IGroupService groupService;

        public LoginService(ILoginDao loginDao = null, IGroupService groupService = null)
        {
            _loginDao = loginDao ?? new LoginDao();
            this.groupService = groupService ?? new GroupService();
        }

        public Result Login(string username, string password, Request request, out int custId, out int onuserId,
            out bool hasMultipleGroupsPermission)
        {
            Check.Argument.IsNotNull(username, "username");
            Check.Argument.IsNotEmpty(username, "username");

            Check.Argument.IsNotNull(password, "password");
            Check.Argument.IsNotEmpty(password, "password");

            custId = 0;
            onuserId = 0;
            hasMultipleGroupsPermission = true;

            var result = _loginDao.Login(username, password, request.Browser, request.IP, request.DomainName,
                request.CountryCode, request.Platform, out custId);

            if (result.IsSuccess)
            {
                _loginDao.Checkin(custId, request.NetSessionId, request.IP, out onuserId);

                hasMultipleGroupsPermission = _loginDao.CheckExternalGroup(custId, 11, "ExternalGroup");

                result.ResultData = groupService.GetGroupById((int)result.ResultData);
            }

            return result;
        }

        public Result Logout(int custId, string username)
        {
            Check.Argument.IsNotNegative(custId, "custId");
            Check.Argument.IsNotNull(username, "username");
            Check.Argument.IsNotEmpty(username, "username");

            return _loginDao.Logout(custId, username);
        }

        public bool IsKickedOut(int custId) => _loginDao.IsKickedOut(custId);

        public ICollection<Group> GroupList() => _loginDao.GroupList().OrderBy(o => o.GroupName).ToList();

        public bool CheckExternalGroup(int custId, int? funcId, string funcKey)
        {
            Check.Argument.IsNotNegative(custId, "custId");
            Check.Argument.IsNotNull(funcKey, "funcKey");
            Check.Argument.IsNotEmpty(funcKey, "funcKey");

            return _loginDao.CheckExternalGroup(custId, funcId, funcKey);
        }

        public DateTime GetDbTime() => _loginDao.GetDbTime();
    }
}