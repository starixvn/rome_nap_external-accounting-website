namespace Fanex.ExternalAccount.DataAccess.Impl.UserControl
{
    using Common;
    using Common.Extensions;
    using Data;
    using DTO;
    using DTO.UserControl;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.UserControl.UserCRUD;
    using System.Linq;

#pragma warning disable S1192 // String literals should not be duplicated

    public interface IUserDao
    {
        IEnumerable<User> GetListUsers(string userName, int? groupId);

        User GetUserById(int userId);

        Result ResetPassword(int adminId, int userId, string password, string ip);

        Result CreateUser(int adminId, string userName, int custId, string password, string firstName, string lastName, int groupId, string ip, out string logData);

        Result EditUser(int adminId, int userId, string firstName, string lastName, string ip, int groupId);

        Result UpdateStatus(int adminId, int userId, bool active, string ip);

        Result CreateExistUser(int adminId, string userName, int groupId, string ip);

        IEnumerable<Function> GetUserFunction(int userId);

        Result UpdateUserPolicy(int adminId, int userId, string oldPolicy, string policy, string ip);

        Result RegisterUser(string userName, out int custId);

        Result AssignGroup(int userId, int groupId, int adminId);

        void Acc_CustomerInsUserInfo_Rollback(int custId);

        void Acc_CustomerIns_Rollback(int custId);

        void WriteLog(int custId, string trackingValue, int userId, int duration, int actionId);
    }

    public class UserDao : IUserDao
    {
        private const int AdminSiteDBGroupId = 0;

        private readonly IDynamicRepository dynamicRepository;

        public UserDao(IDynamicRepository dynamicRepository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<User> GetListUsers(string userName, int? groupId)
        {
            var criteria = new GetListUsersCriteria(userName, groupId);
            return dynamicRepository.Fetch<User>(criteria);
        }

        public User GetUserById(int userId)
        {
            var getUserByIdCriteria = new GetUserByIdCriteria(userId);
            return dynamicRepository.Fetch<User>(getUserByIdCriteria).FirstOrDefault();
        }

        public Result ResetPassword(int adminId, int userId, string password, string ip)
        {
            var criteria = new ResetPasswordCriteria
            {
                UserID = adminId,
                CustID = userId,
                UserPwd = Security.Cryptography.CfsEnCode(password),
                IP = ip
            };
            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public Result CreateUser(int adminId, string userName, int custId, string password, string firstName,
            string lastName, int groupId, string ip, out string logData)
        {
            var criteria = new EAS_UserInfoAddNewCriteria
            {
                UserId = adminId,
                Username = userName,
                UserPwd = Security.Cryptography.CfsEnCode(password),
                FirstName = firstName,
                LastName = lastName,
                GroupID = groupId,
                IP = ip,
                CustID = custId,
            };
            dynamicRepository.Execute(criteria);
            logData = criteria.LogData;
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public Result EditUser(int adminId, int userId, string firstName, string lastName, string ip, int groupId)
        {
            var criteria = new EditUserCriteria
            {
                UserID = adminId,
                CustID = userId,
                FirstName = firstName,
                LastName = lastName,
                GroupID = groupId,
                IP = ip,
            };

            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public Result UpdateStatus(int adminId, int userId, bool active, string ip)
        {
            var criteria = new UpdateStatusCriteria(adminId, userId, active, ip);
            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public Result CreateExistUser(int adminId, string userName, int groupId, string ip)
        {
            var criteria = new CreateExistUserCriteria(adminId, userName, groupId, ip);
            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public IEnumerable<Function> GetUserFunction(int userId)
        {
            var criteria = new GetUserFunctionCriteria { UserID = userId };
            var ds = dynamicRepository.FetchDataSet(criteria);
            
            if (ds == null || ds.Tables.Count == 0)
            {
                return Enumerable.Empty<Function>();
            }

            var userTable = ds.Tables[0];
            if (userTable.Rows.Count == 0)
            {
                return Enumerable.Empty<Function>();
            }
            
            var users = userTable.AsEnumerable()
                .Select(c => new Function
                {
                    FuncID = c.Field<int?>("FuncId") ?? 0,
                    FuncName = c.Field<string>("FuncName") ?? string.Empty,
                    FuncKey = c.Field<string>("FuncKey") ?? string.Empty,
                    DisplayOrder = c.Field<int?>("DisplayOrder") ?? 0,
                    ParentID = c.Field<int?>("ParentId") ?? 0,
                    ParentName = c.Field<string>("ParentName") ?? string.Empty,
                    Policy = c.Field<EPolicy?>("UserPolicy") ?? EPolicy.None,
                    GroupPolicy = c.Field<EPolicy?>("GroupPolicy") ?? EPolicy.None,
                    FuncDesc = c.Field<string>("FuncDesc") ?? string.Empty,
                    CreatedDate = c.Field<string>("CreatedDate") ?? string.Empty,
                    Creator = c.Field<int?>("Creator") ?? 0,
                    Modifier = c.Field<int?>("Modifier") ?? 0,
                    ModifiedDate = c.Field<string>("ModifiedDate") ?? string.Empty,
                    IsTest = c.Field<bool?>("IsTest") ?? false
                })
                .ToList();

            return users;
        }

        public Result UpdateUserPolicy(int adminId, int userId, string oldPolicy, string policy, string ip)
        {
            var criteria = new UpdateUserPolicyCriteria(adminId, userId, oldPolicy, policy, ip);
            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public Result RegisterUser(string userName, out int custId)
        {
            var registerUserCriteria = new RegisterUserCriteria(userName, AdminSiteDBGroupId, 6);
            dynamicRepository.Execute(registerUserCriteria);
            custId = registerUserCriteria.CustId;
            return new Result(registerUserCriteria.ErrCode, registerUserCriteria.ErrMsg);
        }

        public Result AssignGroup(int userId, int groupId, int adminId)
        {
            var assignUserGroupCriteria = new AssignGroupCriteria(userId, groupId, adminId);
            dynamicRepository.Execute(assignUserGroupCriteria);
            return new Result(assignUserGroupCriteria.ErrCode, assignUserGroupCriteria.ErrMsg);
        }

        public void Acc_CustomerInsUserInfo_Rollback(int custId)
            => dynamicRepository.Execute(new Acc_CustomerInsUserInfo_RollbackCriteria(custId));

        public void Acc_CustomerIns_Rollback(int custId)
            => dynamicRepository.Execute(new Acc_CustomerIns_RollbackCriteria(custId));


        public void WriteLog(int custId, string trackingValue, int userId, int duration, int actionId)
        {
            dynamicRepository.Execute(new WriteLogCriteria
            {
                CustId = custId,
                UserId = userId,
                TrackingValue = trackingValue,
                TypeID = 1,
                ActionID = actionId,
                Duration = duration
            });
        }
    }
#pragma warning restore S1192 // String literals should not be duplicated
}