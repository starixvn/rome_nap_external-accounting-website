namespace Fanex.ExternalAccount.Service.Interface.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;

    public interface IGroupService
    {
        IEnumerable<Group> GetListGroup();

        IEnumerable<Group> GetAllGroup();

        IEnumerable<Group> GetExternalGroup();

        IEnumerable<Function> GetAllGroupPolicy(int groupId);

        Group GetGroupById(int groupId);

        Result CreateGroup(string groupName, string description, int custId, string ip);

        Result EditGroup(string groupName, string description, int custId, string ip, int groupId);

        Result EditGroupPolicy(string oldGroupPolicy, string groupPolicy, int custId, string ip, int groupId);

        Result DeleteGroup(int groupId, int userId);
    }
}