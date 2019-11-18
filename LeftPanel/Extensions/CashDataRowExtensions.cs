namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using Utilities;

    public static class CashDataRowExtensions
    {
        internal static int CurrencyId(this DataRow dataRow) => Utils.ToInt32(dataRow["CurrencyId"]);

        internal static string Currency(this DataRow dataRow) => Utils.ToString(dataRow["currency"]);

        internal static decimal UsedCredit(this DataRow dataRow) => Utils.ToDecimal(dataRow["UsedCredit"]);

        internal static decimal GivenCredit(this DataRow dataRow) => Utils.ToDecimal(dataRow["Credit"]);

        internal static decimal DownlineOutstanding(this DataRow dataRow) => Utils.ToDecimal(dataRow["DownlineOutstanding"]);

        internal static decimal YesterdayBalance(this DataRow dataRow) => Utils.ToDecimal(dataRow["YesterdayBalance"]);

        internal static decimal TotalBalance(this DataRow dataRow) => Utils.ToDecimal(dataRow["TotalBalance"]);

        internal static decimal Outstanding(this DataRow dataRow) => Utils.ToDecimal(dataRow["Outstanding"]);

        internal static decimal YesterdayCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["YesterdayCash"]);

        internal static decimal TotalCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["TotalCash"]);
    }
}
