namespace Fanex.ExternalAccount.Service.Impl.Login
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Base;
    using Common.Helper;
    using DataAccess.Impl.Login;
    using DataAccess.Interface.Login;
    using DTO;
    using DTO.UserControl;
    using Interface.Login;
    using UserControl;

    public class LoginService : ServiceBase, ILoginService
    {
        private readonly ILoginDao _loginDao;

        public LoginService(ILoginDao loginDao = null)
        {
            _loginDao = loginDao ?? new LoginDao();
        }

        public Result Login(string username, string password, Request request, out int custId, out int onuserId,
            out bool isIntAcc)
        {
            Check.Argument.IsNotNull(username, "username");
            Check.Argument.IsNotEmpty(username, "username");

            Check.Argument.IsNotNull(password, "password");
            Check.Argument.IsNotEmpty(password, "password");

            custId = 0;
            onuserId = 0;
            isIntAcc = true;

            var result = _loginDao.Login(username, password, request.Browser, request.IP, request.DomainName,
                request.CountryCode, request.Platform, out custId);

            if (result.IsSuccess)
            {
                _loginDao.Checkin(custId, request.NetSessionId, request.IP, out onuserId);

                isIntAcc = _loginDao.CheckExternalGroup(custId, 11, "ExternalGroup");

                result.ResultData = new GroupService().GetGroupById((int) result.ResultData);
            }

            return result;
        }

        public Result Logout(int custId, string username)
        {
            Check.Argument.IsNotNegative(custId, "custId");
            Check.Argument.IsNotNull(username, "username");
            Check.Argument.IsNotEmpty(username, "username");

            try
            {
                return _loginDao.Logout(custId, username);
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }

            return null;
        }

        public bool IsKickedOut(int custId)
        {
            var isKickOut = false;

            Check.Argument.IsNotNegative(custId, "custId");

            try
            {
                isKickOut = _loginDao.IsKickedOut(custId);
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }

            return isKickOut;
        }

        public ICollection<Group> GroupList()
        {
            try
            {
                return _loginDao.GroupList().OrderBy(o => o.GroupName).ToList();
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }

            return null;
        }

        public bool CheckExternalGroup(int custId, int? funcId, string funcKey)
        {
            try
            {
                Check.Argument.IsNotNegative(custId, "custId");
                Check.Argument.IsNotNull(funcKey, "funcKey");
                Check.Argument.IsNotEmpty(funcKey, "funcKey");

                return _loginDao.CheckExternalGroup(custId, funcId, funcKey);
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }

            return true;
        }

        public DateTime GetDBTime()
        {
            try
            {
                return _loginDao.GetDbTime();
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }

            return DateTime.Now;
        }
    }
}