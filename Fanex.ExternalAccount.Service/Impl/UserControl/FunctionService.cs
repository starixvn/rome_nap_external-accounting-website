// --------------------------------------------------------------------------
// <copyright file="FunctionService.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DataAccess.Impl.UserControl;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Logging.Extensions.Web;

    /// <summary>
    /// Function service object.
    /// </summary>
    public class FunctionService : IFunctionService
    {
        private readonly IFunctionDao _funcDao = null;

        public FunctionService(IFunctionDao funcDao = null)
        {
            this._funcDao = funcDao ?? new FunctionDao();
        }

        #region IFunctionService Members

        /// <summary>
        /// Creates new function.
        /// </summary>
        /// <param name="name">Function name.</param>
        /// <param name="key">Function key.</param>
        /// <param name="description">Function description.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="parentName">Parent name.</param>
        /// <param name="displayOrder">Display order.</param>
        /// <param name="custId">Admin user id.</param>
        /// <param name="ip">IP address.</param>
        /// <param name="assignedGroupIDs">Assigned group ids.</param>
        /// <param name="functionId">Function id.</param>
        /// <returns>Result object.</returns>
        public Result CreateFunction(
            string name, 
            string key, 
            string description,
            int parentId, 
            string parentName,
            int displayOrder, 
            int custId,
            string ip, 
            string assignedGroupIDs,
            out int functionId)
        {
            functionId = 0;

            try
            {
                return this._funcDao.CreateFunction(name, key, description, parentId, parentName, displayOrder, custId, ip, assignedGroupIDs, out functionId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        /// <summary>
        /// Updates function.
        /// </summary>
        /// <param name="id">Function id.</param>
        /// <param name="name">Function name.</param>
        /// <param name="description">Function description.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="parentName">Parent name.</param>
        /// <param name="displayOrder">Display order.</param>
        /// <param name="custId">Admin user id.</param>
        /// <param name="ip">IP address.</param>
        /// <param name="oldAssignedGroupIDs">Old assigned group ids.</param>
        /// <param name="assignedGroupIDs">Assigned group ids.</param>
        /// <returns>Result object.</returns>
        public Result UpdateFunction(
            int id, 
            string name,
            string description,
            int parentId, 
            string parentName, 
            int displayOrder,
            int custId,
            string ip, 
            string oldAssignedGroupIDs,
            string assignedGroupIDs)
        {
            try
            {
                return this._funcDao.UpdateFunction(id, name, description, parentId, parentName, displayOrder, custId, ip, oldAssignedGroupIDs, assignedGroupIDs);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets all functions.
        /// </summary>
        /// <returns>IEnumerable object.</returns>
        public IEnumerable<Function> GetAllFunctions()
        {
            try
            {
                return this._funcDao.GetAllFunctions();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets function policy.
        /// </summary>
        /// <param name="funcId">Function id.</param>
        /// <returns>Function object.</returns>
        public Function GetFunctionPolicy(int funcId)
        {
            try
            {
                return this._funcDao.GetFunctionPolicy(funcId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        /// <summary>
        /// Updates function status.
        /// </summary>
        /// <param name="id">Function id.</param>
        /// <param name="isTest">Is test function or not.</param>
        /// <param name="custId">Admin user id.</param>
        /// <param name="ip">IP address.</param>
        /// <returns>Result object.</returns>
        public Result UpdateStatus(int id, bool isTest, int custId, string ip)
        {
            try
            {
                return this._funcDao.UpdateStatus(id, isTest, custId, ip);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        #endregion IFunctionService Members
    }
}