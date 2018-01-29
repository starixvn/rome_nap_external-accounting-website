namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Core.Shared;
    using Extensions;
    using Utilities;

    public class BalanceInfo
    {
        public BalanceInfo(bool isUpdateCredit, DataRow customerRow, DataRow cashRow)
        {
            var roleId = customerRow.RoleId();

            CurrencyId = ObjectInfo.CreateInstance("Currency Id", customerRow.CurrencyId());
            Currency = ObjectInfo.CreateInstance("Currency", customerRow.Currency());
            Balances = new List<ObjectInfo>
            {
                ObjectInfo.CreateInstance("Total Cash", Utils.ToDecimal(cashRow[GetCashColumn(roleId)])),
                ObjectInfo.CreateInstance("Yesterday Cash", Utils.ToDecimal(cashRow[GetYesterdayCashColumn(roleId)])),
                ObjectInfo.CreateInstance("Outstanding", Utils.ToDecimal(cashRow[GetOutstandingColumn(roleId)])),
                ObjectInfo.CreateInstance("Given Credit", customerRow.Credit())
            };
            IsUpdateCredit = isUpdateCredit;

            if (roleId.IsNotMember())
            {
                Balances.Add(ObjectInfo.CreateInstance("Used Credit", cashRow[GetUsedCreditColumn(roleId)]));
                Balances.Add(ObjectInfo.CreateInstance("Total Balance", cashRow.TotalBalance(roleId)));
                Balances.Add(ObjectInfo.CreateInstance("Yesterday Balance", cashRow.TotalYesterdayCash(roleId)));

                Balances.Add(ObjectInfo.CreateInstance(
                    $"{roleId.DirectDownlineLevel().DisplayRoleName()} Outstanding",
                    cashRow.CustomerOutstanding()));
            }
        }

        internal static string GetOutstandingColumn(int roleId) =>
            roleId.IsMember() ? "outstanding" : $"{roleId.DisplayRoleName()}PositionTaking";

        internal static string GetYesterdayCashColumn(int roleId)
            => roleId.IsMember() ? "yesterday_cash" : $"yesterday_{roleId.DisplayRoleName()}Cash";

        internal static string GetCashColumn(int roleId)
            => roleId.IsMember() ? "cash" : $"{roleId.DisplayRoleName()}Cash";

        internal static string GetUsedCreditColumn(int roleId)
        {
            if (roleId.IsMember())
            {
                throw new ArgumentOutOfRangeException(nameof(roleId), "there is no UsedCredit on member level");
            }

            return $"{roleId.DisplayRoleName()}UsedCredit";
        }

        public ObjectInfo CurrencyId { get; }

        public ObjectInfo Currency { get; }

        public List<ObjectInfo> Balances { get; }

        //public List<Status> Statuses { get; }

        public bool IsUpdateCredit { get; }
    }
}