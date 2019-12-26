namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using System.Data;
    using Criteria.Customer;
    using DTO;

    public interface ISuperDao
    {
        Result UpdateSuperInfo(UpdateSuperInfoCriteria criteria);
    }
}