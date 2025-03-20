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

#pragma warning disable S1192 // String literals should not be duplicated

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

        void EAS_WriteLog_UserLog_UserControl_Wrapper(int custId, string trackingValue, int userId, int duration, int actionId);
    }
    
    public class UserDao : IUserDao
    {
        private const int AdminSiteDBGroupId = 0;
        public IEnumerable<User> GetListUsers(string userName, int? groupId)
        {
            var result = new List<User>();

            using (var db = new ObjectDb("EAS_UserInfoSel"))
            {
                db.AddInParameter("GroupID", DbType.Int32, groupId);
                db.AddInParameter("Username", DbType.String, userName);

                using (var reader = db.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            result.Add(new User
                            {
                                CreatedDate = reader.ToString("CreatedDate"),
                                Creator = reader.ToString("Creator"),
                                Enabled = reader.ToBool("Enabled"),
                                GroupID = reader.ToInt("GroupID"),
                                UserID = reader.ToInt("CustId"),
                                Username = reader.ToString("Username"),
                                FirstName = reader.ToString("FirstName"),
                                LastName = reader.ToString("LastName"),
                                GroupName = reader.ToString("GroupName")
                            });
                        }
                    }
                }

                return result;
            }
        }

        public User GetUserById(int userId)
        {
            var result = new User();

            using (var db = new ObjectDb("EAS_UserInfoSelByUserID"))
            {
                db.AddInParameter("UserID", DbType.Int32, userId);

                using (var reader = db.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            result = new User
                            {
                                Enabled = reader.ToBool("Enabled"),
                                GroupID = reader.ToInt("GroupID"),
                                UserID = reader.ToInt("CustId"),
                                Username = reader.ToString("Username"),
                                FirstName = reader.ToString("FirstName"),
                                LastName = reader.ToString("LastName"),
                                GroupName = reader.ToString("GroupName")
                            };
                        }
                    }
                }

                return result;
            }
        }

        public Result ResetPassword(int adminId, int userId, string password, string ip)
        {
            using (var db = new ObjectDb("EAS_UserInfoUpdUserPwd"))
            {
                db.AddInParameter("UserID", DbType.Int32, adminId);
                db.AddInParameter("CustID", DbType.Int32, userId);
                db.AddInParameter("UserPwd", DbType.String, Cryptography.CfsEnCode(password));
                db.AddInParameter("IP", DbType.String, ip);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }

        public Result CreateUser(int adminId, string userName, int custId, string password, string firstName, string lastName,
            int groupId, string ip, out string logData)
        {
            using (var db = new ObjectDb("EAS_UserInfoAddNew"))
            {
                db.AddInParameter("UserID", DbType.Int32, adminId);
                db.AddInParameter("Username", DbType.String, userName);
                db.AddInParameter("UserPwd", DbType.String, Cryptography.CfsEnCode(password));
                db.AddInParameter("FirstName", DbType.String, firstName);
                db.AddInParameter("LastName", DbType.String, lastName);
                db.AddInParameter("GroupID", DbType.Int32, groupId);
                db.AddInParameter("IP", DbType.String, ip);
                db.AddInParameter("CustID", DbType.Int32, custId);
                db.AddOutParameter("LogData", DbType.String, -1);
                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();
                logData = (string)db.GetParameterValue("LogData");
                return GetResultParams(db.Database, db.DbCommand);
            }
        }

        public Result EditUser(int adminId, int userId, string firstName, string lastName, string ip, int groupId)
        {
            using (var db = new ObjectDb("EAS_UserInfoUpdAll"))
            {
                db.AddInParameter("UserID", DbType.Int32, adminId);
                db.AddInParameter("FirstName", DbType.String, firstName);
                db.AddInParameter("LastName", DbType.String, lastName);
                db.AddInParameter("GroupID", DbType.Int32, groupId);
                db.AddInParameter("IP", DbType.String, ip);
                db.AddInParameter("CustID", DbType.Int32, userId);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }

        public Result UpdateStatus(int adminId, int userId, bool active, string ip)
        {
            using (var db = new ObjectDb("EAS_UserInfoUpdStatus"))
            {
                db.AddInParameter("UserID", DbType.Int32, adminId);
                db.AddInParameter("CustID", DbType.Int32, userId);
                db.AddInParameter("Enabled", DbType.Boolean, active);
                db.AddInParameter("IP", DbType.String, ip);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }

        public Result CreateExistUser(int adminId, string userName, int groupId, string ip)
        {
            using (var db = new ObjectDb("EAS_UserInfoAddExistingUser"))
            {
                db.AddInParameter("userID", DbType.Int32, adminId);
                db.AddInParameter("Username", DbType.String, userName);
                db.AddInParameter("groupid", DbType.Int32, groupId);
                db.AddInParameter("IP", DbType.String, ip);
                db.AddOutParameter("custid", DbType.Int32, sizeof(int));

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }

        public IEnumerable<Function> GetUserFunction(int userId)
        {
            using (var db = new ObjectDb("EAS_UserPolicySel"))
            {
                var result = new List<Function>();

                db.AddInParameter("UserID", DbType.Int32, userId);

                using (var reader = db.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            result.Add(new Function
                            {
                                FuncID = reader.ToInt("FuncId"),
                                FuncName = reader.ToString("FuncName"),
                                FuncKey = reader.ToString("FuncKey"),
                                DisplayOrder = reader.ToInt("DisplayOrder"),
                                ParentID = reader.ToInt("ParentId"),
                                ParentName = reader.ToString("ParentName"),
                                Policy = (EPolicy)Convert.ToInt32(reader["UserPolicy"]),
                                GroupPolicy = (EPolicy)Convert.ToInt32(reader["GroupPolicy"]),
                                FuncDesc = reader.ToString("FuncDesc")
                            });
                        }
                    }
                }

                return result;
            }
        }

        public Result UpdateUserPolicy(int adminId, int userId, string oldPolicy, string policy, string ip)
        {
            using (var db = new ObjectDb("EAS_UserPolicyUpd"))
            {
                db.AddInParameter("userID", DbType.Int32, adminId);
                db.AddInParameter("OldUserPolicy", DbType.String, oldPolicy);
                db.AddInParameter("UserPolicy", DbType.String, policy);
                db.AddInParameter("IP", DbType.String, ip);
                db.AddInParameter("custid", DbType.Int32, userId);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }

        public Result Acc_CustomerIns_Register(string userName, out int custId)
        {
            using (var db = new ObjectDb("Acc_CustomerIns_Register"))
            {
                db.AddInParameter("username", DbType.String, userName);
                db.AddInParameter("sitedbgroupid", DbType.Int32, AdminSiteDBGroupId);
                db.AddInParameter("roleid", DbType.Int32, 6);
                db.AddOutParameter("custid", DbType.Int32, sizeof(int));

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                custId = Converter.ToInt(db.GetParameterValue("custid"), 0);
                
                return GetResultParams(db.Database, db.DbCommand);
            }
        }

        public Result EAS_eaUserInfo_Ins(int userId, int groupId, int adminId)
        {
            using (var db = new ObjectDb("EAS_eaUserInfo_Ins"))
            {
                db.AddInParameter("UserId", DbType.Int32, userId);
                db.AddInParameter("GroupId", DbType.Int32, groupId);
                db.AddInParameter("Creator", DbType.Int32, adminId);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();
                return GetResultParams(db.Database, db.DbCommand);
            }
        }

        public void Acc_CustomerInsUserInfo_Rollback(int custId)
        {
            using (var db = new ObjectDb("Acc_CustomerInsUserInfo_Rollback"))
            {
                db.AddInParameter("CustId", DbType.Int32, custId);
                db.ExecuteNonQuery();
            }
        }
        
        public void Acc_CustomerIns_Rollback(int custId)
        {
            using (var db = new ObjectDb("Acc_CustomerIns_Rollback"))
            {
                db.AddInParameter("CustId", DbType.Int32, custId);
                db.ExecuteNonQuery();
            }
        }

        public void EAS_WriteLog_UserLog_UserControl_Wrapper(int custId, string trackingValue, int userId, int duration, int actionId)
        {
            using (var db = new ObjectDb("EAS_WriteLog_UserLog_UserControl_Wrapper"))
            {
                db.AddInParameter("CustId", DbType.Int32, custId);
                db.AddInParameter("UserId", DbType.Int32, userId);
                db.AddInParameter("TrackingValue", DbType.String, trackingValue);
                db.AddInParameter("TypeID", DbType.Int32, 1);
                db.AddInParameter("ActionID", DbType.Int32, actionId);
                db.AddInParameter("Duration", DbType.Int32, duration);
                db.ExecuteNonQuery();
            }
        }
    }
#pragma warning restore S1192 // String literals should not be duplicated
}