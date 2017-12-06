// --------------------------------------------------------------------------
// <copyright file="ILoginDao.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.DataAccess.Interface.Login
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;

    /// <summary>
    /// ILoginDao interface.
    /// </summary>
    public interface ILoginDao
    {
        Result Login(string username, string password, string browser, string ip, string domain, string countryCode, string platForm, out int custId);

        Result Logout(int custId, string username);

        Result Checkin(int custId, string netSessionID, string ip, out int onuserId);

        bool IsKickedOut(int custId);

        ICollection<Group> GroupList();

        bool CheckExternalGroup(int custId, int? funcId, string funcKey);

        DateTime GetDBTime();
    }
}