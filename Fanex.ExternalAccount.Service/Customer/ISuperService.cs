namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using DataAccess.Criteria.Customer;
    using DTO.Customer;

    public interface ISuperService
    {
        CustomerInfo GetSuperInfo(int superId);

        DTO.Result UpdateSuperInfo(UpdateSuperInfoCriteria criteria);

    }
}