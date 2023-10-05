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
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.LiveCasino], StatusType.Checkbox),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.RNGSlot], StatusType.Link),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.RNGLottery], StatusType.Link),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.Sportsbook2], StatusType.OnlyDetail),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.VirtualGames], StatusType.Link),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.Sabaclub], StatusType.Link),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.SabaVirtualSports], StatusType.Checkbox),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.VGaming], StatusType.Link),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.Bitcoin], StatusType.Checkbox),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.Cashout], StatusType.Checkbox)
            };

        public static readonly IReadOnlyDictionary<int, string> VisibleBetSettingPermissions =
            new Dictionary<int, string>
            {
                // Show edit bet setting tab for edit super setting
                {ProductConstants.Casino, eFunctions.CasinoBetting},
                {ProductConstants.Sportsbook, eFunctions.SportsbookBetting},
                {ProductConstants.LiveCasino, eFunctions.LiveCasinoBetting},
                {ProductConstants.RNGSlot, eFunctions.VoidbridgeBetting},
                {ProductConstants.RNGLottery, eFunctions.RNGLotteryBetting},
                {ProductConstants.VirtualGames, eFunctions.VirtualGamesBetting},
                {ProductConstants.Bitcoin, eFunctions.BitcoinBetting}
            };

        // Get Product status from data, convert to standard structure
        public static List<ProductStatus> TransformToProductStatus(DataRow statusRow, bool isSetUplineStatus = false)
            => new List<ProductStatus>
            {
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.RNGLottery], statusRow, isSetUplineStatus: isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.RNGSlot], statusRow, false, isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.LiveCasino], statusRow, false, isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.Sportsbook2], statusRow, isSetUplineStatus:isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.VirtualGames], statusRow, isSetUplineStatus:isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.Sabaclub], statusRow, isSetUplineStatus:isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.VGaming], statusRow, isSetUplineStatus:isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.Bitcoin], statusRow, isSetUplineStatus:isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.Cashout], statusRow, isSetUplineStatus:isSetUplineStatus)
            };

        public static List<ObjectInfo> CreateSyncAndHiddenItems(DataTable dcsProductStatusTable)
            => new List<ObjectInfo>
            {
                new ObjectInfo("HideLiveCasino", dcsProductStatusTable.IsHiddenOrUnsyncProduct(ProductConstants.LiveCasino)),
                new ObjectInfo("HideVoidbridge", dcsProductStatusTable.IsHiddenOrUnsyncProduct(ProductConstants.RNGSlot)),              
                new ObjectInfo("HideRNGLottery", dcsProductStatusTable.IsHiddenOrUnsyncProduct(ProductConstants.RNGLottery)),
                new ObjectInfo("HideVirtualGames", dcsProductStatusTable.IsProductHidden(ProductConstants.VirtualGames)),
                new ObjectInfo("HideSabaclub", dcsProductStatusTable.IsProductHidden(ProductConstants.Sabaclub)),
                new ObjectInfo("HideVGaming", dcsProductStatusTable.IsProductHidden(ProductConstants.VGaming))
            };
    }
}