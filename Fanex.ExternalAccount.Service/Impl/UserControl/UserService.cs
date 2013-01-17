// -----------------------------------------------------------------------
// <copyright file="UserService.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DataAccess.Impl.UserControl;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Impl.Base;
    using Fanex.ExternalAccount.Service.Interface.UserControl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UserService : ServiceBase, IUserService
    {
        private readonly IUserDao userDao = new UserDao();

        #region IUserService Members

        public IEnumerable<User> GetListUsers(string userName, int? groupId)
        {
            try
            {
                return this.userDao.GetListUsers(userName, groupId);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public User GetUserById(int userId)
        {
            try
            {
                return this.userDao.GetUserById(userId);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public DTO.Result ResetPassword(int adminId, int userId, string password, string ip)
        {
            try
            {
                return this.userDao.ResetPassword(adminId, userId, password, ip);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public DTO.Result CreateUser(int adminId, string userName, string password, string firstName, string lastName, int groupId, string ip)
        {
            try
            {
                return this.userDao.CreateUser(adminId, userName, password, firstName, lastName, groupId, ip);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public DTO.Result EditUser(int adminId, int userId, string firstName, string lastName, string ip, int groupId)
        {
            try
            {
                return this.userDao.EditUser(adminId, userId, firstName, lastName, ip, groupId);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public DTO.Result UpdateStatus(int adminId, int userId, bool active, string ip)
        {
            try
            {
                return this.userDao.UpdateStatus(adminId, userId, active, ip);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public DTO.Result CreateExistUser(int adminId, string userName, int groupId, string ip)
        {
            try
            {
                return this.userDao.CreateExistUser(adminId, userName, groupId, ip);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public IEnumerable<Function> GetUserFunction(int userId)
        {
            try
            {
                return this.userDao.GetUserFunction(userId);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public DTO.Result UpdateUserPolicy(int adminId, int userId, string oldPolicy, string policy, string ip)
        {
            try
            {
                return this.userDao.UpdateUserPolicy(adminId, userId, oldPolicy, policy, ip);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        #endregion IUserService Members
    }
}