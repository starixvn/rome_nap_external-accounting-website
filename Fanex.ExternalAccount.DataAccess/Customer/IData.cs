namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using System.Data;

    public interface IData
    {
        DataTable GetData(int? userid, int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null, string groupName = "ACCDDD");
    }
}