// -----------------------------------------------------------------------
// <copyright file="UserService.cs" company="Nexcel Solution Co.Ltd">
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
        private readonly IUserDao _userDao = new UserDao();

        #region IUserService Members

        /// <summary>
        /// Gets user list.
        /// </summary>
        /// <param name="userName">Customer username.</param>
        /// <param name="groupId">Admin group id.</param>
        /// <returns>IEnumerable object.</returns>
        public IEnumerable<User> GetListUsers(string userName, int? groupId)
        {
            try
            {
                return this._userDao.GetListUsers(userName, groupId);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets user by id.
        /// </summary>
        /// <param name="userId">Admin user id.</param>
        /// <returns>User object.</returns>
        public User GetUserById(int userId)
        {
            try
            {
                return this._userDao.GetUserById(userId);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        /// <summary>
        /// Resets password.
        /// </summary>
        /// <param name="adminId">Admin user id.</param>
        /// <param name="userId">Customer user id.</param>
        /// <param name="password">Password characters.</param>
        /// <param name="ip">IP address.</param>
        /// <returns>Result object.</returns>
        public DTO.Result ResetPassword(int adminId, int userId, string password, string ip)
        {
            try
            {
                return this._userDao.ResetPassword(adminId, userId, password, ip);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex, "ExternalAccount", false);
            }

            return null;
        }

        /// <summary>
        /// Creates new user.
        /// </summary>
        /// <param name="adminId">Admin user id.</param>
        /// <param name="userName">Customer username.</param>
        /// <param name="password">Password characters.</param>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="groupId">Admin group id.</param>
        /// <param name="ip">IP address.</param>
        /// <returns>Result object.</returns>
        public DTO.Result CreateUser(int adminId, string userName, string password, string firstName, string lastName, int groupId, string ip)
        {
            try
            {
                return this._userDao.CreateUser(adminId, userName, password, firstName, lastName, groupId, ip);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex, "ExternalAccount", false);
            }

            return null;
        }

        /// <summary>
        /// Edits user info.
        /// </summary>
        /// <param name="adminId">Admin user id.</param>
        /// <param name="userId">Customer user id.</param>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="ip">IP address.</param>
        /// <param name="groupId">Admin group id.</param>
        /// <returns>Result object.</returns>
        public DTO.Result EditUser(int adminId, int userId, string firstName, string lastName, string ip, int groupId)
        {
            try
            {
                return this._userDao.EditUser(adminId, userId, firstName, lastName, ip, groupId);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        /// <summary>
        /// Update user status.
        /// </summary>
        /// <param name="adminId">Admin user id.</param>
        /// <param name="userId">Customer user id.</param>
        /// <param name="active">Active status.</param>
        /// <param name="ip">IP address.</param>
        /// <returns>Result object.</returns>
        public DTO.Result UpdateStatus(int adminId, int userId, bool active, string ip)
        {
            try
            {
                return this._userDao.UpdateStatus(adminId, userId, active, ip);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        /// <summary>
        /// Creates external user from existed user.
        /// </summary>
        /// <param name="adminId">Admin user id.</param>
        /// <param name="userName">Customer username.</param>
        /// <param name="groupId">Admin group id.</param>
        /// <param name="ip">IP address.</param>
        /// <returns>Result object.</returns>
        public DTO.Result CreateExistUser(int adminId, string userName, int groupId, string ip)
        {
            try
            {
                return this._userDao.CreateExistUser(adminId, userName, groupId, ip);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets user functions.
        /// </summary>
        /// <param name="userId">Customer id.</param>
        /// <returns>IEnumerable object.</returns>
        public IEnumerable<Function> GetUserFunction(int userId)
        {
            try
            {
                return this._userDao.GetUserFunction(userId);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        /// <summary>
        /// Updates user policy.
        /// </summary>
        /// <param name="adminId">Admin user id.</param>
        /// <param name="userId">Customer id.</param>
        /// <param name="oldPolicy">Old policy.</param>
        /// <param name="policy">New policy.</param>
        /// <param name="ip">IP address.</param>
        /// <returns>Result object.</returns>
        public DTO.Result UpdateUserPolicy(int adminId, int userId, string oldPolicy, string policy, string ip)
        {
            try
            {
                return this._userDao.UpdateUserPolicy(adminId, userId, oldPolicy, policy, ip);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        #endregion IUserService Members
    }
}