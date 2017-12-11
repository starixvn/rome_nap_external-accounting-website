namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using System.Data;
    using Fanex.ExternalAccount.DTO.Customer;

    public interface IAgentService
    {
        DataTable GetData(int? userId, int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null);

        Transfer GetTransferOption(int custId);
    }
}