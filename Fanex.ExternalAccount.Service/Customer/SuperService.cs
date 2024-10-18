namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using DataAccess.Criteria.Customer;
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using DTO.Customer;
    using Interface.Customer;
    using NAP.Framework.Data;

    public class SuperService : CustomerService, ISuperService
    {
        private readonly IDynamicRepository dynamicRepository = new DynamicRepository();
        private readonly ISuperDao _dao = new SuperDao();

        public CustomerInfo GetSuperInfo(int superId)
            => dynamicRepository.Get<CustomerInfo>(new GetSuperInfoCriteria(superId));

        public DTO.Result UpdateSuperInfo(UpdateSuperInfoCriteria criteria)
            => _dao.UpdateSuperInfo(criteria);
   
    }
}