namespace Fanex.ExternalAccount.DataAccess.Interface.UserControl
{
    using System.Collections.Generic;
    using DTO;
    using DTO.UserControl;

    public interface IMenuDao
    {
        IEnumerable<Menu> GetAllMenus();

        UserCache GetMenuByUserId(int userId);

        Result CreateMenu(Menu menu, int custId, string ip);

        Result UpdateMenu(Menu menu, int custId, string ip);

        Result UpdateMenuStatus(int id, bool isActive, int custId);

        Result DeleteMenu(int id, int custId);
    }
}