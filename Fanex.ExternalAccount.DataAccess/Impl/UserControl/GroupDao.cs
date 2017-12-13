namespace Fanex.ExternalAccount.DataAccess.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Base;
    using Common.Extensions;
    using Data;
    using DTO;
    using DTO.UserControl;
    using Interface.UserControl;

    public class GroupDao : DAOBase, IGroupDao
    {
        public IEnumerable<Group> GetListGroup()
        {
            var result = new List<Group>();

            using (var db = new ObjectDb("EAS_GroupInfoSel"))
            {
                using (var reader = db.ExecuteReader())
                {
                    while (reader?.Read() == true)
                    {
                        result.Add(new Group
                        {
                            GroupID = reader.ToInt("GroupID"),
                            GroupName = reader.ToString("GroupName"),
                            NoUsers = reader.ToInt("NoUsers"),
                            Description = reader.ToString("Description"),
                            Creator = reader.ToString("Creator"),
                            CreatedDate = reader.ToString("CreatedDate")
                        });
                    }
                }

                return result;
            }
        }

        public IEnumerable<Group> GetAllGroup()
        {
            var result = new List<Group>();

            using (var db = new ObjectDb("EAS_GroupInfoSelAllGroup"))
            {
                using (var reader = db.ExecuteReader())
                {
                    while (reader?.Read() == true)
                    {
                        result.Add(new Group
                        {
                            GroupID = reader.ToInt("GroupID"),
                            GroupName = reader.ToString("GroupName")
                        });
                    }
                }

                return result;
            }
        }

        public IEnumerable<Function> GetAllGroupPolicy(int groupId)
        {
            var result = new List<Function>();

            using (var db = new ObjectDb("EAS_GroupPolicySel"))
            {
                db.AddInParameter("GroupID", DbType.Int32, groupId);

                using (var reader = db.ExecuteReader())
                {
                    while (reader?.Read() == true)
                    {
                        result.Add(new Function
                        {
                            FuncID = reader.ToInt("FuncID"),
                            FuncKey = reader.ToString("FuncKey"),
                            FuncName = reader.ToString("FuncName"),
                            DisplayOrder = reader.ToInt("DisplayOrder"),
                            ParentID = reader.ToInt("ParentID"),
                            ParentName = reader.ToString("ParentName"),
                            GroupPolicy = (EPolicy) Convert.ToInt32(reader["Policy"]),
                            FuncDesc = reader.ToString("FuncDesc")
                        });
                    }
                }
            }

            return result;
        }

        public Group GetGroupById(int groupId)
        {
            using (var db = new ObjectDb("EAS_GroupInfoSelByGroupID"))
            {
                db.AddInParameter("GroupID", DbType.Int32, groupId);

                using (var reader = db.ExecuteReader())
                {
                    if (reader?.Read() == true)
                    {
                        return new Group
                        {
                            GroupID = reader.ToInt("GroupID"),
                            GroupName = reader.ToString("GroupName"),
                            Description = reader.ToString("Description")
                        };
                    }
                }
            }

            return null;
        }

        public Result CreateGroup(string groupName, string description, int custId, string ip, out int groupId)
        {
            groupId = 0;

            using (var db = new ObjectDb("EAS_GroupInfoIns"))
            {
                db.AddInParameter("UserID", DbType.Int32, custId);
                db.AddInParameter("GroupName", DbType.String, groupName);
                db.AddInParameter("Description", DbType.String, description);
                db.AddInParameter("IP", DbType.String, ip);
                db.AddOutParameter("GroupID", DbType.Int32, sizeof(int));

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                var result = GetResultParams(db.Database, db.DbCommand);

                if (result.IsSuccess)
                {
                    groupId = (int) db.GetParameterValue("GroupID");
                }

                return result;
            }
        }

        public Result EditGroup(string groupName, string description, int custId, string ip, int groupId)
        {
            using (var db = new ObjectDb("EAS_GroupInfoUpd"))
            {
                db.AddInParameter("UserID", DbType.Int32, custId);
                db.AddInParameter("GroupID", DbType.Int32, groupId);
                db.AddInParameter("GroupName", DbType.String, groupName);
                db.AddInParameter("Description", DbType.String, description);
                db.AddInParameter("IP", DbType.String, ip);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }

        public Result EditGroupPolicy(string oldGroupPolicy, string groupPolicy, int custId, string ip, int groupId)
        {
            using (var db = new ObjectDb("EAS_GroupPolicyUpd"))
            {
                db.AddInParameter("UserID", DbType.Int32, custId);
                db.AddInParameter("GroupID", DbType.Int32, groupId);
                db.AddInParameter("OldGroupPolicy", DbType.String, oldGroupPolicy);
                db.AddInParameter("GroupPolicy", DbType.String, groupPolicy);
                db.AddInParameter("IP", DbType.String, ip);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }

        public Result DeleteGroup(int groupId, int userId)
        {
            using (var db = new ObjectDb("EAS_GroupInfoDel"))
            {
                db.AddInParameter("GroupID", DbType.Int32, groupId);
                db.AddInParameter("UserID", DbType.Int32, userId);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }
    }
}