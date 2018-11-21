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
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.AllBet], StatusType.Link),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.VoidBridge], StatusType.Checkbox),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.NG2], StatusType.Checkbox),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.FishingWorld], StatusType.Checkbox),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.Lottery], StatusType.Checkbox),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.Lucky3], StatusType.Checkbox),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.BullFighting], StatusType.Checkbox),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.PokerGames], StatusType.Link),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.Sportsbook2], StatusType.OnlyDetail),
            };

        public static readonly IReadOnlyDictionary<int, string> VisibleBetSettingPermissions =
            new Dictionary<int, string>
            {
                // Show edit bet setting tab for edit super setting
                {ProductConstants.RNGCasino, eFunctions.CasinoBetting},
                {ProductConstants.GoldDeluxe, eFunctions.GoldDeluxeBetting},
                {ProductConstants.Keno, eFunctions.KenoBetting},
                {ProductConstants.Sportsbook, eFunctions.SportsbookBetting},
                {ProductConstants.AllBet, eFunctions.AllbetBetting},
                {ProductConstants.VoidBridge, eFunctions.VoidbridgeBetting},
                {ProductConstants.NG2, eFunctions.NG2Betting},
                {ProductConstants.Lottery, eFunctions.LotteryBetting},
                {ProductConstants.BullFighting, eFunctions.BullfightingBetting}
            };

        // Get Product status from data, convert to standard structure
        public static List<ProductStatus> TransformToProductStatus(DataRow statusRow, bool isSetUplineStatus = false)
            => new List<ProductStatus>
            {
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.Lucky3], statusRow, isSetUplineStatus: isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.Lottery], statusRow, isSetUplineStatus: isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.FishingWorld], statusRow, isSetUplineStatus: isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.NG2], statusRow, isSetUplineStatus: isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.VoidBridge], statusRow, false, isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.AllBet], statusRow, false, isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.BullFighting], statusRow, isSetUplineStatus: isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.PokerGames], statusRow, isSetUplineStatus:isSetUplineStatus),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.Sportsbook2], statusRow, isSetUplineStatus:isSetUplineStatus)
            };

        public static List<ObjectInfo> CreateSyncAndHiddenItems(DataRow nonDCSProductStatusRow,DataTable dcsProductStatusTable)
            => new List<ObjectInfo>
            {
                new ObjectInfo("SyncCasino", nonDCSProductStatusRow.IsSyncedCasino(), "Casino Synced"),
                new ObjectInfo("HideAllbet", dcsProductStatusTable.IsHiddenOrUnsyncProduct(ProductConstants.AllBet)),
                new ObjectInfo("HideVoidbridge", dcsProductStatusTable.IsHiddenOrUnsyncProduct(ProductConstants.VoidBridge)),
                new ObjectInfo("HideNG2", dcsProductStatusTable.IsHiddenOrUnsyncProduct(ProductConstants.NG2)),
                new ObjectInfo("HideFishingWorld", dcsProductStatusTable.IsHiddenOrUnsyncProduct(ProductConstants.FishingWorld)),
                new ObjectInfo("HideLottery", dcsProductStatusTable.IsHiddenOrUnsyncProduct(ProductConstants.Lottery)),
                new ObjectInfo("HideLucky3", dcsProductStatusTable.IsHiddenOrUnsyncProduct(ProductConstants.Lucky3)),
                new ObjectInfo("HideBullfighting", dcsProductStatusTable.IsHiddenOrUnsyncProduct(ProductConstants.BullFighting)),
                new ObjectInfo("HidePokerGames", dcsProductStatusTable.IsHiddenOrUnsyncProduct(ProductConstants.PokerGames))
            };
    }
}