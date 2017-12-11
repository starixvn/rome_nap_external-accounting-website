namespace Fanex.ExternalAccount.Service.Impl.Login
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.ExternalAccount.DataAccess.Impl.Login;
    using Fanex.ExternalAccount.DataAccess.Interface.Login;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.Login;
    using Logging.Extensions.Web;

    public class LoginService : ILoginService
    {
        private readonly ILoginDao _loginDao;

        public LoginService(ILoginDao loginDao = null)
        {
            this._loginDao = loginDao ?? new LoginDao();
        }

        #region ILoginService Members

        public DTO.Result Login(string username, string password, Request request, out int custId, out int onuserId, out bool isIntAcc)
        {
            Check.Argument.IsNotNull(username, "username");
            Check.Argument.IsNotEmpty(username, "username");

            Check.Argument.IsNotNull(password, "password");
            Check.Argument.IsNotEmpty(password, "password");

            custId = 0;
            onuserId = 0;
            isIntAcc = true;

            var result = _loginDao.Login(username, password, request.Browser, request.IP, request.DomainName, request.CountryCode, request.Platform, out custId);

            if (result.IsSuccess)
            {
                _loginDao.Checkin(custId, request.NetSessionId, request.IP, out onuserId);

                isIntAcc = _loginDao.CheckExternalGroup(custId, 11, "ExternalGroup");

                result.ResultData = new GroupService().GetGroupById((int)result.ResultData);
            }

            return result;
        }

        public DTO.Result Logout(int custId, string username)
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
                ExceptionHandler.Handle(ex);
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
                ExceptionHandler.Handle(ex);
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
                ExceptionHandler.Handle(ex);
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
                ExceptionHandler.Handle(ex);
            }

            return true;
        }

        public DateTime GetDBTime()
        {
            try
            {
                return this._loginDao.GetDBTime();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return DateTime.Now;
        }

        #endregion ILoginService Members
    }
}