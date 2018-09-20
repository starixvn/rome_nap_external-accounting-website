namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System.Data;
    using DataAccess.Criteria.Customer;
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using DTO;
    using DTO.Customer;
    using Interface.Customer;
    using NAP.Framework.Data;
    using PTEngine.BusinessObjects;

    public class SuperService : CustomerService, ISuperService
    {
        private readonly IDynamicRepository dynamicRepository = new DynamicRepository();
        private readonly ISuperDao _dao = new SuperDao();
        private readonly PTEngine.Services.ICustomerService customerService = new PTEngine.Services.CustomerService();

        public DataTable GetData(
            int? userId,
            string groupName,
            int? custId = null,
            bool? enabled = null,
            bool? closed = null,
            bool? suspended = null,
            bool? withDataLimited = null)
            => _dao.GetData(userId, groupName, custId, enabled, closed, suspended, withDataLimited);

        public CustomerInfo GetSuperInfo(int superId)
            => dynamicRepository.Get<CustomerInfo>(new GetSuperInfoCriteria(superId));

        public DTO.Result UpdateSuperInfo(UpdateSuperInfoCriteria criteria)
            => _dao.UpdateSuperInfo(criteria);

        public ICustomer GetCustomerForCompanyEditSuper(RequestContext context)
        {
            var customer = customerService.GetCustomerForCompanyEditSuper(context);
            var superInfo = GetSuperInfo(context.TargetCustId);

            if (superInfo != null)
            {

                customer.FirstName = superInfo.FirstName;
                customer.LastName = superInfo.LastName;
                customer.Phone = superInfo.Phone;
                customer.MobilePhone = superInfo.MobilePhone;
                customer.Fax = superInfo.Fax;
            }

            return customer;
        }
    }
}