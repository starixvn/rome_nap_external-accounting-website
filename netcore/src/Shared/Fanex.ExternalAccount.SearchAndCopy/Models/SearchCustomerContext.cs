namespace Fanex.ExternalAccount.SearchAndCopy.Models
{
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.SearchAndCopy.Requests;

    public class SearchCustomerContext
    {
        public SearchCustomerContext(SearchCustomerRequest request, int roleId, ICurrentSession currentSession)
        {
            Request = request;
            RoleId = roleId;
            CurrentSession = currentSession;
        }

        public SearchCustomerRequest Request { get; }

        public int RoleId { get; }

        public ICurrentSession CurrentSession { get; }
    }
}
