namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using System.Data;

    public interface IDataService
    {
        DataTable GetData(int? userId, string groupName, int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null);
    }
}