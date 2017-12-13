namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System.Data;
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using Interface.Customer;

    public class SuperService : CustomerService, ISuperService
    {
        private readonly ISuperDao _dao = new SuperDao();

        public DataTable GetData(
            int? userId,
            int? recommend = null,
            int? custId = null,
            bool? enabled = null,
            bool? closed = null,
            bool? suspended = null,
            string groupName = "ACCDDD")
            => _dao.GetData(userId, recommend, custId, enabled, closed, suspended, groupName);
    }
}