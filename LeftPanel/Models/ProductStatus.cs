namespace Fanex.ExternalAccount.Module.LeftPanel.Models
{
    using System.Data;
    using Core.Shared;
    using Utilities;

    public class ProductStatus
    {
        public ProductStatus(int productId, string productKey, DataRow dataRow, bool checkSyncedDownlines = true)
        {
            ProductId = productId;
            ProductKey = productKey;
            CheckSyncedDownlines = checkSyncedDownlines;
            SetProductStatus(dataRow);
        }

        public int ProductId { get; }

        public string ProductKey { get; }

        public bool IsDisabled { get; set; }

        public bool IsHidden { get; set; }

        public bool IsSynced { get; set; }

        public bool IsSyncedDownlines { get; set; }

        public void SetProductStatus(DataRow dataRow)
        {
            IsDisabled = dataRow.ToBoolean($"Disable{ProductKey}");
            IsHidden = !AppSettings.IsShowProduct(ProductId) || dataRow.ToBoolean($"Hide{ProductKey}");
            IsSynced = dataRow.ToBoolean($"Synced{ProductKey}", true);

            if (CheckSyncedDownlines)
            {
                IsSyncedDownlines = dataRow.ToBoolean($"Synced{ProductKey}Downline", true);
            }
        }

        public bool IsShow => IsSynced && !IsHidden;

        public ProductStatus Upline { get; set; }

        public bool CheckSyncedDownlines { get; }
    }
}