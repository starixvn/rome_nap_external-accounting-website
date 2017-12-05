// --------------------------------------------------------------------------
// <copyright file="LoginService.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

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

    /// <summary>
    /// Login service.
    /// </summary>
    public class LoginService : ILoginService
    {
        private readonly ILoginDao _loginDao;

        public LoginService(ILoginDao loginDao = null)
        {
            this._loginDao = loginDao ?? new LoginDao();
        }

        #region ILoginService Members

        /// <summary>
        /// Processes login.
        /// </summary>
        /// <param name="username">Admin username.</param>
        /// <param name="password">Admin password.</param>
        /// <param name="request">Request object.</param>
        /// <param name="custId">Admin user id.</param>
        /// <param name="onuserId">Admin on user id.</param>
        /// <param name="isIntAcc">Is Internal account or not.</param>
        /// <returns>Result object.</returns>
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

        /// <summary>
        /// Logout session.
        /// </summary>
        /// <param name="custId">Admin user id.</param>
        /// <param name="username">Admin username.</param>
        /// <returns>Result object.</returns>
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

        /// <summary>
        /// Recheck in info.
        /// </summary>
        /// <param name="custId">Admin user id.</param>
        /// <param name="isKickOut">Is kicked out or not.</param>
        /// <returns>Result object.</returns>
        public DTO.Result Recheckin(int custId, out bool isKickOut)
        {
            isKickOut = false;

            Check.Argument.IsNotNegative(custId, "custId");

            try
            {
                return _loginDao.Recheckin(custId, out isKickOut);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets all groups.
        /// </summary>
        /// <returns>ICollection object.</returns>
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

        /// <summary>
        /// Checks whether an admin is external group or not.
        /// </summary>
        /// <param name="custId">Admin user id.</param>
        /// <param name="funcId">Function id.</param>
        /// <param name="funcKey">Function key.</param>
        /// <returns>Boolean object.</returns>
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

        /// <summary>
        /// Gets DB date time.
        /// </summary>
        /// <returns>DateTime object.</returns>
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