namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using System.Collections.Generic;
    using System.Data;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;

    public interface IMasterService
    {
        DataTable GetData(int? userId, int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null);

        Transfer GetTransferOption(int custId);

        IEnumerable<int> GetSuperInAutomateTransfer(int custId);

        Result EditTransferOption(string transferOption, int custId, int userId, string ip, int subAccId, string language);
    }
}