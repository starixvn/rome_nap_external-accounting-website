namespace Fanex.ExternalAccount.DataAccess.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Logging.Extensions.Database;

    /// <summary>
    /// Group Data object.
    /// </summary>
    public class GroupDao : DAOBase, IGroupDao
    {
        #region IGroupDao Members

        /// <summary>
        /// Gets the list group.
        /// </summary>
        /// <returns>IEnumerable object.</returns>
        public IEnumerable<Group> GetListGroup()
        {
            var result = new List<Group>();

            using (var db = new ObjectDb("EAS_GroupInfoSel"))
            {
                try
                {
                    using (var reader = db.ExecuteReader())
                    {
                        while (reader != null && reader.Read())
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
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
                
                return result;
            }
        }

        /// <summary>
        /// Gets all group.
        /// </summary>
        /// <returns>IEnumerable object.</returns>
        public IEnumerable<Group> GetAllGroup()
        {
            var result = new List<Group>();

            using (var db = new ObjectDb("EAS_GroupInfoSelAllGroup"))
            {
                try
                {
                    using (var reader = db.ExecuteReader())
                    {
                        while (reader != null && reader.Read())
                        {
                            result.Add(new Group
                            {
                                GroupID = reader.ToInt("GroupID"),
                                GroupName = reader.ToString("GroupName")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets all group policy.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <returns>IEnumerable object.</returns>
        public IEnumerable<Function> GetAllGroupPolicy(int groupId)
        {
            var result = new List<Function>();

            using (var db = new ObjectDb("EAS_GroupPolicySel"))
            {
                db.AddInParameter("GroupID", System.Data.DbType.Int32, groupId);

                try
                {
                    using (var reader = db.ExecuteReader())
                    {
                        while (reader != null && reader.Read())
                        {
                            result.Add(new Function
                            {
                                FuncID = reader.ToInt("FuncID"),
                                FuncKey = reader.ToString("FuncKey"),
                                FuncName = reader.ToString("FuncName"),
                                DisplayOrder = reader.ToInt("DisplayOrder"),
                                ParentID = reader.ToInt("ParentID"),
                                ParentName = reader.ToString("ParentName"),
                                GroupPolicy = (EPolicy)Convert.ToInt32(reader["Policy"]),
                                FuncDesc = reader.ToString("FuncDesc")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the group by id.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <returns>Group object.</returns>
        public Group GetGroupById(int groupId)
        {
            using (var db = new ObjectDb("EAS_GroupInfoSelByGroupID"))
            {
                db.AddInParameter("GroupID", System.Data.DbType.Int32, groupId);

                try
                {
                    using (var reader = db.ExecuteReader())
                    {
                        if (reader != null && reader.Read())
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
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }

            return null;
        }

        /// <summary>
        /// Creates the group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="description">The description.</param>
        /// <param name="custId">The customer id.</param>
        /// <param name="ip">The IP address.</param>
        /// <param name="groupId">The group id.</param>
        /// <returns>Result object.</returns>
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

                try
                {
                    db.ExecuteNonQuery();
                    var result = this.GetResultParams(db.Database, db.DbCommand);

                    if (result.IsSuccess)
                    {
                        groupId = (int)db.GetParameterValue("GroupID");
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }
        }

        /// <summary>
        /// Edits the group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="description">The description.</param>
        /// <param name="custId">The customer id.</param>
        /// <param name="ip">The IP address.</param>
        /// <param name="groupId">The group id.</param>
        /// <returns>Result object.</returns>
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

        /// <summary>
        /// Edits the group policy.
        /// </summary>
        /// <param name="oldGroupPolicy">The old group policy.</param>
        /// <param name="groupPolicy">The group policy.</param>
        /// <param name="custId">The customer id.</param>
        /// <param name="ip">The IP address.</param>
        /// <param name="groupId">The group id.</param>
        /// <returns>Result object.</returns>
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

        /// <summary>
        /// Deletes the group.
        /// </summary>
        /// <param name="groupID">The group ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <returns>Result object.</returns>
        public Result DeleteGroup(int groupID, int userId)
        {
            using (var db = new ObjectDb("EAS_GroupInfoDel"))
            {
                db.AddInParameter("GroupID", System.Data.DbType.Int32, groupID);
                db.AddInParameter("UserID", System.Data.DbType.Int32, userId);

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

        #endregion IGroupDao Members
    }
}