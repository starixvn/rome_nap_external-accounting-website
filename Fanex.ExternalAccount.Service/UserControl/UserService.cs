namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using DataAccess.Impl.UserControl;
    using DataAccess.Interface.UserControl;
    using DTO;
    using DTO.UserControl;
    using Interface.UserControl;
    using System.Collections.Generic;

    public class UserService : IUserService
    {
        private readonly IUserDao _userDao = new UserDao();

        public IEnumerable<User> GetListUsers(string userName, int? groupId)
            => _userDao.GetListUsers(userName, groupId);

        public User GetUserById(int userId)
            => _userDao.GetUserById(userId);

        public Result ResetPassword(int adminId, int userId, string password, string ip)
            => _userDao.ResetPassword(adminId, userId, password, ip);

        public Result CreateUser(int adminId, string userName, string password, string firstName, string lastName,
            int groupId, string ip)
            => _userDao.CreateUser(adminId, userName, password, firstName, lastName, groupId, ip);

        public Result EditUser(int adminId, int userId, string firstName, string lastName, string ip, int groupId)
            => _userDao.EditUser(adminId, userId, firstName, lastName, ip, groupId);

        public Result UpdateStatus(int adminId, int userId, bool active, string ip)
            => _userDao.UpdateStatus(adminId, userId, active, ip);

        public Result CreateExistUser(int adminId, string userName, int groupId, string ip)
            => _userDao.CreateExistUser(adminId, userName, groupId, ip);

        public IEnumerable<Function> GetUserFunction(int userId)
            => _userDao.GetUserFunction(userId);

        public Result UpdateUserPolicy(int adminId, int userId, string oldPolicy, string policy, string ip)
            => _userDao.UpdateUserPolicy(adminId, userId, oldPolicy, policy, ip);        
    }
}