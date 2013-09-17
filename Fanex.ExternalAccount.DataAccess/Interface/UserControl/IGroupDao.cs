// --------------------------------------------------------------------------
// <copyright file="IGroupDao.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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

    public interface IGroupDao
    {
        IEnumerable<Group> GetListGroup();

        IEnumerable<Group> GetAllGroup();

        IEnumerable<Function> GetAllGroupPolicy(int groupId);

        Group GetGroupById(int groupId);

        Result CreateGroup(string groupName, string description, int custId, string ip, out int groupId);

        Result EditGroup(string groupName, string description, int custId, string ip, int groupId);

        Result EditGroupPolicy(string oldGroupPolicy, string groupPolicy, int custId, string ip, int groupId);

        Result DeleteGroup(int groupID, int userId);
    }
}