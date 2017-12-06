// --------------------------------------------------------------------------
// <copyright file="ILoginService.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.Service.Interface.Login
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;

    public interface ILoginService
    {
        Result Login(
            string username, 
            string password, 
            Request request, 
            out int custId, 
            out int onuserId, 
            out bool isIntAcc);

        Result Logout(int custId, string username);

        bool IsKickedOut(int custId);

        ICollection<Group> GroupList();

        bool CheckExternalGroup(int custId, int? funcId, string funcKey);

        DateTime GetDBTime();
    }
}