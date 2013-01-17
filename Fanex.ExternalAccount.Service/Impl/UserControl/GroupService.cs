// -----------------------------------------------------------------------
// <copyright file="GroupService.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.ExternalAccount.DataAccess.Impl.UserControl;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.Service.Impl.Base;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Fanex.ExternalAccount.DTO.UserControl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GroupService : ServiceBase, IGroupService
    {
        private IGroupDao groupDao = null;

        public GroupService(IGroupDao groupDao = null)
        {
            this.groupDao = groupDao ?? new GroupDao();
        }

        #region IGroupService Members

        public IEnumerable<Group> GetListGroup()
        {
            try
            {
                return this.groupDao.GetListGroup();
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public IEnumerable<Group> GetAllGroup()
        {
            try
            {
                return this.groupDao.GetAllGroup();
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public IEnumerable<Function> GetAllGroupPolicy(int groupId)
        {
            try
            {
                return this.groupDao.GetAllGroupPolicy(groupId);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public Group GetGroupById(int groupId)
        {
            Check.Argument.IsNotNegativeOrZero(groupId, "groupId");

            try
            {
                return groupDao.GetGroupById(groupId);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public DTO.Result CreateGroup(string groupName, string description, int custId, string ip, out int groupId)
        {
            groupId = 0;

            try
            {
                return this.groupDao.CreateGroup(groupName, description, custId, ip, out groupId);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public DTO.Result EditGroup(string groupName, string description, int custId, string ip, int groupId)
        {
            try
            {
                return this.groupDao.EditGroup(groupName, description, custId, ip, groupId);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public DTO.Result EditGroupPolicy(string oldGroupPolicy, string groupPolicy, int custId, string ip, int groupId)
        {
            try
            {
                return this.groupDao.EditGroupPolicy(oldGroupPolicy, groupPolicy, custId, ip, groupId);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public DTO.Result DeleteGroup(int groupID, int userId)
        {
            try
            {
                return this.groupDao.DeleteGroup(groupID, userId);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        #endregion IGroupService Members
    }
}