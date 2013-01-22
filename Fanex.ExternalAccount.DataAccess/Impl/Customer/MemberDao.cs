// --------------------------------------------------------------------------
// <copyright file="MemberDao.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.25">Created</change>
// </history>

namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System.Collections;
    using Fanex.Data.DNAide;
    using System.Data;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MemberDao : CustomerDao, IMemberDao
    {
        #region IData Members

        /// <summary>
        /// Base function for Member level
        /// </summary>
        /// <param name="recommend">Id of current level</param>
        /// <param name="custId">Id of selected user</param>
        /// <param name="enabled">Enable status</param>
        /// <param name="closed">Close status</param>
        /// <param name="suspended">Suspend status</param>
        /// <param name="groupName">Group name of currency</param>
        /// <returns>DataTable object</returns>
        public DataTable GetData(int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null, string groupName = "ACCDDD")
        {
            var param = new Hashtable();
            param.Add("recommend", recommend);
            param.Add("custid", custId);
            param.Add("enabled", enabled);
            param.Add("closed", closed);
            param.Add("suspended", suspended);

            return DbHelper.ExecuteDataTable("EAS_CustomerSelByAg", param);
        }

        #endregion IData Members
    }
}