namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using Fanex.ExternalAccount.Core.Shared;
    using System.Data;
    using Fanex.ExternalAccount.Module.LeftPanel.Extensions;

    public class RewardInfo
    {
        public ObjectInfo RewardReceivable { get; }

        public ObjectInfo RewardPayable { get; }

        public ObjectInfo RewardCurrency { get; }

        public RewardInfo(int roleId, DataRow cashRow)
        {
            RewardCurrency = ObjectInfo.CreateInstance("Reward Currency", cashRow.RewardCurrency());
            RewardReceivable = new ObjectInfo
            {
                Text = "Reward Receivable",
                Value = cashRow.RewardReceivable(),
                Tooltip = "Amount you can receive from your upline."
            };
            if (roleId.IsNotMember())
            {
                RewardPayable = new ObjectInfo
                {
                    Text = "Reward Payable",
                    Value = cashRow.RewardPayable(),
                    Tooltip = "Amount you can distribute to your downlines."
                };
            }
        }
    }
}