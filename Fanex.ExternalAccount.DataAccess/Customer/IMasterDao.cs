namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DTO.Customer;

    public interface IMasterDao
    {
        DataTable GetData(int? userId, int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null);

        IEnumerable<int> GetSuperInAutomateTransfer(int custId);

        Transfer GetTransferOption(int custId);

        DbResult EditTransferOption(string transferOption, int autoTransferCondition, int custId, int userId, string ip, int subAccId, string language);
    }
}