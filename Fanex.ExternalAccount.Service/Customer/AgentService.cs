namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System.Data;
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using Interface.Customer;

    public class AgentService : CustomerService, IAgentService
    {
        private readonly IAgentDao _dao = new AgentDao();

        public DataTable GetData(
            int? userId,
            int? recommend = null,
            int? custId = null,
            bool? enabled = null,
            bool? closed = null,
            bool? suspended = null)
            => _dao.GetData(userId, recommend, custId, enabled, closed, suspended);
    }
}