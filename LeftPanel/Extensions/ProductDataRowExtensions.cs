namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using System.Linq;
    using Core.Shared;
    using Utilities;

    internal static class ProductDataRowExtensions
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