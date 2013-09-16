// --------------------------------------------------------------------------
// <copyright file="BasicInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.16">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    /// <summary>
    /// Basic information object.
    /// </summary>
    public class BasicInfo
    {
        public ObjectInfo CustId { get; set; }

        public ObjectInfo UplineId { get; set; }

        public ObjectInfo RoleId { get; set; }

        public ObjectInfo UplineRoleId { get; set; }
    }
}
