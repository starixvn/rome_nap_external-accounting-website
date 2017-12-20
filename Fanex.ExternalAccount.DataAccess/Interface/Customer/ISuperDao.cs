namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using System.Data;

    public interface ISuperDao
    {
        DataTable GetData(int? userId, int? custId = null, bool? enabled = null,
            bool? closed = null, bool? suspended = null, string groupName = "ACCDDD");
    }
}