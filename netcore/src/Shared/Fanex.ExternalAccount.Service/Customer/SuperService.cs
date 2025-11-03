namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using DataAccess.Criteria.Customer;
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using DTO.Customer;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DTO;
    using Interface.Customer;

    public class SuperService : CustomerService, ISuperService
    {
        private readonly IDynamicRepository dynamicRepository = new DynamicRepository();
        private readonly ISuperDao _dao = new SuperDao();

        public CustomerInfo GetSuperInfo(int superId)
            => dynamicRepository.Get<CustomerInfo>(new GetSuperInfoCriteria(superId));

        public Result UpdateSuperInfo(UpdateSuperInfoCommand criteria)
            => _dao.UpdateSuperInfo(criteria);
   
    }
}