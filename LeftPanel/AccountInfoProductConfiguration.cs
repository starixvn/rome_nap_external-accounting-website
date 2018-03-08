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
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.BullFighting], StatusType.Checkbox)
            };

        public static readonly IReadOnlyDictionary<int, string> VisibleBetSettingPermissions =
            new Dictionary<int, string>
            {
                // Show edit bet setting tab for edit super setting
                {ProductConstants.RNGCasino, eFunctions.CasinoBetting},
                {ProductConstants.GoldDeluxe, eFunctions.GoldDeluxeBetting},
                {ProductConstants.Keno, eFunctions.KenoBetting},
                {ProductConstants.LiveCasino, eFunctions.LiveCasinoBetting},
                {ProductConstants.Racing, eFunctions.RacingBetting},
                {ProductConstants.Sportsbook, eFunctions.SportsbookBetting},
                {ProductConstants.AllBet, eFunctions.AllbetBetting},
                {ProductConstants.VoidBridge, eFunctions.VoidbridgeBetting},
                {ProductConstants.NG2, eFunctions.NG2Betting},
                {ProductConstants.Lottery, eFunctions.LotteryBetting},
                {ProductConstants.BullFighting, eFunctions.BullfightingBetting}
            };

        // Get Product status from data, convert to standard structure
        public static List<ProductStatus> TransformToProductStatus(DataRow statusRow)
            => new List<ProductStatus>
            {
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.Lucky3], statusRow),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.Lottery], statusRow),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.FishingWorld], statusRow),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.NG2], statusRow),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.VoidBridge], statusRow, false),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.AllBet], statusRow, false),
                new ProductStatus(ProductSettings.SiteProducts[ProductConstants.BullFighting], statusRow)
            };

        public static List<ObjectInfo> CreateSyncAndHiddenItems(DataRow customerRow, DataTable table3RdDcs)
            => new List<ObjectInfo>
            {
                new ObjectInfo("SyncCasino", customerRow.IsSyncedCasino(), "Casino Synced"),
                new ObjectInfo("SyncLiveCasino", customerRow.IsSyncedLiveCasino(), "Live Casino Synced"),
                new ObjectInfo("HideAllbet", table3RdDcs.IsHiddenProduct(ProductConstants.AllBet)),
                new ObjectInfo("HideVoidbridge", table3RdDcs.IsHiddenProduct(ProductConstants.VoidBridge)),
                new ObjectInfo("HideNG2", table3RdDcs.IsHiddenProduct(ProductConstants.NG2)),
                new ObjectInfo("HideFishingWorld", table3RdDcs.IsHiddenProduct(ProductConstants.FishingWorld)),
                new ObjectInfo("HideLottery", table3RdDcs.IsHiddenProduct(ProductConstants.Lottery)),
                new ObjectInfo("HideLucky3", table3RdDcs.IsHiddenProduct(ProductConstants.Lucky3)),
                new ObjectInfo("HideBullfighting", table3RdDcs.IsHiddenProduct(ProductConstants.BullFighting))
            };
    }
}