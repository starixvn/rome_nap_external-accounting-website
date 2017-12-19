namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using System.Linq;
    using Utilities;

    internal static class Extensions
    {
        internal static int SuperId(this DataRow dataRow) => Utils.ToInt32(dataRow["superID"]);

        internal static int MasterId(this DataRow dataRow) => Utils.ToInt32(dataRow["masterID"]);

        internal static int AgentId(this DataRow dataRow) => Utils.ToInt32(dataRow["agentID"]);

        internal static string SuperName(this DataRow dataRow) => Utils.ToString(dataRow["superName"]);

        internal static string MasterName(this DataRow dataRow) => Utils.ToString(dataRow["masterName"]);

        internal static string AgentName(this DataRow dataRow) => Utils.ToString(dataRow["agentName"]);

        internal static string MemberName(this DataRow dataRow) => Utils.ToString(dataRow["memberName"]);

        internal static bool Closed(this DataRow dataRow) => Utils.ToBoolean(dataRow["closed"]);

        internal static bool Suspended(this DataRow dataRow) => Utils.ToBoolean(dataRow["suspended"]);

        internal static bool IsHidden(this DataRow dataRow) => Utils.ToBoolean(dataRow["IsHidden"]);

        internal static bool IsSynced(this DataRow dataRow) => Utils.ToBoolean(dataRow["IsSync"]);

        internal static bool IsDisabled(this DataRow dataRow) => Utils.ToBoolean(dataRow["isDisabled"]);

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

        internal static bool EnableHorseRacingFixedOdds(this DataRow dataRow) => !Utils.ToBoolean(dataRow["DisableHRFixedOdds"]);

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

        internal static decimal TotalAgentBalance(this DataRow dataRow)
            => dataRow.CustomerCash() + dataRow.AgentCash();

        internal static decimal TotalMasterBalance(this DataRow dataRow)
            => dataRow.TotalAgentBalance() + dataRow.MasterCash();

        internal static decimal TotalSuperBalance(this DataRow dataRow)
            => dataRow.TotalMasterBalance() + dataRow.SuperCash();

        internal static decimal SuperYesterdayCash(this DataRow dataRow)
            => Utils.ToDecimal(dataRow["yesterday_superCash"]);

        internal static decimal MasterYesterdayCash(this DataRow dataRow)
            => Utils.ToDecimal(dataRow["yesterday_masterCash"]);

        internal static decimal AgentYesterdayCash(this DataRow dataRow)
            => Utils.ToDecimal(dataRow["yesterday_agentCash"]);

        internal static decimal CustomerYesterdayCash(this DataRow dataRow)
            => Utils.ToDecimal(dataRow["yesterday_customerCash"]);

        internal static decimal TotalAgentYesterdayCash(this DataRow dataRow)
            => dataRow.AgentYesterdayCash() + dataRow.CustomerYesterdayCash();

        internal static decimal TotalMasterYesterdayCash(this DataRow dataRow)
            => dataRow.TotalAgentYesterdayCash() + dataRow.MasterYesterdayCash();

        internal static decimal TotalSuperYesterdayCash(this DataRow dataRow)
            => dataRow.TotalMasterYesterdayCash() + dataRow.SuperYesterdayCash();

        internal static decimal CustomerOutstanding(this DataRow dataRow)
            => Utils.ToDecimal(dataRow["customerOutstanding"]);

        private static DataRow GetProductRow(DataTable table3RdDcs, int productId)
            => table3RdDcs.Select($"ProductID = {productId}").FirstOrDefault();

        internal static bool Get3rdProductStatus(this DataTable table3RdDcs, int productId)
        {
            var productRow = GetProductRow(table3RdDcs, productId);

            return productRow == null || productRow.IsHidden() || productRow.IsSynced();
        }

        internal static bool GetNG2ProductStatus(this DataTable table3RdDcs)
        {
            var productRow = GetProductRow(table3RdDcs, 23);

            return productRow == null || !Utils.ToBoolean(productRow["IsSync"]);
        }
        internal static bool GetVirtualSportsStatus(this DataTable table3rdDCS)
        {
            var drowVirtualSports = table3rdDCS.Select("ProductID = 5");

            return drowVirtualSports.Length > 0 && !Utils.ToBoolean(drowVirtualSports[0]["isDisabled"]);
        }
    }
}