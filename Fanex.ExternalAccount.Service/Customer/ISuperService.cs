namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using DataAccess.Criteria.Customer;
    using DTO.Customer;
    using Fanex.ExternalAccount.DTO;

    public interface ISuperService
    {
        CustomerInfo GetSuperInfo(int superId);

        Result UpdateSuperInfo(UpdateSuperInfoCommand criteria);

    }
}