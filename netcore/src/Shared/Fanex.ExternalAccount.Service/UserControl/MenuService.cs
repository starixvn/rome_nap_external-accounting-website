namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess.Impl.UserControl;
    using DTO;
    using DTO.UserControl;
    
    public interface IMenuService
    {
        IEnumerable<Menu> GetAllMenus();

        IEnumerable<Menu> GetMenusByUserId(
            int userId,
            string currentGroup,
            bool isShowAssignSubGroup,
            bool isShowSuperWinLossDetailBySubGroup);

        Result CreateMenu(Menu menu, int custId, string ip);

        Result UpdateMenu(Menu menu, int custId, string ip);

        Result UpdateMenuStatus(int id, bool isActive, int custId);

        Result DeleteMenu(int id, int custId);
    }
    public class MenuService : IMenuService
    {
        private const string AssignSubGroupUrl = "~/SubGroup/SubGroupList";
        private const string SuperWinLossDetailBySubGroup = "~/SuperWinLossDetailBySubGroup/Index";
        private readonly IMenuDao _menuDao;
        private readonly ICachingService CachingService = new CachingService();

        public MenuService(IMenuDao menuDao = null)
        {
            _menuDao = menuDao ?? new MenuDao();
        }

        public IEnumerable<Menu> GetAllMenus() => _menuDao.GetAllMenus();

        public IEnumerable<Menu> GetMenusByUserId(
            int userId,
            string currentGroup,
            bool isShowAssignSubGroup,
            bool isShowSuperWinLossDetailBySubGroup)
        {
            var userCache = CachingService.GetUserCache(userId, currentGroup);
            var menuList = userCache.MenuList.AsEnumerable();

            if (!isShowAssignSubGroup)
            {
                menuList = menuList.Where(menu => menu.Url != AssignSubGroupUrl);
            }

            if (!isShowSuperWinLossDetailBySubGroup)
            {
                menuList = menuList.Where(menu => menu.Url != SuperWinLossDetailBySubGroup);
            }

            return menuList;
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