namespace Fanex.ExternalAccount.DataAccess.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO.UserControl;

    /// <summary>
    /// GroupDao
    /// </summary>
    public class GroupDao : DAOBase, IGroupDao
    {
        #region IGroupDao Members

        /// <summary>
        /// Gets the list group.
        /// </summary>
        /// <returns>IEnumerable<Group></returns>
        public IEnumerable<Group> GetListGroup()
        {
            var result = new List<Group>();

            using (var db = new ObjectDb("EAS_GroupInfoSel"))
            {
                using (var reader = db.ExecuteReader())
                {
                    while (reader != null && reader.Read())
                    {
                        result.Add(new Group()
                        {
                            GroupID = reader.ToInt("GroupID"),
                            GroupName = reader.To<string>("GroupName"),
                            NoUsers = reader.ToInt("NoUsers"),
                            Description = reader.To<string>("Description"),
                            Creator = reader.To<string>("Creator"),
                            CreatedDate = reader.To<string>("CreatedDate")
                        });
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets all group.
        /// </summary>
        /// <returns>IEnumerable<Group></returns>
        public IEnumerable<Group> GetAllGroup()
        {
            var result = new List<Group>();

            using (var db = new ObjectDb("EAS_GroupInfoSelAllGroup"))
            {
                using (var reader = db.ExecuteReader())
                {
                    while (reader != null && reader.Read())
                    {
                        result.Add(new Group()
                        {
                            GroupID = reader.ToInt("GroupID"),
                            GroupName = reader.To<string>("GroupName")
                        });
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets all group policy.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <returns>IEnumerable<Function></returns>
        public IEnumerable<Function> GetAllGroupPolicy(int groupId)
        {
            var result = new List<Function>();

            using (var db = new ObjectDb("EAS_GroupPolicySel"))
            {
                db.AddInParameter("GroupID", System.Data.DbType.Int32, groupId);

                using (var reader = db.ExecuteReader())
                {
                    while (reader != null && reader.Read())
                    {
                        result.Add(new Function()
                        {
                            FuncID = reader.ToInt("FuncID"),
                            FuncName = reader.To<string>("FuncName"),
                            DisplayOrder = reader.ToInt("DisplayOrder"),
                            ParentID = reader.ToInt("ParentID"),
                            ParentName = reader.To<string>("ParentName"),
                            GroupPolicy = (ePolicy)Convert.ToInt32(reader["Policy"])
                        });
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the group by id.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <returns>Group</returns>
        public Group GetGroupById(int groupId)
        {
            var result = new Group();

            using (var db = new ObjectDb("EAS_GroupInfoSelByGroupID"))
            {
                db.AddInParameter("GroupID", System.Data.DbType.Int32, groupId);

                using (var reader = db.ExecuteReader())
                {
                    while (reader != null && reader.Read())
                    {
                        return new Group()
                        {
                            GroupID = reader.ToInt("GroupID"),
                            GroupName = reader.To<string>("GroupName"),
                            Description = reader.To<string>("Description")
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Creates the group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="description">The description.</param>
        /// <param name="custId">The cust id.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="groupId">The group id.</param>
        /// <returns>Result</returns>
        public Result CreateGroup(string groupName, string description, int custId, string ip, out int groupId)
        {
            groupId = 0;

            using (var db = new ObjectDb("EAS_GroupInfoIns"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, custId);
                db.AddInParameter("GroupName", System.Data.DbType.String, groupName);
                db.AddInParameter("Description", System.Data.DbType.String, description);
                db.AddInParameter("IP", System.Data.DbType.String, ip);
                db.AddOutParameter("GroupID", System.Data.DbType.Int32, sizeof(int));

                this.AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                var result = this.GetResultParams(db.Database, db.DbCommand);

                if (result.IsSuccess)
                {
                    groupId = (int)db.GetParameterValue("GroupID");
                }

                return result;
            }
        }

        /// <summary>
        /// Edits the group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="description">The description.</param>
        /// <param name="custId">The cust id.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="groupId">The group id.</param>
        /// <returns>Result</returns>
        public Result EditGroup(string groupName, string description, int custId, string ip, int groupId)
        {
            using (var db = new ObjectDb("EAS_GroupInfoUpd"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, custId);
                db.AddInParameter("GroupID", System.Data.DbType.Int32, groupId);
                db.AddInParameter("GroupName", System.Data.DbType.String, groupName);
                db.AddInParameter("Description", System.Data.DbType.String, description);
                db.AddInParameter("IP", System.Data.DbType.String, ip);

                this.AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return this.GetResultParams(db.Database, db.DbCommand);
            }
        }

        /// <summary>
        /// Edits the group policy.
        /// </summary>
        /// <param name="oldGroupPolicy">The old group policy.</param>
        /// <param name="groupPolicy">The group policy.</param>
        /// <param name="custId">The cust id.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="groupId">The group id.</param>
        /// <returns>Result</returns>
        public Result EditGroupPolicy(string oldGroupPolicy, string groupPolicy, int custId, string ip, int groupId)
        {
            using (var db = new ObjectDb("EAS_GroupPolicyUpd"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, custId);
                db.AddInParameter("GroupID", System.Data.DbType.Int32, groupId);
                db.AddInParameter("OldGroupPolicy", System.Data.DbType.String, oldGroupPolicy);
                db.AddInParameter("GroupPolicy", System.Data.DbType.String, groupPolicy);
                db.AddInParameter("IP", System.Data.DbType.String, ip);

                this.AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return this.GetResultParams(db.Database, db.DbCommand);
            }
        }

        /// <summary>
        /// Deletes the group.
        /// </summary>
        /// <param name="groupID">The group ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <returns>Result</returns>
        public Result DeleteGroup(int groupID, int userID)
        {
            using (var db = new ObjectDb("EAS_GroupInfoDel"))
            {
                db.AddInParameter("GroupID", System.Data.DbType.Int32, groupID);
                db.AddInParameter("UserID", System.Data.DbType.Int32, userID);

                this.AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return this.GetResultParams(db.Database, db.DbCommand);
            }
        }

        #endregion IGroupDao Members
    }
}