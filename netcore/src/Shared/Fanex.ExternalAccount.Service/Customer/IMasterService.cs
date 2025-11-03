namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;

    public interface IMasterService
    {
        Transfer GetTransferOption(int custId);

        IEnumerable<int> GetSuperInAutomateTransfer(int custId);

        Result EditTransferOption(string transferOption, int autoTransferCondition, int custId, int userId, string ip, int subAccId, string language);
    }
}