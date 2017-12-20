namespace Fanex.ExternalAccount.DataAccess.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Base;
    using Common;
    using Common.Extensions;
    using Data;
    using DTO;
    using DTO.UserControl;
    using Interface.UserControl;

    public class UserDao : DAOBase, IUserDao
    {
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

        public Result CreateUser(int adminId, string userName, string password, string firstName, string lastName,
            int groupId, string ip)
        {
            const int custId = 0;

            using (var db = new ObjectDb("EAS_UserInfoAddNew"))
            {
                db.AddInParameter("UserID", DbType.Int32, adminId);
                db.AddInParameter("Username", DbType.String, userName);
                db.AddInParameter("UserPwd", DbType.String, Cryptography.CfsEnCode(password));
                db.AddInParameter("FirstName", DbType.String, firstName);
                db.AddInParameter("LastName", DbType.String, lastName);
                db.AddInParameter("GroupID", DbType.Int32, groupId);
                db.AddInParameter("IP", DbType.String, ip);
                db.AddOutParameter("CustID", DbType.Int32, custId);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

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
                                Policy = (EPolicy) Convert.ToInt32(reader["UserPolicy"]),
                                GroupPolicy = (EPolicy) Convert.ToInt32(reader["GroupPolicy"]),
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
    }
}