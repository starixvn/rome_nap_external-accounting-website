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
#pragma warning disable S1192 // String literals should not be duplicated
                            GroupID = reader.ToInt("GroupID"),
                            GroupName = reader.ToString("GroupName"),
                            NoUsers = reader.ToInt("NoUsers"),
                            Description = reader.ToString("Description"),
                            Creator = reader.ToString("Creator"),
                            CreatedDate = reader.ToString("CreatedDate")
#pragma warning restore S1192 // String literals should not be duplicated
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

        public IEnumerable<Group> GetExternalGroup()
        {
            using (var db = new ObjectDb("EAS_GroupInfoSelExternalGroup"))
            {
                using (var reader = db.ExecuteReader())
                {
                    while (reader?.Read() == true)
                    {
                        yield return new Group
                        {
                            GroupID = reader.ToInt("GroupID"),
                            GroupName = reader.To<string>("GroupName")
                        };
                    }
                }
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
                            FuncID = reader.ToInt("FuncId"),
                            FuncKey = reader.ToString("FuncKey"),
                            FuncName = reader.ToString("FuncName"),
                            DisplayOrder = reader.ToInt("DisplayOrder"),
                            ParentID = reader.ToInt("ParentId"),
                            ParentName = reader.ToString("ParentName"),
                            GroupPolicy = (EPolicy)Convert.ToInt32(reader["Policy"]),
                            FuncDesc = reader.ToString("FuncDesc")
                        });
                    }
                }
            }

            return result;
        }

        public IEnumerable<Group> GetGroupById(int groupId, byte mode)
        {
            using (var db = new ObjectDb("EAS_GroupInfoSelByGroupID"))
            {
                db.AddInParameter("GroupID", DbType.Int32, groupId);
                db.AddInParameter("Mode", DbType.Byte, mode);

                using (var reader = db.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return GetGroup(reader);
                    }
                }
            }
        }

        private static Group GetGroup(IDataReader reader)
            => new Group
            {
                GroupID = reader.ToInt("GroupID"),
                GroupName = reader.ToString("GroupName"),
                Description = reader.ToString("Description"),
                ParentGroupName = reader.ToString("ParentGroup")
            };

        public Result CreateGroup(string groupName, string description, int parentId, int custId, string ip)
        {
            using (var db = new ObjectDb("EAS_GroupInfoIns"))
            {
#pragma warning disable S1192 // String literals should not be duplicated
                db.AddInParameter("UserID", DbType.Int32, custId);
#pragma warning restore S1192 // String literals should not be duplicated
                db.AddInParameter("GroupName", DbType.String, groupName);
                db.AddInParameter("Description", DbType.String, description);
                db.AddInParameter("ParentID", DbType.Int32, parentId);
                db.AddInParameter("IP", DbType.String, ip);
                db.AddOutParameter("GroupID", DbType.Int32, sizeof(int));

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
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