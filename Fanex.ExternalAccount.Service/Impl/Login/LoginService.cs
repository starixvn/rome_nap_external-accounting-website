namespace Fanex.ExternalAccount.Service.Impl.Login
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.ExternalAccount.DataAccess.Impl.Login;
    using Fanex.ExternalAccount.DataAccess.Interface.Login;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Base;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.Login;
    using Fanex.ExternalAccount.DTO.UserControl;

    public class LoginService : ServiceBase, ILoginService
    {
        private readonly ILoginDao loginDao = null;

        public LoginService(ILoginDao loginDao = null)
        {
            this.loginDao = loginDao ?? new LoginDao();
        }

        #region ILoginService Members

        public DTO.Result Login(string username, string password, Request request, out int custId, out int onUserId, out bool isIntAcc)
        {
            Check.Argument.IsNotNull(username, "username");
            Check.Argument.IsNotEmpty(username, "username");

            Check.Argument.IsNotNull(password, "password");
            Check.Argument.IsNotEmpty(password, "password");

            custId = 0;
            onUserId = 0;
            isIntAcc = true;

            try
            {
                var result = loginDao.Login(username, password, request.Browser, request.IP, request.DomainName, request.CountryCode, request.Platform, out custId);

                if (result.IsSuccess)
                {
                    var result2 = loginDao.Checkin(custId, request.NetSessionId, request.IP, out onUserId);
                    isIntAcc = loginDao.CheckExternalGroup(custId, 11, "ExternalGroup");

                    result.ResultData = new GroupService().GetGroupById((int)result.ResultData);

                    return result;
                }

                return result;
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public DTO.Result Logout(int custId, string username)
        {
            Check.Argument.IsNotNegative(custId, "custId");
            Check.Argument.IsNotNull(username, "username");
            Check.Argument.IsNotEmpty(username, "username");

            try
            {
                return loginDao.Logout(custId, username);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public DTO.Result Recheckin(int custId, out bool isKickOut)
        {
            isKickOut = false;

            Check.Argument.IsNotNegative(custId, "custId");

            try
            {
                return loginDao.Recheckin(custId, out isKickOut);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public ICollection<Group> GroupList()
        {
            try
            {
                return loginDao.GroupList().OrderBy(o => o.GroupName).ToList();
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
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

                return loginDao.CheckExternalGroup(custId, funcId, funcKey);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return true;
        }

        #endregion ILoginService Members
    }
}