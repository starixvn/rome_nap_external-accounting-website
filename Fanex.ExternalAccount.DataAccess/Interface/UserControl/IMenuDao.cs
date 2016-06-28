// --------------------------------------------------------------------------
// <copyright file="IMenuDao.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.DataAccess.Interface.UserControl
{
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;

    public interface IMenuDao
    {
        IEnumerable<Menu> GetAllMenus();

        UserCache GetMenuByUserId(int userId);

        Result CreateMenu(
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
            out int menuId);

        Result UpdateMenu(
            int id, 
            string name,
            string description,
            string url,
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