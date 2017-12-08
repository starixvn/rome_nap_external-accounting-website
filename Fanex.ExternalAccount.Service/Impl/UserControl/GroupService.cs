namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.ExternalAccount.DataAccess.Impl.UserControl;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Logging.Extensions.Web;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GroupService : IGroupService
    {
        private IGroupDao _groupDao = null;

        public GroupService(IGroupDao groupDao = null)
        {
            this._groupDao = groupDao ?? new GroupDao();
        }

        #region IGroupService Members

        /// <summary>
        /// Gets groups.
        /// </summary>
        /// <returns>IEnumerable object.</returns>
        public IEnumerable<Group> GetListGroup()
        {
            try
            {
                return this._groupDao.GetListGroup();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets groups.
        /// </summary>
        /// <returns>IEnumerable object.</returns>
        public IEnumerable<Group> GetAllGroup()
        {
            try
            {
                return this._groupDao.GetAllGroup();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets policy of group.
        /// </summary>
        /// <param name="groupId">Group's id.</param>
        /// <returns>IEnumerable object.</returns>
        public IEnumerable<Function> GetAllGroupPolicy(int groupId)
        {
            try
            {
                return this._groupDao.GetAllGroupPolicy(groupId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets group by id.
        /// </summary>
        /// <param name="groupId">Group's id.</param>
        /// <returns>Group object.</returns>
        public Group GetGroupById(int groupId)
        {
            Check.Argument.IsNotNegativeOrZero(groupId, "groupId");

            try
            {
                return _groupDao.GetGroupById(groupId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        /// <summary>
        /// Creates new group.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <param name="description">Group description.</param>
        /// <param name="custId">Admin user id.</param>
        /// <param name="ip">IP address.</param>
        /// <param name="groupId">Id of group.</param>
        /// <returns>Result object.</returns>
        public DTO.Result CreateGroup(string groupName, string description, int custId, string ip, out int groupId)
        {
            groupId = 0;

            try
            {
                return this._groupDao.CreateGroup(groupName, description, custId, ip, out groupId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        /// <summary>
        /// Updates group.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <param name="description">Group description.</param>
        /// <param name="custId">Admin user id.</param>
        /// <param name="ip">IP address.</param>
        /// <param name="groupId">Id of group.</param>
        /// <returns>Result object.</returns>
        public DTO.Result EditGroup(string groupName, string description, int custId, string ip, int groupId)
        {
            try
            {
                return this._groupDao.EditGroup(groupName, description, custId, ip, groupId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        /// <summary>
        /// Edits group policy.
        /// </summary>
        /// <param name="oldGroupPolicy">Old group policy.</param>
        /// <param name="groupPolicy">New group policy.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="ip">IP address.</param>
        /// <param name="groupId">Id of group.</param>
        /// <returns>Result object.</returns>
        public DTO.Result EditGroupPolicy(string oldGroupPolicy, string groupPolicy, int custId, string ip, int groupId)
        {
            try
            {
                return this._groupDao.EditGroupPolicy(oldGroupPolicy, groupPolicy, custId, ip, groupId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        /// <summary>
        /// Deletes group.
        /// </summary>
        /// <param name="groupID">Id of group.</param>
        /// <param name="userId">Admin user id.</param>
        /// <returns>Result object.</returns>
        public DTO.Result DeleteGroup(int groupID, int userId)
        {
            try
            {
                return this._groupDao.DeleteGroup(groupID, userId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        #endregion IGroupService Members
    }
}