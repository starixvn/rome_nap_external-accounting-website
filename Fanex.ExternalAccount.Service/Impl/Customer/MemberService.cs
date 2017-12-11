namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System;
    using Fanex.ExternalAccount.DataAccess.Impl.Customer;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;
    using Fanex.ExternalAccount.Service.Interface.Customer;
    using Logging.Extensions.Web;

    public class MemberService : CustomerService, IMemberService
    {
        private readonly IMemberDao _dao = new MemberDao();

        #region IDataService Members

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