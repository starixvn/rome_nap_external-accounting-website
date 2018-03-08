namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using System.Data;

    public interface IMemberDao
    {
        DataTable GetData(int? userId, int? recommend = null, int? custId = null, bool? enabled = null,
            bool? closed = null, bool? suspended = null, string groupName = "ACCDDD");
    }
}