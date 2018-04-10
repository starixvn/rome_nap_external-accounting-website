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
                                MenuId = reader.ToInt32("MenuId"),
                                MenuName = reader.ToString("MenuName"),
                                MenuDesc = reader.ToString("MenuDesc"),
                                ParentId = reader.ToInt32("ParentId"),
                                ParentName = reader.ToString("ParentName"),
                                Url = reader.ToString("Url"),
                                FuncId = reader.ToInt32("FuncId"),
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
                                     let parentId = c.Field<int>("ParentId")
                                     select new Menu
                                     {
                                         MenuId = c.Field<int>("MenuId"),
                                         MenuName = c.Field<string>("MenuName"),
                                         ParentId = c.Field<int>("ParentId"),
                                         ParentName = c.Field<string>("ParentName"),
                                         Url = c.Field<string>("Url"),
                                         FuncId = c.Field<int>("FuncId"),
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

        public Result CreateMenu(Menu menu, int custId, string ip)
        {
            using (var db = new ObjectDb("EAS_MenuIns"))
            {
                db.AddInParameter("UserID", DbType.Int32, custId);
                db.AddInParameter("FuncId", DbType.Int32, menu.FuncId);
                db.AddInParameter("ParentId", DbType.Int32, menu.ParentId);
                db.AddInParameter("DisplayOrder", DbType.Int32, menu.DisplayOrder);

                db.AddInParameter("MenuName", DbType.String, menu.MenuName);
                db.AddInParameter("MenuDesc", DbType.String, menu.MenuDesc);
                db.AddInParameter("ParentName", DbType.String, menu.ParentName);
                db.AddInParameter("URL", DbType.String, menu.Url);
                db.AddInParameter("IP", DbType.String, ip);
                db.AddOutParameter("MenuId", DbType.Int32, sizeof(int));

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }

        public Result UpdateMenu(Menu menu, int custId, string ip)
        {
            using (var db = new ObjectDb("EAS_MenuUpd"))
            {
                db.AddInParameter("UserID", DbType.Int32, custId);
                db.AddInParameter("MenuId", DbType.Int32, menu.MenuId);
                db.AddInParameter("FuncId", DbType.Int32, menu.FuncId);
                db.AddInParameter("ParentId", DbType.Int32, menu.ParentId);
                db.AddInParameter("DisplayOrder", DbType.Int32, menu.DisplayOrder);

                db.AddInParameter("MenuName", DbType.String, menu.MenuName);
                db.AddInParameter("MenuDesc", DbType.String, menu.MenuDesc);
                db.AddInParameter("ParentName", DbType.String, menu.ParentName);
                db.AddInParameter("URL", DbType.String, menu.Url);
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
                db.AddInParameter("MenuId", DbType.Int32, id);
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
                db.AddInParameter("MenuId", DbType.Int32, id);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return GetResultParams(db.Database, db.DbCommand);
            }
        }
    }
}