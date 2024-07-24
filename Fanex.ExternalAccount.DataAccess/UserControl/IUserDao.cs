namespace Fanex.ExternalAccount.DataAccess.Interface.UserControl
{
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;

    public interface IUserDao
    {
        IEnumerable<User> GetListUsers(string userName, int? groupId);

        User GetUserById(int userId);

        Result ResetPassword(int adminId, int userId, string password, string ip);

        Result CreateUser(int adminId, string userName,int custId, string password, string firstName, string lastName, int groupId, string ip, out string logData);

        Result EditUser(int adminId, int userId, string firstName, string lastName, string ip, int groupId);

        Result UpdateStatus(int adminId, int userId, bool active, string ip);

        Result CreateExistUser(int adminId, string userName, int groupId, string ip);

        IEnumerable<Function> GetUserFunction(int userId);

        Result UpdateUserPolicy(int adminId, int userId, string oldPolicy, string policy, string ip);

        Result Acc_CustomerIns_Register(string userName, out int custId);

        Result EAS_eaUserInfo_Ins(int userId, int groupId, int adminId);

        void Acc_CustomerInsUserInfo_Rollback(int custId);

        void Acc_CustomerIns_Rollback(int custId);

        void EAS_WriteLog_UserLog_UserControl_Wrapper(int custId, string trackingValue, int userId, int duration);
    }
}