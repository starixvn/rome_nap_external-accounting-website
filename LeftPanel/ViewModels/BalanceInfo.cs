namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using Core.Shared;
    using Extensions;
    using System.Collections.Generic;
    using System.Data;

    public class BalanceInfo
    {
        public BalanceInfo(
            bool isUpdateCredit, 
            int roleId, 
            DataRow cashRow,
            decimal balance,
            decimal memberOustanding)
        {
            CurrencyId = ObjectInfo.CreateInstance("Currency Id", cashRow.CurrencyId());
            Currency = ObjectInfo.CreateInstance("Currency", cashRow.Currency());

            Balances = new List<ObjectInfo>
            {
                ObjectInfo.CreateInstance("Total Cash", roleId.IsMember() ? balance + memberOustanding : cashRow.TotalCash()),
                ObjectInfo.CreateInstance("Yesterday Cash", cashRow.YesterdayCash()),
                ObjectInfo.CreateInstance("Outstanding", roleId.IsMember() ? memberOustanding : cashRow.Outstanding()),
                ObjectInfo.CreateInstance("Given Credit", cashRow.Credit())
            };
            IsUpdateCredit = isUpdateCredit;

            if (roleId.IsNotMember())
            {
                Balances.Add(ObjectInfo.CreateInstance("Used Credit", cashRow.UsedCredit()));
                Balances.Add(ObjectInfo.CreateInstance("Total Balance", cashRow.TotalBalance()));
                Balances.Add(ObjectInfo.CreateInstance("Yesterday Balance", cashRow.YesterdayBalance()));
                Balances.Add(ObjectInfo.CreateInstance(
                    $"{roleId.DirectDownlineLevel().DisplayRoleName()} Outstanding",
                    cashRow.DownlineOutstanding()));
            }
        }

        public ObjectInfo CurrencyId { get; }

        public ObjectInfo Currency { get; }

        public List<ObjectInfo> Balances { get; }

        public bool IsUpdateCredit { get; }
    }
}