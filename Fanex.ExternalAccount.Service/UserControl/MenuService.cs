namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess.Impl.UserControl;
    using DataAccess.Interface.UserControl;
    using DTO;
    using DTO.UserControl;
    using Interface.UserControl;

    public class MenuService : IMenuService
    {
        private const string AssignSubGroupUrl = "~/SubGroup/SubGroupList";
        private readonly IMenuDao _menuDao;
        private readonly ICachingService CachingService = new CachingService();

        public MenuService(IMenuDao menuDao = null)
        {
            _menuDao = menuDao ?? new MenuDao();
        }

        public IEnumerable<Menu> GetAllMenus() => _menuDao.GetAllMenus();

        public IEnumerable<Menu> GetMenusByUserId(int userId, string currentGroup, bool isShowAssignSubGroup)
        {
            var userCache = CachingService.GetUserCache(userId, currentGroup);
            
            if (!isShowAssignSubGroup)
            {
                return userCache.MenuList.Where(menu => menu.Url != AssignSubGroupUrl);
            }

            return userCache.MenuList;
        }

        public Result CreateMenu(Menu menu, int custId, string ip)
        {
            var result = _menuDao.CreateMenu(menu, custId, ip);

            if (result.IsSuccess)
            {
                OnMenuUpdated();
            }

            return result;
        }

        public Result UpdateMenu(Menu menu, int custId, string ip)
        {
            var result = _menuDao.UpdateMenu(menu, custId, ip);

            if (result.IsSuccess)
            {
                OnMenuUpdated();
            }

            return result;
        }

        public Result UpdateMenuStatus(int id, bool isActive, int custId)
        {
            var result = _menuDao.UpdateMenuStatus(id, isActive, custId);

            if (result.IsSuccess)
            {
                OnMenuUpdated();
            }

            return result;
        }

        public Result DeleteMenu(int id, int custId)
        {
            var result = _menuDao.DeleteMenu(id, custId);

            if (result.IsSuccess)
            {
                OnMenuUpdated();
            }

            return result;
        }

        private void OnMenuUpdated()
        {
            CachingService.ClearUserCache();
        }
    }
}