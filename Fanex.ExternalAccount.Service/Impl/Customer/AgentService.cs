namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System;
    using Fanex.ExternalAccount.DataAccess.Impl.Customer;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;
    using Fanex.ExternalAccount.DTO.Customer;
    using Fanex.ExternalAccount.Service.Interface.Customer;
    using Logging.Extensions.Web;

    public class AgentService : CustomerService, IAgentService
    {
        private readonly IAgentDao _dao = new AgentDao();

        #region IDataService Agents

        public System.Data.DataTable GetData(
            int? userId,
            int? recommend = null, 
            int? custId = null, 
            bool? enabled = null, 
            bool? closed = null, 
            bool? suspended = null)
        {
            try
            {
                return this._dao.GetData(userId, recommend, custId, enabled, closed, suspended);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public Transfer GetTransferOption(int custId)
        {
            try
            {
                return this._dao.GetTransferOption(custId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        #endregion IDataService Agents
    }
}