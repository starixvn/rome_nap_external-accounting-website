namespace Fanex.ExternalAccount.DataAccess.Impl.UserControl
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// MenuDao
    /// </summary>
    public class MenuDao : DAOBase, IMenuDao
    {
        #region IMenuDao Members

        /// <summary>
        /// Gets all menus.
        /// </summary>
        /// <returns>IEnumerable<Menu></returns>
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
                                    MenuID = reader.ToInt32("MenuID"),
                                    MenuName = reader.ToString("MenuName"),
                                    MenuDesc = reader.ToString("MenuDesc"),
                                    ParentID = reader.ToInt32("ParentID"),
                                    ParentName = reader.ToString("ParentName"),
                                    Url = reader.ToString("Url"),
                                    FuncID = reader.ToInt32("FuncID"),
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

        /// <summary>
        /// Gets the menu by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>UserCache</returns>
        public UserCache GetMenuByUserId(int userId)
        {
            var menus = new List<Menu>();
            var funcs = new List<Function>();

            using (var db = new ObjectDb("EAS_MenuSelByUserID"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, userId);

                using (var ds = db.ExecuteDataSet())
                {
                    if (ds != null && ds.Tables.Count == 2)
                    {
                        var menu = ds.Tables[0];
                        var func = ds.Tables[1];

                        menus = (from c in menu.AsEnumerable()
                                 let parentId = c.Field<int>("ParentID")
                                 select new Menu
                                 {
                                     MenuID = c.Field<int>("MenuID"),
                                     MenuName = c.Field<string>("MenuName"),
                                     ParentID = c.Field<int>("ParentID"),
                                     ParentName = c.Field<string>("ParentName"),
                                     Url = c.Field<string>("Url"),
                                     FuncID = c.Field<int>("FuncID"),
                                     DisplayOrder = c.Field<int>("DisplayOrder"),
                                     IsActive = c.Field<bool>("Active")
                                 }).ToList();

                        funcs = (from c in func.AsEnumerable()
                                 select new Function
                                 {
                                     FuncID = c.Field<int>("funcid"),
                                     ParentID = c.Field<int>("parentId"),
                                     FuncName = c.Field<string>("funcName"),
                                     FuncKey = c.Field<string>("funcKey"),
                                     Policy = c.Field<ePolicy>("policy"),
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

        /// <summary>
        /// Creates the menu.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="url">The URL.</param>
        /// <param name="functionId">The function id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="parentName">Name of the parent.</param>
        /// <param name="displayOrder">The display order.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="custId">The cust id.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="menuId">The menu id.</param>
        /// <returns>Result</returns>
        public Result CreateMenu(string name, string description, string url, int functionId, int parentId, string parentName, int displayOrder, bool isActive, int custId, string ip, out int menuId)
        {
            menuId = 0;

            using (var db = new ObjectDb("EAS_MenuIns"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, custId);
                db.AddInParameter("FuncID", System.Data.DbType.Int32, functionId);
                db.AddInParameter("ParentID", System.Data.DbType.Int32, parentId);
                db.AddInParameter("DisplayOrder", System.Data.DbType.Int32, displayOrder);

                db.AddInParameter("MenuName", System.Data.DbType.String, name);
                db.AddInParameter("MenuDesc", System.Data.DbType.String, description);
                db.AddInParameter("ParentName", System.Data.DbType.String, parentName);
                db.AddInParameter("URL", System.Data.DbType.String, url);
                db.AddInParameter("IP", System.Data.DbType.String, ip);
                db.AddOutParameter("MenuID", DbType.Int32, sizeof(int));

                this.AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                var result = this.GetResultParams(db.Database, db.DbCommand);

                if (result.IsSuccess)
                {
                    menuId = (int)db.GetParameterValue("MenuID");
                }

                return result;
            }
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="url">The URL.</param>
        /// <param name="functionId">The function id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="parentName">Name of the parent.</param>
        /// <param name="displayOrder">The display order.</param>
        /// <param name="custId">The cust id.</param>
        /// <param name="ip">The ip.</param>
        /// <returns>Result</returns>
        public Result UpdateMenu(int id, string name, string description, string url, int functionId, int parentId, string parentName, int displayOrder, int custId, string ip)
        {
            using (var db = new ObjectDb("EAS_MenuUpd"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, custId);
                db.AddInParameter("MenuID", System.Data.DbType.Int32, id);
                db.AddInParameter("FuncID", System.Data.DbType.Int32, functionId);
                db.AddInParameter("ParentID", System.Data.DbType.Int32, parentId);
                db.AddInParameter("DisplayOrder", System.Data.DbType.Int32, displayOrder);

                db.AddInParameter("MenuName", System.Data.DbType.String, name);
                db.AddInParameter("MenuDesc", System.Data.DbType.String, description);
                db.AddInParameter("ParentName", System.Data.DbType.String, parentName);
                db.AddInParameter("URL", System.Data.DbType.String, url);
                db.AddInParameter("IP", System.Data.DbType.String, ip);

                this.AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return this.GetResultParams(db.Database, db.DbCommand);
            }
        }

        /// <summary>
        /// Updates the menu status.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="custId">The cust id.</param>
        /// <returns>Result</returns>
        public Result UpdateMenuStatus(int id, bool isActive, int custId)
        {
            using (var db = new ObjectDb("EAS_MenuUpdStatus"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, custId);
                db.AddInParameter("MenuID", System.Data.DbType.Int32, id);
                db.AddInParameter("Active", System.Data.DbType.Boolean, isActive);

                this.AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return this.GetResultParams(db.Database, db.DbCommand);
            }
        }

        /// <summary>
        /// Deletes the menu.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="custId">The cust id.</param>
        /// <returns>Result</returns>
        public Result DeleteMenu(int id, int custId)
        {
            using (var db = new ObjectDb("EAS_MenuDel"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, custId);
                db.AddInParameter("MenuID", System.Data.DbType.Int32, id);

                this.AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                return this.GetResultParams(db.Database, db.DbCommand);
            }
        }

        #endregion IMenuDao Members
    }
}