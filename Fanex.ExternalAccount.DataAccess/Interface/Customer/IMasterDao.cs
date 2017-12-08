namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using System.Data;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IMasterDao
    {
        DataTable GetData(int? userId, int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null);

        Transfer GetTransferOption(int custId);

        Result EditTransferOption(string transferOption, int custId, int userId, string ip, int subAccId, string language);
    }
}