namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using DataAccess.Criteria.Customer;
    using DTO.Customer;
    using PTEngine.BusinessObjects;

    public interface ISuperService
    {
        CustomerInfo GetSuperInfo(int superId);

        DTO.Result UpdateSuperInfo(UpdateSuperInfoCriteria criteria);

        ICustomer GetCustomerForCompanyEditSuper(RequestContext context);
    }
}