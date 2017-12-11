namespace Fanex.ExternalAccount.DataAccess.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using Fanex.Common;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Logging.Extensions.Database;

    public class UserDao : DAOBase, IUserDao
    {
        #region IUserDao Members

        public IEnumerable<User> GetListUsers(string userName, int? groupId)
        {
            var result = new List<User>();

            using (var db = new ObjectDb("EAS_UserInfoSel"))
            {
                db.AddInParameter("GroupID", System.Data.DbType.Int32, groupId);
                db.AddInParameter("Username", System.Data.DbType.String, userName);

                try
                {
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
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }
        }

        public User GetUserById(int userId)
        {
            var result = new User();

            using (var db = new ObjectDb("EAS_UserInfoSelByUserID"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, userId);

                try
                {
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
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }
        }

        public DTO.Result ResetPassword(int adminId, int userId, string password, string ip)
        {
            var result = new DTO.Result();

            using (var db = new ObjectDb("EAS_UserInfoUpdUserPwd"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, adminId);
                db.AddInParameter("CustID", System.Data.DbType.Int32, userId);
                db.AddInParameter("UserPwd", System.Data.DbType.String, Cryptography.CfsEnCode(password));
                db.AddInParameter("IP", System.Data.DbType.String, ip);

                this.AddResultParams(db.Database, db.DbCommand);

                try
                {
                    db.ExecuteNonQuery();

                    return result = this.GetResultParams(db.Database, db.DbCommand);
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }
        }

        public DTO.Result CreateUser(int adminId, string userName, string password, string firstName, string lastName, int groupId, string ip)
        {
            var custId = 0;

            using (var db = new ObjectDb("EAS_UserInfoAddNew"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, adminId);
                db.AddInParameter("Username", System.Data.DbType.String, userName);
                db.AddInParameter("UserPwd", System.Data.DbType.String, Cryptography.CfsEnCode(password));
                db.AddInParameter("FirstName", System.Data.DbType.String, firstName);
                db.AddInParameter("LastName", System.Data.DbType.String, lastName);
                db.AddInParameter("GroupID", System.Data.DbType.Int32, groupId);
                db.AddInParameter("IP", System.Data.DbType.String, ip);
                db.AddOutParameter("CustID", System.Data.DbType.Int32, custId);

                this.AddResultParams(db.Database, db.DbCommand);

                try
                {
                    db.ExecuteNonQuery();

                    return this.GetResultParams(db.Database, db.DbCommand);
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }
        }

        public DTO.Result EditUser(int adminId, int userId, string firstName, string lastName, string ip, int groupId)
        {
            using (var db = new ObjectDb("EAS_UserInfoUpdAll"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, adminId);
                db.AddInParameter("FirstName", System.Data.DbType.String, firstName);
                db.AddInParameter("LastName", System.Data.DbType.String, lastName);
                db.AddInParameter("GroupID", System.Data.DbType.Int32, groupId);
                db.AddInParameter("IP", System.Data.DbType.String, ip);
                db.AddInParameter("CustID", System.Data.DbType.Int32, userId);

                this.AddResultParams(db.Database, db.DbCommand);

                try
                {
                    db.ExecuteNonQuery();

                    return this.GetResultParams(db.Database, db.DbCommand);
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }
        }

        public DTO.Result UpdateStatus(int adminId, int userId, bool active, string ip)
        {
            using (var db = new ObjectDb("EAS_UserInfoUpdStatus"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, adminId);
                db.AddInParameter("CustID", System.Data.DbType.Int32, userId);
                db.AddInParameter("Enabled", System.Data.DbType.Boolean, active);
                db.AddInParameter("IP", System.Data.DbType.String, ip);

                this.AddResultParams(db.Database, db.DbCommand);

                try
                {
                    db.ExecuteNonQuery();

                    return this.GetResultParams(db.Database, db.DbCommand);
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }
        }

        public DTO.Result CreateExistUser(int adminId, string userName, int groupId, string ip)
        {
            using (var db = new ObjectDb("EAS_UserInfoAddExistingUser"))
            {
                db.AddInParameter("userID", System.Data.DbType.Int32, adminId);
                db.AddInParameter("Username", System.Data.DbType.String, userName);
                db.AddInParameter("groupid", System.Data.DbType.Int32, groupId);
                db.AddInParameter("IP", System.Data.DbType.String, ip);
                db.AddOutParameter("custid", System.Data.DbType.Int32, sizeof(int));

                this.AddResultParams(db.Database, db.DbCommand);

                try
                {
                    db.ExecuteNonQuery();

                    return this.GetResultParams(db.Database, db.DbCommand);
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }
        }

        public IEnumerable<Function> GetUserFunction(int userId)
        {
            using (var db = new ObjectDb("EAS_UserPolicySel"))
            {
                var result = new List<Function>();

                db.AddInParameter("UserID", System.Data.DbType.Int32, userId);

                try
                {
                    using (var reader = db.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                result.Add(new Function
                                {
                                    FuncID = reader.ToInt("FuncID"),
                                    FuncName = reader.ToString("FuncName"),
                                    FuncKey = reader.ToString("FuncKey"),
                                    DisplayOrder = reader.ToInt("DisplayOrder"),
                                    ParentID = reader.ToInt("ParentID"),
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
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }
        }

        public DTO.Result UpdateUserPolicy(int adminId, int userId, string oldPolicy, string policy, string ip)
        {
            using (var db = new ObjectDb("EAS_UserPolicyUpd"))
            {
                db.AddInParameter("userID", System.Data.DbType.Int32, adminId);
                db.AddInParameter("OldUserPolicy", System.Data.DbType.String, oldPolicy);
                db.AddInParameter("UserPolicy", System.Data.DbType.String, policy);
                db.AddInParameter("IP", System.Data.DbType.String, ip);
                db.AddInParameter("custid", System.Data.DbType.Int32, userId);

                this.AddResultParams(db.Database, db.DbCommand);

                try
                {
                    db.ExecuteNonQuery();

                    return this.GetResultParams(db.Database, db.DbCommand);
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }
        }

        #endregion IUserDao Members
    }
}