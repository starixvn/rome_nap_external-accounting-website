namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System;
    using System.Data;
    using Core.Shared;
    using Utilities;

    public static class CashDataRowExtensions
    {
        private static decimal SuperCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["superCash"]);

        private static decimal MasterCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["masterCash"]);

        private static decimal AgentCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["agentCash"]);

        private static decimal CustomerCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["customerCash"]);

        internal static decimal Cash(this DataRow dataRow, int role)
        {
            switch (role)
            {
                case Role.Super:
                    return dataRow.SuperCash();

                case Role.Master:
                    return dataRow.MasterCash();

                case Role.Agent:
                    return dataRow.AgentCash();

                case Role.Member:
                    return dataRow.CustomerCash();

                default:
                    throw new ArgumentException(nameof(role));
            }
        }

        private static decimal SuperYesterdayCash(this DataRow dataRow)
            => Utils.ToDecimal(dataRow["yesterday_superCash"]);

        private static decimal MasterYesterdayCash(this DataRow dataRow)
            => Utils.ToDecimal(dataRow["yesterday_masterCash"]);

        private static decimal AgentYesterdayCash(this DataRow dataRow)
            => Utils.ToDecimal(dataRow["yesterday_agentCash"]);

        private static decimal CustomerYesterdayCash(this DataRow dataRow)
            => Utils.ToDecimal(dataRow["yesterday_customerCash"]);

        internal static decimal YesterdayCash(this DataRow dataRow, int role)
        {
            switch (role)
            {
                case Role.Super:
                    return dataRow.SuperYesterdayCash();

                case Role.Master:
                    return dataRow.MasterYesterdayCash();

                case Role.Agent:
                    return dataRow.AgentYesterdayCash();

                case Role.Member:
                    return dataRow.CustomerYesterdayCash();

                default:
                    throw new ArgumentException(nameof(role));
            }
        }

        internal static decimal TotalAgentYesterdayCash(this DataRow dataRow)
            => dataRow.AgentYesterdayCash() + dataRow.CustomerYesterdayCash();

        internal static decimal TotalMasterYesterdayCash(this DataRow dataRow)
            => dataRow.TotalAgentYesterdayCash() + dataRow.MasterYesterdayCash();

        internal static decimal TotalSuperYesterdayCash(this DataRow dataRow)
            => dataRow.TotalMasterYesterdayCash() + dataRow.SuperYesterdayCash();

        internal static decimal TotalYesterdayCash(this DataRow dataRow, int role)
        {
            switch (role)
            {
                case Role.Super:
                    return dataRow.TotalSuperYesterdayCash();

                case Role.Master:
                    return dataRow.TotalMasterYesterdayCash();

                case Role.Agent:
                    return dataRow.TotalAgentYesterdayCash();

                default:
                    throw new ArgumentException(nameof(role));
            }
        }

        internal static decimal CustomerOutstanding(this DataRow dataRow)
            => Utils.ToDecimal(dataRow["customerOutstanding"]);

        internal static DateTime LastTransaction(this DataRow dataRow)
            => Utils.ToDateTime(dataRow["lastTxn"]);

        internal static decimal SuperUsedCredit(this DataRow dataRow)
            => Utils.ToDecimal(dataRow["superUsedCredit"]);

        private static decimal TotalAgentBalance(this DataRow dataRow)
            => dataRow.CustomerCash() + dataRow.AgentCash();

        private static decimal TotalMasterBalance(this DataRow dataRow)
            => dataRow.TotalAgentBalance() + dataRow.MasterCash();

        private static decimal TotalSuperBalance(this DataRow dataRow)
            => dataRow.TotalMasterBalance() + dataRow.SuperCash();

        internal static decimal TotalBalance(this DataRow dataRow, int role)
        {
            switch (role)
            {
                case Role.Super:
                    return dataRow.TotalSuperBalance();

                case Role.Master:
                    return dataRow.TotalMasterBalance();

                case Role.Agent:
                    return dataRow.TotalAgentBalance();

                default:
                    throw new ArgumentException(nameof(role));
            }
        }
    }
}
