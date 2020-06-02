namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using Utilities;

    public static class CashDataRowExtensions
    {
        internal static int CurrencyId(this DataRow dataRow) => Utils.ToInt32(dataRow["CurrencyId"]);

        internal static string Currency(this DataRow dataRow) => Utils.ToString(dataRow["currency"]);

        internal static decimal UsedCredit(this DataRow dataRow) => Utils.ToDecimal(dataRow["UsedCredit"]).Format();

        internal static decimal GivenCredit(this DataRow dataRow) => Utils.ToDecimal(dataRow["Credit"]).Format();

        internal static decimal DownlineOutstanding(this DataRow dataRow) => Utils.ToDecimal(dataRow["DownlineOutstanding"]).Format();

        internal static decimal YesterdayBalance(this DataRow dataRow) => Utils.ToDecimal(dataRow["YesterdayBalance"]).Format();

        internal static decimal TotalBalance(this DataRow dataRow) => Utils.ToDecimal(dataRow["TotalBalance"]).Format();

        internal static decimal Outstanding(this DataRow dataRow) => Utils.ToDecimal(dataRow["Outstanding"]).Format();

        internal static decimal PendingTransfer3rd(this DataRow dataRow) => Utils.ToDecimal(dataRow["PendingTransfer3rd"]).Format();

        internal static decimal YesterdayCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["YesterdayCash"]).Format();

        internal static decimal TotalCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["TotalCash"]).Format();
    }
}
