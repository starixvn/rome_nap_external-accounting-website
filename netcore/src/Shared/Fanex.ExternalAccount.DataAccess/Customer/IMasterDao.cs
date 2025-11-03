namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using System.Collections.Generic;
    using System.Data;
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;

    public interface IMasterDao
    {
        IEnumerable<int> GetSuperInAutomateTransfer(GetSuperInAutomateTransferCriteria criteria);

        Transfer GetTransferOption(GetTransferOptionCriteria criteria);

        Result EditTransferOption(EditTransferOptionCriteria criteria);
    }
}