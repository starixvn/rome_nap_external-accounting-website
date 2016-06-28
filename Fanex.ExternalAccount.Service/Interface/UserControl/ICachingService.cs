// --------------------------------------------------------------------------
// <copyright file="ICachingService.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.Service.Interface.UserControl
{
    using System;
    using System.Text;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;

    public interface ICachingService
    {
        UserCache GetUserCache(int userId);
    }
}