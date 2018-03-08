namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System.Data;
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using Interface.Customer;

    public class MemberService : CustomerService, IMemberService
    {
        private readonly IMemberDao _dao = new MemberDao();

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