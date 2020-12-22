namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Helper;
    using DataAccess.Impl.UserControl;
    using DataAccess.Interface.UserControl;
    using DTO;
    using DTO.UserControl;
    using Interface.UserControl;

    public class GroupService : IGroupService
    {
        private readonly IGroupDao _groupDao;

        public GroupService(IGroupDao groupDao = null)
        {
            _groupDao = groupDao ?? new GroupDao();
        }

        public IEnumerable<Group> GetListGroup() => _groupDao.GetListGroup();

        public IEnumerable<Group> GetExternalGroup() => _groupDao.GetExternalGroup().OrderBy(i => i.GroupName);

        public IEnumerable<Group> GetAllGroup(int mode = 0) => _groupDao.GetAllGroup(mode);

        public IEnumerable<Function> GetAllGroupPolicy(int groupId) => _groupDao.GetAllGroupPolicy(groupId);

        public Group GetGroupById(int groupId)
        {
            Check.Argument.IsNotNegativeOrZero(groupId, "groupId");

            return _groupDao.GetGroupById(groupId, 1).FirstOrDefault();
        }

        public IEnumerable<Group> GetGroupWithSubGroups(int groupId)
        {
            if(groupId <= 0)
            {
                return Enumerable.Empty<Group>();
            }

            const int GroupWithSub = 2;
            var groups = _groupDao.GetGroupById(groupId, GroupWithSub).ToList();

            return groups.Count > 1 ? groups : Enumerable.Empty<Group>();
        }

        public Result CreateGroup(string groupName, string description, int parentId, int custId, string ip)
            => _groupDao.CreateGroup(groupName, description, parentId, custId, ip);

        public Result EditGroup(string groupName, string description, int custId, string ip, int groupId)
            => _groupDao.EditGroup(groupName, description, custId, ip, groupId);

        public Result EditGroupPolicy(string oldGroupPolicy, string groupPolicy, int custId, string ip, int groupId)
            => _groupDao.EditGroupPolicy(oldGroupPolicy, groupPolicy, custId, ip, groupId);

        public Result DeleteGroup(int groupId, int userId)
            => _groupDao.DeleteGroup(groupId, userId);

        public bool CheckIsMainGroup(int groupId)
        {
            var group = GetGroupById(groupId);

            return group?.ParentGroupId == 0;
        }

        public Group GetGroupByName(string groupName)
            => _groupDao.GetGroupByName(groupName);
    }
}