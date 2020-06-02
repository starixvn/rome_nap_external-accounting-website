namespace Fanex.ExternalAccount.Module.LeftPanel.Models
{
    public class MemberBalanceInfoModel
    {
        public MemberBalanceInfoModel()
        {

        }

        public MemberBalanceInfoModel(decimal balance, decimal pendingTransfer3rd, decimal netPosition)
        {
            Balance = balance;
            NetPosition = netPosition;
            PendingTransfer3rd = pendingTransfer3rd;
        }

        public decimal Balance { get; }

        public decimal NetPosition { get; }

        public decimal PendingTransfer3rd { get; }
    }
}