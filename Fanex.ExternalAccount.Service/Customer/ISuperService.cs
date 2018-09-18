namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using System.Data;
    using DataAccess.Criteria.Customer;
    using DTO;
    using DTO.Customer;
    using PTEngine.BusinessObjects;

    public interface ISuperService
    {
        DataTable GetData(int? userId, string groupName, int? custId = null, bool? enabled = null,
            bool? closed = null, bool? suspended = null, bool? withDataLimited = null);

        CustomerInfo GetSuperInfo(int superId);

        DTO.Result UpdateSuperInfo(UpdateSuperInfoCriteria criteria);

        ICustomer GetCustomerForCompanyEditSuper(RequestContext context);
    }
}