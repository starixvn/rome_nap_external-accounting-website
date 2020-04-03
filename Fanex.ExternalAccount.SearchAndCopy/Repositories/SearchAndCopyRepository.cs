namespace Fanex.ExternalAccount.SearchAndCopy.Repositories
{
    using Fanex.ExternalAccount.SearchAndCopy.Criteria;
    using Fanex.ExternalAccount.SearchAndCopy.Models;
    using Fanex.NAP.Framework.Data;
    using System.Collections.Generic;

    public interface ISearchAndCopyRepository
    {
        IEnumerable<SearchCustomerModel> SearchCustomer(string username);
    }

    public class SearchAndCopyRepository : ISearchAndCopyRepository
    {
        private readonly IDynamicRepository dynamicRepository;

        public SearchAndCopyRepository()
            : this(null)
        {
        }

        public SearchAndCopyRepository(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<SearchCustomerModel> SearchCustomer(string username)
        {
            var criteria = new SearchCustomerCriteria(username);
            return dynamicRepository.Fetch<SearchCustomerModel>(criteria);
        }
    }
}
