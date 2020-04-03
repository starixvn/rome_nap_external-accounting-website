namespace Fanex.ExternalAccount.SearchAndCopy.Services
{
    using Fanex.ExternalAccount.SearchAndCopy.Models;
    using Fanex.ExternalAccount.SearchAndCopy.Repositories;
    using System.Linq;

    public interface ISearchAndCopyService
    {
        SearchCustomerModel SearchCustomer(string username, int roleId);
    }

    public class SearchAndCopyService : ISearchAndCopyService
    {
        private readonly ISearchAndCopyRepository searchAndCopyRepository;

        public SearchAndCopyService()
            : this(null)
        {
        }

        public SearchAndCopyService(ISearchAndCopyRepository searchAndCopyRepository)
        {
            this.searchAndCopyRepository = searchAndCopyRepository ?? new SearchAndCopyRepository();
        }

        public SearchCustomerModel SearchCustomer(string username, int roleId)
        {
            return searchAndCopyRepository.SearchCustomer(username).FirstOrDefault(c => c.RoleId == roleId);
        }
    }
}
