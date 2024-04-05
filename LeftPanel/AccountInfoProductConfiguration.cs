namespace Fanex.ExternalAccount.Module.LeftPanel
{
    using System.Collections.Generic;
    using System.Data;
    using Extensions;
    using Core.Shared;
    using Models;
    using ViewModels;

    public static class AccountInfoProductConfiguration
    {
        // Status check box/link, edit setting link
        public static readonly IEnumerable<ProductSettingElement> ProductSettingParamList
            = new List<ProductSettingElement>
            {
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.RNGLottery], StatusType.Link),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.SabaVirtualSports], StatusType.Checkbox),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.Bitcoin], StatusType.Checkbox),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.Cashout], StatusType.Checkbox)
            };

        public static readonly IReadOnlyDictionary<int, string> VisibleBetSettingPermissions =
            new Dictionary<int, string>
            {
                // Show edit bet setting tab for edit super setting
                {ProductConstants.Sportsbook, eFunctions.SportsbookBetting},
                {ProductConstants.RNGLottery, eFunctions.RNGLotteryBetting},
                {ProductConstants.Bitcoin, eFunctions.BitcoinBetting}
            };

        // Get Product status from data, convert to standard structure
        public static List<ProductStatus> TransformToProductStatus(DataRow statusRow, bool isSetUplineStatus = false)
            => new List<ProductStatus>
            {
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.RNGLottery], statusRow, isSetUplineStatus: isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.Bitcoin], statusRow, isSetUplineStatus:isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.Cashout], statusRow, isSetUplineStatus:isSetUplineStatus)
            };

        public static List<ObjectInfo> CreateSyncAndHiddenItems(DataTable dcsProductStatusTable)
            => new List<ObjectInfo>
            {
                new ObjectInfo("HideRNGLottery", dcsProductStatusTable.IsHiddenOrUnsyncProduct(ProductConstants.RNGLottery)),
            };
    }
}