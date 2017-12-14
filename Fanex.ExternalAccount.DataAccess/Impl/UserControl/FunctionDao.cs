namespace Fanex.ExternalAccount.DataAccess.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Base;
    using Data;
    using DTO;
    using DTO.UserControl;
    using Interface.UserControl;
    using Utilities;

    public class FunctionDao : DAOBase, IFunctionDao
    {
        public Result CreateFunction(
            string name,
            string key,
            string description,
            int parentId,
            string parentName,
            int displayOrder,
            int custId,
            string ip,
            string assignedGroupIDs)
        {
            using (var db = new ObjectDb("EAS_FunctionIns"))
            {
                db.AddInParameter("UserID", DbType.Int32, custId);
                db.AddInParameter("ParentID", DbType.Int32, parentId);
                db.AddInParameter("DisplayOrder", DbType.Int32, displayOrder);
                db.AddInParameter("IsTest", DbType.Boolean, false);

                db.AddInParameter("ParentName", DbType.String, parentName);
                db.AddInParameter("FuncName", DbType.String, name);
                db.AddInParameter("FuncDesc", DbType.String, description);
                db.AddInParameter("FuncKey", DbType.String, key);
                db.AddInParameter("IP", DbType.String, ip);
                db.AddInParameter("AssigedGroupIDs", DbType.String, assignedGroupIDs);

                db.AddOutParameter("FuncID", DbType.Int32, sizeof(int));

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }

        public Result UpdateFunction(
            int id,
            string name,
            string description,
            int parentId,
            string parentName,
            int displayOrder,
            int custId,
            string ip,
            string oldAssignedGroupIDs,
            string assignedGroupIDs)
        {
            using (var db = new ObjectDb("EAS_FunctionUpd"))
            {
                db.AddInParameter("UserID", DbType.Int32, custId);
                db.AddInParameter("FuncID", DbType.Int32, id);
                db.AddInParameter("ParentID", DbType.Int32, parentId);
                db.AddInParameter("DisplayOrder", DbType.Int32, displayOrder);

                db.AddInParameter("ParentName", DbType.String, parentName);
                db.AddInParameter("FuncName", DbType.String, name);
                db.AddInParameter("FuncDesc", DbType.String, description);
                db.AddInParameter("IP", DbType.String, ip);
                db.AddInParameter("AssigedGroupIDs", DbType.String, assignedGroupIDs);
                db.AddInParameter("OldAssigedGroupIDs", DbType.String, oldAssignedGroupIDs);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }

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

        public Function GetFunctionPolicy(int funcId)
        {
            var func = GetAllFunctions().FirstOrDefault(o => o.FuncID == funcId);

            using (var db = new ObjectDb("EAS_FunctionSelEdit"))
            {
                db.AddInParameter("funcId", DbType.Int32, funcId);

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

        public Result UpdateStatus(int id, bool isTest, int custId, string ip)
        {
            using (var db = new ObjectDb("EAS_FuncUpdStatus"))
            {
                db.AddInParameter("UserID", DbType.Int32, custId);
                db.AddInParameter("FuncID", DbType.Int32, id);
                db.AddInParameter("IsTest", DbType.Boolean, isTest);
                db.AddInParameter("IP", DbType.String, ip);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }
    }
}