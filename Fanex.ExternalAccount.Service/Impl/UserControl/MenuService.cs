namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DataAccess.Impl.UserControl;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Logging.Extensions.Web;

    public class MenuService : IMenuService
    {
        private readonly IMenuDao _menuDao = null;

        public MenuService(IMenuDao menuDao = null)
        {
            this._menuDao = menuDao ?? new MenuDao();
        }

        #region IMenuService Members

        public IEnumerable<Menu> GetAllMenus()
        {
            try
            {
                return this._menuDao.GetAllMenus();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public IEnumerable<Menu> GetMenusByUserId(int userId)
        {
            ICachingService cachingService = new CachingService();
            var userCache = cachingService.GetUserCache(userId);
            return userCache.MenuList;
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
                ExceptionHandler.Handle(ex);
            }

            return null;
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
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

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
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

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
                ExceptionHandler.Handle(ex);
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