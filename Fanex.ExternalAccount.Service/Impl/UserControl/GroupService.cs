namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System.Collections.Generic;
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

        public IEnumerable<Group> GetAllGroup() => _groupDao.GetAllGroup();

        public IEnumerable<Function> GetAllGroupPolicy(int groupId) => _groupDao.GetAllGroupPolicy(groupId);

        public Group GetGroupById(int groupId)
        {
            Check.Argument.IsNotNegativeOrZero(groupId, "groupId");

            return _groupDao.GetGroupById(groupId);
        }

        public Result CreateGroup(string groupName, string description, int custId, string ip)
            => _groupDao.CreateGroup(groupName, description, custId, ip);

        public Result EditGroup(string groupName, string description, int custId, string ip, int groupId)
            => _groupDao.EditGroup(groupName, description, custId, ip, groupId);

        public Result EditGroupPolicy(string oldGroupPolicy, string groupPolicy, int custId, string ip, int groupId)
            => _groupDao.EditGroupPolicy(oldGroupPolicy, groupPolicy, custId, ip, groupId);

        public Result DeleteGroup(int groupId, int userId)
            => _groupDao.DeleteGroup(groupId, userId);
    }
}