namespace Fanex.ExternalAccount.Module.LeftPanel.Models
{
    public class MemberBalanceInfoModel
    {
        public MemberBalanceInfoModel(decimal balance, decimal netPosition)
        {
            Balance = balance;
            NetPosition = netPosition;
        }

        public decimal Balance { get; }

        public decimal NetPosition { get; }
    }
}