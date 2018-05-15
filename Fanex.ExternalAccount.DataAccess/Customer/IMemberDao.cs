namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using System.Data;

    public interface IMemberDao
    {
        DataTable GetData(int? userId, string groupName, int? recommend = null, int? custId = null, bool? enabled = null,
            bool? closed = null, bool? suspended = null);
    }
}