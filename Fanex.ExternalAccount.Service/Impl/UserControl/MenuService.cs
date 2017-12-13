namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using Base;
    using DataAccess.Impl.UserControl;
    using DataAccess.Interface.UserControl;
    using DTO;
    using DTO.UserControl;
    using Interface.UserControl;

    public class MenuService : ServiceBase, IMenuService
    {
        private readonly IMenuDao _menuDao;

        public MenuService(IMenuDao menuDao = null)
        {
            _menuDao = menuDao ?? new MenuDao();
        }

        public IEnumerable<Menu> GetAllMenus()
        {
            try
            {
                return _menuDao.GetAllMenus();
            }
            catch (Exception ex)
            {
                ProcessException(ex);
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
                var result = _menuDao.CreateMenu(name, description, url, functionId, parentId, parentName, displayOrder,
                    isActive, custId, ip, out menuId);

                if (result.IsSuccess)
                {
                    OnMenuUpdated();
                }

                return result;
            }
            catch (Exception ex)
            {
                ProcessException(ex);
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
                var result = _menuDao.UpdateMenu(id, name, description, url, functionId, parentId, parentName,
                    displayOrder, custId, ip);

                if (result.IsSuccess)
                {
                    OnMenuUpdated();
                }

                return result;
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }

            return null;
        }

        public Result UpdateMenuStatus(int id, bool isActive, int custId)
        {
            try
            {
                var result = _menuDao.UpdateMenuStatus(id, isActive, custId);

                if (result.IsSuccess)
                {
                    OnMenuUpdated();
                }

                return result;
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }

            return null;
        }

        public Result DeleteMenu(int id, int custId)
        {
            try
            {
                var result = _menuDao.DeleteMenu(id, custId);

                if (result.IsSuccess)
                {
                    OnMenuUpdated();
                }

                return result;
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }

            return null;
        }

        private static void OnMenuUpdated() => CachingService.ClearUserCache();
    }
}