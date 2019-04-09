namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System.Collections.Generic;
    using System.Data;
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using DTO;
    using DTO.Customer;
    using Interface.Customer;

    public class MasterService : CustomerService, IMasterService
    {
        private readonly IMasterDao _dao = new MasterDao();

        public DataTable GetData(int? userId, int? recommend = null, int? custId = null, bool? enabled = null,
            bool? closed = null, bool? suspended = null)
            => _dao.GetData(userId, recommend, custId, enabled, closed, suspended);

        public IEnumerable<int> GetSuperInAutomateTransfer(int custId)
            => _dao.GetSuperInAutomateTransfer(custId);

        public Transfer GetTransferOption(int custId)
            => _dao.GetTransferOption(custId);

        public Result EditTransferOption(string transferOption, int custId, int userId, string ip, int subAccId,
            string language)
            => _dao.EditTransferOption(transferOption, custId, userId, ip, subAccId, language);
    }
}