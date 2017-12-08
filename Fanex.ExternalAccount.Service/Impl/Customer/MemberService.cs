namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System;
    using Fanex.ExternalAccount.DataAccess.Impl.Customer;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;
    using Fanex.ExternalAccount.Service.Interface.Customer;
    using Logging.Extensions.Web;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MemberService : CustomerService, IMemberService
    {
        private readonly IMemberDao _dao = new MemberDao();

        #region IDataService Members

        /// <summary>
        /// Base function for all levels.
        /// </summary>
        /// <param name="recommend">Id of current level.</param>
        /// <param name="custId">Id of selected user.</param>
        /// <param name="enabled">Enable status.</param>
        /// <param name="closed">Close status.</param>
        /// <param name="suspended">Suspend status.</param>
        /// <param name="groupName">Group name of currency.</param>
        /// <returns>DataTable object.</returns>
        public System.Data.DataTable GetData(
            int? userId,
            int? recommend = null, 
            int? custId = null, 
            bool? enabled = null, 
            bool? closed = null, 
            bool? suspended = null, 
            string groupName = "ACCDDD")
        {
            try
            {
                return this._dao.GetData(userId, recommend, custId, enabled, closed, suspended, groupName);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        #endregion IDataService Members
    }
}