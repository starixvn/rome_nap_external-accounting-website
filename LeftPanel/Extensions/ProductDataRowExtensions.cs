namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using System.Linq;
    using Core.Shared;
    using Utilities;

    internal static class ProductDataRowExtensions
    {
        internal static bool DisableOutright(this DataRow dataRow) => Utils.ToBoolean(dataRow["disableOutright"]);

        internal static bool DisableOutrightUpline(this DataRow dataRow) => Utils.ToBoolean(dataRow["upLineDisableOutright"]);

        internal static bool EnableOutright(this DataRow dataRow) => !DisableOutright(dataRow);

        internal static bool EnableAutoMasterPT(this DataRow dataRow) => !Utils.ToBoolean(dataRow["disableAutoMPT"]);

        internal static bool EnableDoubleComm(this DataRow dataRow) => Utils.ToBoolean(dataRow["doubleCommLosing"]);

        internal static bool EnableDoubleCommUpline(this DataRow dataRow) => Utils.ToBoolean(dataRow["uplineDoubleCommLosing"], true);

        internal static bool DisableTransfer(this DataRow dataRow) => Utils.ToBoolean(dataRow["disableTransfer"]);

        internal static bool HideVirtualSports(this DataRow dataRow) => Utils.ToBoolean(dataRow["hideVirtualSports"]);

        internal static bool DisableVirtualSports(this DataRow dataRow) => Utils.ToBoolean(dataRow["disableVirtualSports"]);

        internal static bool IsHiddenUpline(this DataRow dataRow) => Utils.ToBoolean(dataRow["isHiddenUpline"]);

        internal static bool IsDisabledUpline(this DataRow dataRow) => Utils.ToBoolean(dataRow["isDisabledUpline"]);

        internal static bool IsSyncUpline(this DataRow dataRow) => Utils.ToBoolean(dataRow["isSyncedUpline"]);

        private static DataRow GetProductRow(DataTable table3RdDcs, int productId)
            => table3RdDcs.Select($"ProductID = {productId}").FirstOrDefault();

        internal static bool IsHiddenOrUnsyncProduct(this DataTable productTable, int productId)
        {
            var productRow = GetProductRow(productTable, productId);

            return productRow == null
                   || productRow.IsHidden()
                   || !productRow.IsSync();
        }
     
        internal static bool IsProductHidden(this DataTable productTable, int productId)
        {
            var productRows = productTable.Select($"ProductID = {productId}");
            return productRows.Length == 0
                    || productRows[0].IsHidden();
        }

        internal static bool IsProductHiddenUpline(this DataTable productTable, int productId)
        {
            var productRows = productTable.Select($"ProductID = {productId}");

            return productRows.Length == 0
                    || productRows[0].IsHiddenUpline();
        }

        internal static bool IsProductDisabled(this DataTable productTable, int productId)
        {
            var productRows = productTable.Select($"ProductID = {productId}");

            return productRows.Length == 0
                   || productRows[0].IsDisabled();
        }

        internal static bool IsProductDisabledUpline(this DataTable productTable, int productId)
        {
            var productRows = productTable.Select($"ProductID = {productId}");

            return productRows.Length == 0
                   || productRows[0].IsDisabledUpline();
        }

        internal static bool IsProductSynced(this DataTable productTable, int productId)
        {
            var productRows = productTable.Select($"ProductID = {productId}");

            return productRows.Length > 0
                   && productRows[0].IsSync();
        }

        internal static bool IsProductSyncedUpline(this DataTable productTable, int productId)
        {
            var productRows = productTable.Select($"ProductID = {productId}");

            return productRows.Length > 0
                   && productRows[0].IsSyncUpline();
        }

        internal static bool IsProductHasDownline(this DataTable productTable, int productId)
        {
            var productRows = productTable.Select($"ProductID = {productId}");

            return productRows.Length > 0
                   && productRows[0].HasDownline();
        }
    }
}