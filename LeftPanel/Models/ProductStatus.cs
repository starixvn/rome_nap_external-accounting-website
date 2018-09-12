namespace Fanex.ExternalAccount.Module.LeftPanel.Models
{
    using System.Data;
    using Core.Shared;
    using Utilities;

    public class ProductStatus
    {
        private ProductStatus()
        {
        }

        public ProductStatus(Product product, DataRow dataRow, bool checkSyncedDownlines = true, bool isSetUplineStatus = false)
        {
            ProductId = product.Id;
            ProductKey = product.Key;
            CheckSyncedDownlines = checkSyncedDownlines;
            SetProductStatus(dataRow);

            if (isSetUplineStatus)
            {
                SetUplineProductStatus(dataRow);
            }
        }

        public int ProductId { get; }

        public string ProductKey { get; }

        public bool IsDisabled { get; set; }

        public bool IsHidden { get; set; }

        public bool IsSynced { get; set; }

        public bool IsSyncedDownlines { get; set; }

        public bool IsShow => IsSynced && !IsHidden;

        public ProductStatus Upline { get; set; }

        public bool CheckSyncedDownlines { get; }

        public void SetProductStatus(DataRow dataRow)
        {
            IsDisabled = GetDisabledStatus(dataRow);
            IsHidden = !ProductSettings.IsShowProduct(ProductId) || dataRow.ToBoolean($"Hide{ProductKey}");
            IsSynced = dataRow.ToBoolean($"Synced{ProductKey}", true);

            if (CheckSyncedDownlines)
            {
                IsSyncedDownlines = dataRow.ToBoolean($"Synced{ProductKey}Downline", true);
            }
        }

        private void SetUplineProductStatus(DataRow dataRow)
        {
            Upline = Upline ?? new ProductStatus();

            Upline.IsDisabled = dataRow.ToBoolean($"Disable{ProductKey}Upline");
            Upline.IsHidden = !ProductSettings.IsShowProduct(ProductId) || dataRow.ToBoolean($"Hide{ProductKey}Upline");
            Upline.IsSynced = dataRow.ToBoolean($"Synced{ProductKey}Upline", true);
        }

        private bool GetDisabledStatus(DataRow dataRow)
        {
            if (ProductId == ProductConstants.PokerGames)
            {
                return dataRow.ToInt32($"DisablePokerGames1401")
                       + dataRow.ToInt32($"DisablePokerGames1402")
                       + dataRow.ToInt32($"DisablePokerGames1403")
                       + dataRow.ToInt32($"DisablePokerGames1404")
                       + dataRow.ToInt32($"DisablePokerGames1405") > 0;
            }

            return dataRow.ToBoolean($"Disable{ProductKey}");
        }
    }
}