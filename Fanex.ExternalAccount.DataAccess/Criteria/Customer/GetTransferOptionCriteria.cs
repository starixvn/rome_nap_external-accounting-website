namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetTransferOptionCriteria : CriteriaBase
    {
        public int CustId { get; }
        public string TransferOption { get; }

        public GetTransferOptionCriteria(int custId)
        {
            CustId = custId;
            TransferOption = "00000000";
        }

        public override string GetSettingKey() => "Acc_CustomerSelTransferOption";

        public override bool IsValid() => CustId > 0;
    }
}
