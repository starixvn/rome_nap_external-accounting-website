namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using Criteria.Customer;
    using DTO;

    public interface ISuperDao
    {
        Result UpdateSuperInfo(UpdateSuperInfoCommand command);
    }
}