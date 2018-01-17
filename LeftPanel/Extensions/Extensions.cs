namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System;
    using System.Data;
    using System.Linq;
    using Core.Shared;
    using Utilities;

    internal static class Extensions
    {
        internal static bool DisableOutright(this DataRow dataRow) => Utils.ToBoolean(dataRow["disableOutright"]);

        internal static bool EnableOutright(this DataRow dataRow) => !DisableOutright(dataRow);

        internal static bool DisableCasino(this DataRow dataRow) => Utils.ToBoolean(dataRow["disableCasino"]);

        internal static bool EnableCasino(this DataRow dataRow) => !DisableCasino(dataRow);

        internal static bool DisableNumberGame(this DataRow dataRow) => Utils.ToBoolean(dataRow["disableNumber"]);

        internal static bool EnableNumberGame(this DataRow dataRow) => !DisableNumberGame(dataRow);

        internal static bool DisableLiveCasino(this DataRow dataRow) => Utils.ToBoolean(dataRow["disableLiveCasino"]);

        internal static bool EnableLiveCasino(this DataRow dataRow) => !DisableLiveCasino(dataRow);

        internal static bool EnableAutoMasterPT(this DataRow dataRow) => !Utils.ToBoolean(dataRow["disableAutoMPT"]);

        internal static bool EnableDoubleComm(this DataRow dataRow) => Utils.ToBoolean(dataRow["doubleCommLosing"]);

        internal static bool EnableHorseRacingFixedOdds(this DataRow dataRow) =>
            !Utils.ToBoolean(dataRow["DisableHRFixedOdds"]);

        internal static bool DisableESports(this DataRow dataRow) => Utils.ToBoolean(dataRow["disableESports"]);

        internal static bool EnableESports(this DataRow dataRow) => !DisableESports(dataRow);

        internal static bool DisableColossusBet(this DataRow dataRow) => Utils.ToBoolean(dataRow["DisableColossusBet"]);

        internal static bool EnableColossusBet(this DataRow dataRow) => !dataRow.DisableColossusBet();

        internal static bool EnableGoldDeluxe(this DataRow dataRow) => !Utils.ToBoolean(dataRow["DisableGoldDeluxe"]);

        internal static bool IsSyncedCasino(this DataRow dataRow) => Utils.ToBoolean(dataRow["syncCasino"]);

        internal static bool IsSyncedLiveCasino(this DataRow dataRow) => Utils.ToBoolean(dataRow["syncLiveCasino"]);

        internal static decimal SuperCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["superCash"]);

        internal static decimal MasterCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["masterCash"]);

        internal static decimal AgentCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["agentCash"]);

        internal static decimal CustomerCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["customerCash"]);

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

        internal static decimal TotalAgentBalance(this DataRow dataRow)
            => dataRow.CustomerCash() + dataRow.AgentCash();

        internal static decimal TotalMasterBalance(this DataRow dataRow)
            => dataRow.TotalAgentBalance() + dataRow.MasterCash();

        internal static decimal TotalSuperBalance(this DataRow dataRow)
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

        internal static decimal SuperYesterdayCash(this DataRow dataRow)
            => Utils.ToDecimal(dataRow["yesterday_superCash"]);

        internal static decimal MasterYesterdayCash(this DataRow dataRow)
            => Utils.ToDecimal(dataRow["yesterday_masterCash"]);

        internal static decimal AgentYesterdayCash(this DataRow dataRow)
            => Utils.ToDecimal(dataRow["yesterday_agentCash"]);

        internal static decimal CustomerYesterdayCash(this DataRow dataRow)
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

        private static DataRow GetProductRow(DataTable table3RdDcs, int productId)
            => table3RdDcs.Select($"ProductID = {productId}").FirstOrDefault();

        internal static bool IsHiddenProduct(this DataTable productTable, int productId)
        {
            var productRow = GetProductRow(productTable, productId);

            return productRow == null
                   || productRow.IsHidden()
                   || !productRow.IsSync();
        }

        internal static bool GetNG2ProductStatus(this DataTable table3RdDcs)
        {
            var productRow = GetProductRow(table3RdDcs, ProductConstants.NG2);

            return productRow == null || !productRow.IsSync();
        }

        internal static bool GetVirtualSportsStatus(this DataTable table3rdDCS)
            => table3rdDCS.IsProductEnabled(ProductConstants.VirtualSports);

        internal static bool IsProductEnabled(this DataTable productTable, int productId)
        {
            var productRows = productTable.Select($"ProductID = {productId}");

            return productRows.Length > 0
                   && !productRows[0].IsDisabled();
        }

        internal static bool IsProductSynced(this DataTable productTable, int productId)
        {
            var productRows = productTable.Select($"ProductID = {productId}");

            return productRows.Length > 0
                   && productRows[0].IsSync();
        }
    }
}