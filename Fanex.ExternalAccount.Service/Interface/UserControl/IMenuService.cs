namespace Fanex.ExternalAccount.Service.Interface.UserControl
{
    using System.Collections.Generic;
    using DTO;
    using DTO.UserControl;

    public interface IMenuService
    {
        IEnumerable<Menu> GetAllMenus();

        IEnumerable<Menu> GetMenusByUserId(int userId);

        Result CreateMenu(
            string name,
            string description,
            string path,
            int functionId,
            int parentId,
            string parentName,
            int displayOrder,
            bool isActive,
            int custId,
            string ip);

        Result UpdateMenu(
            int id,
            string name,
            string description,
            string path,
            int functionId,
            int parentId,
            string parentName,
            int displayOrder,
            int custId,
            string ip);

        Result UpdateMenuStatus(int id, bool isActive, int custId);

        Result DeleteMenu(int id, int custId);
    }
}