namespace Fanex.ExternalAccount.DataAccess.Impl.UserControl
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Base;
    using Data;
    using DTO;
    using DTO.UserControl;
    using Interface.UserControl;
    using Utilities;

    public class MenuDao : DAOBase, IMenuDao
    {
        public IEnumerable<Menu> GetAllMenus()
        {
            var result = new List<Menu>();

            using (var db = new ObjectDb("EAS_MenuSel"))
            {
                using (var reader = db.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            result.Add(new Menu
                            {
                                MenuId = reader.ToInt32("MenuID"),
                                MenuName = reader.ToString("MenuName"),
                                MenuDesc = reader.ToString("MenuDesc"),
                                ParentId = reader.ToInt32("ParentID"),
                                ParentName = reader.ToString("ParentName"),
                                Url = reader.ToString("Url"),
                                FuncId = reader.ToInt32("FuncID"),
                                FuncName = reader.ToString("FuncName"),
                                DisplayOrder = reader.ToInt32("DisplayOrder"),
                                ModifiedDate = reader.ToString("ModifiedDate"),
                                IsActive = reader.ToBoolean("Active")
                            });
                        }

                        return result;
                    }
                }

                return null;
            }
        }

        public UserCache GetMenuByUserId(int userId)
        {
            using (var db = new ObjectDb("EAS_MenuSelByUserID"))
            {
                db.AddInParameter("UserID", DbType.Int32, userId);
                using (var ds = db.ExecuteDataSet())
                {
                    if (ds?.Tables.Count == 2)
                    {
                        var menu = ds.Tables[0];
                        var func = ds.Tables[1];

                        var menus = (from c in menu.AsEnumerable()
                            let parentId = c.Field<int>("ParentID")
                            select new Menu
                            {
                                MenuId = c.Field<int>("MenuID"),
                                MenuName = c.Field<string>("MenuName"),
                                ParentId = c.Field<int>("ParentID"),
                                ParentName = c.Field<string>("ParentName"),
                                Url = c.Field<string>("Url"),
                                FuncId = c.Field<int>("FuncID"),
                                DisplayOrder = c.Field<int>("DisplayOrder"),
                                IsActive = c.Field<bool>("Active")
                            }).ToList();

                        var funcs = (from c in func.AsEnumerable()
                            select new Function
                            {
                                FuncID = c.Field<int>("funcid"),
                                ParentID = c.Field<int>("parentId"),
                                FuncName = c.Field<string>("funcName"),
                                FuncKey = c.Field<string>("funcKey"),
                                Policy = c.Field<EPolicy>("policy"),
                                IsTest = c.Field<bool>("IsTest")
                            }).ToList();

                        return new UserCache
                        {
                            MenuList = menus,
                            PolicyList = funcs
                        };
                    }
                }

                return null;
            }
        }

        public Result CreateMenu(
            string name,
            string description,
            string url,
            int functionId,
            int parentId,
            string parentName,
            int displayOrder,
            bool isActive,
            int custId,
            string ip,
            out int menuId)
        {
            menuId = 0;

            using (var db = new ObjectDb("EAS_MenuIns"))
            {
                db.AddInParameter("UserID", DbType.Int32, custId);
                db.AddInParameter("FuncID", DbType.Int32, functionId);
                db.AddInParameter("ParentID", DbType.Int32, parentId);
                db.AddInParameter("DisplayOrder", DbType.Int32, displayOrder);

                db.AddInParameter("MenuName", DbType.String, name);
                db.AddInParameter("MenuDesc", DbType.String, description);
                db.AddInParameter("ParentName", DbType.String, parentName);
                db.AddInParameter("URL", DbType.String, url);
                db.AddInParameter("IP", DbType.String, ip);
                db.AddOutParameter("MenuID", DbType.Int32, sizeof(int));

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                var result = GetResultParams(db.Database, db.DbCommand);

                if (result.IsSuccess)
                {
                    menuId = (int) db.GetParameterValue("MenuID");
                }

                return result;
            }
        }

        public Result UpdateMenu(
            int id,
            string name,
            string description,
            string url,
            int functionId,
            int parentId,
            string parentName,
            int displayOrder,
            int custId,
            string ip)
        {
            using (var db = new ObjectDb("EAS_MenuUpd"))
            {
                db.AddInParameter("UserID", DbType.Int32, custId);
                db.AddInParameter("MenuID", DbType.Int32, id);
                db.AddInParameter("FuncID", DbType.Int32, functionId);
                db.AddInParameter("ParentID", DbType.Int32, parentId);
                db.AddInParameter("DisplayOrder", DbType.Int32, displayOrder);

                db.AddInParameter("MenuName", DbType.String, name);
                db.AddInParameter("MenuDesc", DbType.String, description);
                db.AddInParameter("ParentName", DbType.String, parentName);
                db.AddInParameter("URL", DbType.String, url);
                db.AddInParameter("IP", DbType.String, ip);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }

        public Result UpdateMenuStatus(int id, bool isActive, int custId)
        {
            using (var db = new ObjectDb("EAS_MenuUpdStatus"))
            {
                db.AddInParameter("UserID", DbType.Int32, custId);
                db.AddInParameter("MenuID", DbType.Int32, id);
                db.AddInParameter("Active", DbType.Boolean, isActive);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }

        public Result DeleteMenu(int id, int custId)
        {
            using (var db = new ObjectDb("EAS_MenuDel"))
            {
                db.AddInParameter("UserID", DbType.Int32, custId);
                db.AddInParameter("MenuID", DbType.Int32, id);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }
    }
}