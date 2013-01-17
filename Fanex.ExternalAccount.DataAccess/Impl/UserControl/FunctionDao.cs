namespace Fanex.ExternalAccount.DataAccess.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO.UserControl;

    /// <summary>
    /// FunctionDao
    /// </summary>
    public class FunctionDao : DAOBase, IFunctionDao
    {
        #region IFunctionDao Members

        /// <summary>
        /// Creates the function.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="key">The key.</param>
        /// <param name="description">The description.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="parentName">Name of the parent.</param>
        /// <param name="displayOrder">The display order.</param>
        /// <param name="custId">The cust id.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="assignedGroupIDs">The assigned group I ds.</param>
        /// <param name="functionId">The function id.</param>
        /// <returns>Result</returns>
        public Result CreateFunction(string name, string key, string description, int parentId, string parentName, int displayOrder, int custId, string ip, string assignedGroupIDs, out int functionId)
        {
            functionId = 0;

            using (var db = new ObjectDb("EAS_FunctionIns"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, custId);
                db.AddInParameter("ParentID", System.Data.DbType.Int32, parentId);
                db.AddInParameter("DisplayOrder", System.Data.DbType.Int32, displayOrder);
                db.AddInParameter("IsTest", System.Data.DbType.Boolean, false);

                db.AddInParameter("ParentName", System.Data.DbType.String, parentName);
                db.AddInParameter("FuncName", System.Data.DbType.String, name);
                db.AddInParameter("FuncDesc", System.Data.DbType.String, description);
                db.AddInParameter("FuncKey", System.Data.DbType.String, key);
                db.AddInParameter("IP", System.Data.DbType.String, ip);
                db.AddInParameter("AssigedGroupIDs", System.Data.DbType.String, assignedGroupIDs);

                db.AddOutParameter("FuncID", System.Data.DbType.Int32, sizeof(int));

                this.AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                var result = this.GetResultParams(db.Database, db.DbCommand);

                if (result.IsSuccess)
                {
                    functionId = (int)db.GetParameterValue("FuncID");
                }

                return result;
            }
        }

        /// <summary>
        /// Updates the function.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="parentName">Name of the parent.</param>
        /// <param name="displayOrder">The display order.</param>
        /// <param name="custId">The cust id.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="oldAssignedGroupIDs">The old assigned group I ds.</param>
        /// <param name="assignedGroupIDs">The assigned group I ds.</param>
        /// <returns>Result</returns>
        public Result UpdateFunction(int id, string name, string description, int parentId, string parentName, int displayOrder, int custId, string ip, string oldAssignedGroupIDs, string assignedGroupIDs)
        {
            using (var db = new ObjectDb("EAS_FunctionUpd"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, custId);
                db.AddInParameter("FuncID", System.Data.DbType.Int32, id);
                db.AddInParameter("ParentID", System.Data.DbType.Int32, parentId);
                db.AddInParameter("DisplayOrder", System.Data.DbType.Int32, displayOrder);

                db.AddInParameter("ParentName", System.Data.DbType.String, parentName);
                db.AddInParameter("FuncName", System.Data.DbType.String, name);
                db.AddInParameter("FuncDesc", System.Data.DbType.String, description);
                db.AddInParameter("IP", System.Data.DbType.String, ip);
                db.AddInParameter("AssigedGroupIDs", System.Data.DbType.String, assignedGroupIDs);
                db.AddInParameter("OldAssigedGroupIDs", System.Data.DbType.String, oldAssignedGroupIDs);

                this.AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return this.GetResultParams(db.Database, db.DbCommand);
            }
        }

        /// <summary>
        /// Gets all functions.
        /// </summary>
        /// <returns>IEnumerable<Function></returns>
        public IEnumerable<Function> GetAllFunctions()
        {
            var result = new List<Function>();

            using (var db = new ObjectDb("EAS_FunctionSel"))
            {
                using (var reader = db.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            result.Add(new Function
                            {
                                ModifiedDate = reader.ToString("ModifiedDate"),
                                DisplayOrder = reader.ToInt32("DisplayOrder"),
                                FuncDesc = reader.ToString("FuncDesc"),
                                FuncID = reader.ToInt32("FuncID"),
                                FuncKey = reader.ToString("FuncKey"),
                                FuncName = reader.ToString("FuncName"),
                                IsTest = reader.ToBoolean("IsTest"),
                                ParentID = reader.ToInt32("ParentID"),
                                ParentName = reader.ToString("ParentName")
                            });
                        }

                        return result;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the function policy.
        /// </summary>
        /// <param name="funcId">The func id.</param>
        /// <returns>Function</returns>
        public Function GetFunctionPolicy(int funcId)
        {
            var func = this.GetAllFunctions().Where(o => o.FuncID == funcId).FirstOrDefault();

            using (var db = new ObjectDb("EAS_FunctionSelEdit"))
            {
                db.AddInParameter("funcId", System.Data.DbType.Int32, funcId);

                using (var reader = db.ExecuteReader())
                {
                    if (reader != null)
                    {
                        var groups = new List<FunctionPolicy>();

                        while (reader.Read())
                        {
                            groups.Add(new FunctionPolicy
                            {
                                GroupID = reader.ToInt32("GroupID"),
                                GroupName = reader.ToString("GroupName"),
                                HasPermission = Convert.ToBoolean(reader["HasPermission"])
                            });
                        }

                        func.AssignGroups = groups;
                    }
                }
            }

            return func;
        }

        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="isTest">if set to <c>true</c> [is test].</param>
        /// <param name="custId">The cust id.</param>
        /// <param name="ip">The ip.</param>
        /// <returns>Result</returns>
        public Result UpdateStatus(int id, bool isTest, int custId, string ip)
        {
            using (var db = new ObjectDb("EAS_FuncUpdStatus"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, custId);
                db.AddInParameter("FuncID", System.Data.DbType.Int32, id);
                db.AddInParameter("IsTest", System.Data.DbType.Boolean, isTest);
                db.AddInParameter("IP", System.Data.DbType.String, ip);

                this.AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return this.GetResultParams(db.Database, db.DbCommand);
            }
        }

        #endregion IFunctionDao Members
    }
}