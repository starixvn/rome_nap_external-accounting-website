// --------------------------------------------------------------------------
// <copyright file="MenuService.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DataAccess.Impl.UserControl;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Impl.Base;
    using Fanex.ExternalAccount.Service.Interface.UserControl;

    /// <summary>
    /// Menu service class.
    /// </summary>
    public class MenuService : ServiceBase, IMenuService
    {
        private readonly IMenuDao _menuDao = null;

        public MenuService(IMenuDao menuDao = null)
        {
            this._menuDao = menuDao ?? new MenuDao();
        }

        #region IMenuService Members

        /// <summary>
        /// Gets all menus.
        /// </summary>
        /// <returns>IEnumerable object.</returns>
        public IEnumerable<Menu> GetAllMenus()
        {
            try
            {
                return this._menuDao.GetAllMenus();
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        public IEnumerable<Menu> GetMenusByUserId(int userId)
        {
            ICachingService cachingService = new CachingService();
            var userCache = cachingService.GetUserCache(userId);
            return userCache.MenuList;
        }

        /// <summary>
        /// Creates new menu.
        /// </summary>
        /// <param name="name">The menu name.</param>
        /// <param name="description">Menu Description.</param>
        /// <param name="url">The menu URL.</param>
        /// <param name="functionId">Function id.</param>
        /// <param name="parentId">Parent function id.</param>
        /// <param name="parentName">Parent function name.</param>
        /// <param name="displayOrder">Display order.</param>
        /// <param name="isActive">Is menu active or not.</param>
        /// <param name="custId">Admin user id.</param>
        /// <param name="ip">IP address.</param>
        /// <param name="menuId">The menu id.</param>
        /// <returns>Result object.</returns>
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

            try
            {
                var result = this._menuDao.CreateMenu(name, description, url, functionId, parentId, parentName, displayOrder, isActive, custId, ip, out menuId);

                if (result.IsSuccess)
                {
                    this.OnMenuUpdated();
                }

                return result;
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        /// <summary>
        /// Update menu info.
        /// </summary>
        /// <param name="id">The menu id.</param>
        /// <param name="name">The menu name.</param>
        /// <param name="description">Menu Description.</param>
        /// <param name="url">The menu URL.</param>
        /// <param name="functionId">Function id.</param>
        /// <param name="parentId">Parent function id.</param>
        /// <param name="parentName">Parent function name.</param>
        /// <param name="displayOrder">Display order.</param>
        /// <param name="custId">Admin user id.</param>
        /// <param name="ip">IP address.</param>
        /// <returns>Result object.</returns>
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
            try
            {
                var result = this._menuDao.UpdateMenu(id, name, description, url, functionId, parentId, parentName, displayOrder, custId, ip);

                if (result.IsSuccess)
                {
                    this.OnMenuUpdated();
                }

                return result;
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        /// <summary>
        /// Updates menu status.
        /// </summary>
        /// <param name="id">The menu id.</param>
        /// <param name="isActive">Is menu active or not.</param>
        /// <param name="custId">Admin user id.</param>
        /// <returns>Result object.</returns>
        public Result UpdateMenuStatus(int id, bool isActive, int custId)
        {
            try
            {
                var result = this._menuDao.UpdateMenuStatus(id, isActive, custId);

                if (result.IsSuccess)
                {
                    this.OnMenuUpdated();
                }

                return result;
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        /// <summary>
        /// Deletes menu item.
        /// </summary>
        /// <param name="id">The menu id.</param>
        /// <param name="custId">Admin user id.</param>
        /// <returns>Result object.</returns>
        public Result DeleteMenu(int id, int custId)
        {
            try
            {
                var result = this._menuDao.DeleteMenu(id, custId);

                if (result.IsSuccess)
                {
                    this.OnMenuUpdated();
                }

                return result;
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        #endregion IMenuService Members

        private void OnMenuUpdated()
        {
            var cachingService = new CachingService();
            cachingService.ClearUserCache();
        }
    }
}