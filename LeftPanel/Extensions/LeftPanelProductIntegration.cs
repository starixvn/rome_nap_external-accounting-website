namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Collections.Generic;
    using System.Data;
    using Core.Shared;
    using Models;
    using ViewModels;

    public static class LeftPanelProductIntegration
    {
        public static readonly IEnumerable<ProductSettingParam> ProductSettingParamList = new List<ProductSettingParam>
        {
            new ProductSettingParam(ProductConstants.AllBet, "Allbet", StatusType.Link, false),
            new ProductSettingParam(ProductConstants.VoidBridge, "Macau Games", StatusType.Checkbox, false),
            new ProductSettingParam(ProductConstants.NG2, "NG2", StatusType.Checkbox, true),
            new ProductSettingParam(ProductConstants.FishingWorld, "Fishing World", StatusType.Checkbox, false),
            new ProductSettingParam(ProductConstants.Lottery, "Lottery", StatusType.Checkbox, false),
            new ProductSettingParam(ProductConstants.Lucky3, "Lucky 3", StatusType.Checkbox, false),
            new ProductSettingParam(ProductConstants.BullFighting, "Bullfighting", StatusType.Checkbox, false)
        };

        public static List<ObjectInfo> CreateSyncItems(DataRow customerRow, DataTable table3RdDcs)
             => new List<ObjectInfo>
             {
                new ObjectInfo("SyncCasino", customerRow.IsSyncedCasino(), "Casino Synced"),
                new ObjectInfo("SyncLiveCasino", customerRow.IsSyncedLiveCasino(), "Live Casino Synced"),
                new ObjectInfo("HideAllbet", table3RdDcs.IsHideProduct(ProductConstants.AllBet)),
                new ObjectInfo("HideVoidbridge", table3RdDcs.IsHideProduct(ProductConstants.VoidBridge)),
                new ObjectInfo("HideNG2", table3RdDcs.IsHideProduct(ProductConstants.NG2)),
                new ObjectInfo("HideFishingWorld", table3RdDcs.IsHideProduct(ProductConstants.FishingWorld)),
                new ObjectInfo("HideLottery", table3RdDcs.IsHideProduct(ProductConstants.Lottery)),
                new ObjectInfo("HideLucky3", table3RdDcs.IsHideProduct(ProductConstants.Lucky3)),
                new ObjectInfo("HideBullfighting", table3RdDcs.IsHideProduct(ProductConstants.BullFighting))
             };

        public static List<ProductStatus> CreateProductStatusList(DataRow statusRow)
            => new List<ProductStatus>
            {
                new ProductStatus(ProductConstants.Lucky3, "Lucky3", statusRow),
                new ProductStatus(ProductConstants.Lottery, "Lottery", statusRow),
                new ProductStatus(ProductConstants.FishingWorld, "FishingWorld", statusRow),
                new ProductStatus(ProductConstants.NG2, "NG2", statusRow),
                new ProductStatus(ProductConstants.VoidBridge, "VoidBridge", statusRow, false),
                new ProductStatus(ProductConstants.AllBet, "Allbet", statusRow, false),
                new ProductStatus(ProductConstants.BullFighting, "Bullfighting", statusRow)
            };

        public static readonly IReadOnlyDictionary<int, string> VisibleBetSettingPermissions = new Dictionary<int, string>
        {
            { ProductConstants.RNGCasino, eFunctions.CasinoBetting },
            { ProductConstants.GoldDeluxe,eFunctions.GoldDeluxeBetting },
            { ProductConstants.Keno, eFunctions.KenoBetting },
            { ProductConstants.LiveCasino, eFunctions.LiveCasinoBetting },
            { ProductConstants.Racing, eFunctions.RacingBetting },
            { ProductConstants.Sportsbook, eFunctions.SportsbookBetting },
            { ProductConstants.AllBet, eFunctions.AllbetBetting },
            { ProductConstants.VoidBridge, eFunctions.VoidbridgeBetting },
            { ProductConstants.NG2, eFunctions.NG2Betting },
            { ProductConstants.Lottery, eFunctions.LotteryBetting },
            { ProductConstants.BullFighting, eFunctions.BullfightingBetting }
        };
    }
}