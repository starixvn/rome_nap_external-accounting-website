namespace Fanex.ExternalAccount.SearchAndCopy.Repositories
{
    using Fanex.ExternalAccount.SearchAndCopy.Criteria;
    using Fanex.ExternalAccount.SearchAndCopy.Models;
    using Fanex.NAP.Framework.Data;

    public interface ICustomerRepository
    {
        CustomerInfo GetCustomerInfoByUserId(int custId, bool isMember);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDynamicRepository dynamicRepository;

        public CustomerRepository()
            : this(null)
        {
        }

        public CustomerRepository(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public CustomerInfo GetCustomerInfoByUserId(int custId, bool isMember)
        {
            var criteria = new GetCustomerInfoByUserIdCriteria(custId, isMember);
            return dynamicRepository.Get<CustomerInfo>(criteria);
        }
    }
}
