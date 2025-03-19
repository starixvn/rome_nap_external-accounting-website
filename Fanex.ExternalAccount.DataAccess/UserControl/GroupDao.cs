namespace Fanex.ExternalAccount.DataAccess.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Common.Extensions;
    using Criteria.UserControl.Group;
    using Data;
    using DTO;
    using DTO.UserControl;
    using Fanex.Data.Repository;
    using Interface.UserControl;
    using System.Linq;

    public class GroupDao: IGroupDao
    {
        private readonly IDynamicRepository dynamicRepository;

        public GroupDao(IDynamicRepository dynamicRepository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<Group> GetListGroup()
        {
            var criteria = new GetListGroupCriteria();
            return dynamicRepository.Fetch<Group>(criteria) ?? new List<Group>();
        }

        public IEnumerable<Group> GetAllGroup(int mode)
        {
            var criteria = new GetAllGroupCriteria { Mode = mode };
            return dynamicRepository.Fetch<Group>(criteria) ?? new List<Group>();
        }

        public IEnumerable<Group> GetExternalGroup()
        {
            var criteria = new GetExternalGroupCriteria();
            return dynamicRepository.Fetch<Group>(criteria) ?? new List<Group>();
        }

        public IEnumerable<Function> GetAllGroupPolicy(int groupId)
        {
            var criteria = new GetAllGroupPolicyCriteria { GroupId = groupId };
            return dynamicRepository.Fetch<Function>(criteria) ?? new List<Function>();
        }

        public IEnumerable<Group> GetGroupById(int groupId, byte mode)
        {
            var criteria = new GetGroupByIdCriteria { GroupId = groupId, Mode = mode };
            return dynamicRepository.Fetch<Group>(criteria) ?? new List<Group>();
        }
        
        public Group GetGroupByName(string groupName)
        {
            var criteria = new GetGroupByNameCriteria { GroupName = groupName };
            return dynamicRepository.Fetch<Group>(criteria).FirstOrDefault();
        }

        public Result CreateGroup(string groupName, string description, int parentId, int custId, string ip)
        {
            var criteria = new CreateGroupCriteria
            {
                UserId = custId,
                GroupName = groupName,
                Description = description,
                ParentId = parentId,
                IP = ip
            };

            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public Result EditGroup(string groupName, string description, int custId, string ip, int groupId)
        {
            var criteria = new EditGroupCriteria
            {
                UserId = custId,
                GroupId = groupId,
                GroupName = groupName,
                Description = description,
                IP = ip
            };

            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public Result EditGroupPolicy(string oldGroupPolicy, string groupPolicy, int custId, string ip, int groupId)
        {
            var criteria = new EditGroupPolicyCriteria
            {
                UserId = custId,
                GroupId = groupId,
                OldGroupPolicy = oldGroupPolicy,
                GroupPolicy = groupPolicy,
                IP = ip
            };

            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public Result DeleteGroup(int groupId, int userId)
        {
            var criteria = new DeleteGroupCriteria { GroupId = groupId, UserId = userId };
            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }
    }
}