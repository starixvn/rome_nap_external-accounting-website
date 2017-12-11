namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DataAccess.Impl.UserControl;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Logging.Extensions.Web;

    public class UserService : IUserService
    {
        private readonly IUserDao _userDao = new UserDao();

        #region IUserService Members

        public IEnumerable<User> GetListUsers(string userName, int? groupId)
        {
            try
            {
                return this._userDao.GetListUsers(userName, groupId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public User GetUserById(int userId)
        {
            try
            {
                return this._userDao.GetUserById(userId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public DTO.Result ResetPassword(int adminId, int userId, string password, string ip)
        {
            try
            {
                return this._userDao.ResetPassword(adminId, userId, password, ip);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public DTO.Result CreateUser(int adminId, string userName, string password, string firstName, string lastName, int groupId, string ip)
        {
            try
            {
                return this._userDao.CreateUser(adminId, userName, password, firstName, lastName, groupId, ip);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public DTO.Result EditUser(int adminId, int userId, string firstName, string lastName, string ip, int groupId)
        {
            try
            {
                return this._userDao.EditUser(adminId, userId, firstName, lastName, ip, groupId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public DTO.Result UpdateStatus(int adminId, int userId, bool active, string ip)
        {
            try
            {
                return this._userDao.UpdateStatus(adminId, userId, active, ip);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public DTO.Result CreateExistUser(int adminId, string userName, int groupId, string ip)
        {
            try
            {
                return this._userDao.CreateExistUser(adminId, userName, groupId, ip);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public IEnumerable<Function> GetUserFunction(int userId)
        {
            try
            {
                return this._userDao.GetUserFunction(userId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public DTO.Result UpdateUserPolicy(int adminId, int userId, string oldPolicy, string policy, string ip)
        {
            try
            {
                return this._userDao.UpdateUserPolicy(adminId, userId, oldPolicy, policy, ip);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        #endregion IUserService Members
    }
}