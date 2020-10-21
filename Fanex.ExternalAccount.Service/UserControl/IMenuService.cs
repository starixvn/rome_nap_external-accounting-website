namespace Fanex.ExternalAccount.Service.Interface.UserControl
{
    using System.Collections.Generic;
    using DTO;
    using DTO.UserControl;

    public interface IMenuService
    {
        IEnumerable<Menu> GetAllMenus();

        IEnumerable<Menu> GetMenusByUserId(int userId, string currentGroup);

        Result CreateMenu(Menu menu, int custId, string ip);

        Result UpdateMenu(Menu menu, int custId, string ip);

        Result UpdateMenuStatus(int id, bool isActive, int custId);

        Result DeleteMenu(int id, int custId);
    }
}