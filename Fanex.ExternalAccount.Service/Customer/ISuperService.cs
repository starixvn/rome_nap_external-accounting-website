namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using System.Data;
    using DataAccess.Criteria.Customer;
    using DTO;
    using DTO.Customer;
    using PTEngine.BusinessObjects;

    public interface ISuperService
    {
        CustomerInfo GetSuperInfo(int superId);

        DTO.Result UpdateSuperInfo(UpdateSuperInfoCriteria criteria);

        ICustomer GetCustomerForCompanyEditSuper(RequestContext context);
    }
}