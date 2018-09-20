namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using System.Data;
    using Criteria.Customer;
    using DTO;

    public interface ISuperDao
    {
        DataTable GetData(int? userId, string groupName, int? custId = null, bool? enabled = null,
            bool? closed = null, bool? suspended = null, bool? withDataLimited = null);

        Result UpdateSuperInfo(UpdateSuperInfoCriteria criteria);
    }
}