namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using Criteria.Customer;
    using DTO;
    using Fanex.ExternalAccount.DTO.Customer;
    using System.Data;

    public interface IAgentDao
    {
        DataTable GetData(int? userId, int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null);

        Transfer GetTransferOption(int custId);

        Result IsLimitMemberCreationBySuper(CheckMemberCreationCriteria criteria);
    }
}