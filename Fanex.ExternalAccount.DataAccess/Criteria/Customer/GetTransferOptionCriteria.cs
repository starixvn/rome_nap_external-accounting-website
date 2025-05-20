namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data.Repository;

    public class GetTransferOptionCriteria : INonQueryCommand
    {
        public int CustId { get; }

        public string TransferOption { get; set; }

        public GetTransferOptionCriteria(int custId)
        {
            CustId = custId;
            TransferOption = "00000000";
        }

        public string GetSettingKey() => "Acc_CustomerSelTransferOption";

        public bool IsValid() => CustId > 0;
    }
}
