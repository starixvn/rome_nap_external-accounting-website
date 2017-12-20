namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using System.Data;

    public interface ISuperService 
    {
        DataTable GetData(int? userId, int? custId = null, bool? enabled = null,
            bool? closed = null, bool? suspended = null, string groupName = "ACCDDD");
    }
}