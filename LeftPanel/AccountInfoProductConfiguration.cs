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
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.SabaVirtualSports], StatusType.Checkbox),
                new ProductSettingElement(ProductSettings.SiteProducts[ProductConstants.Cashout], StatusType.Checkbox)
            };

        public static readonly IReadOnlyDictionary<int, string> VisibleBetSettingPermissions =
            new Dictionary<int, string>
            {
                // Show edit bet setting tab for edit super setting
                {ProductConstants.Sportsbook, eFunctions.SportsbookBetting}
            };

        public static List<ObjectInfo> CreateSyncAndHiddenItems(DataTable dcsProductStatusTable)
            => new List<ObjectInfo>
            {
                new ObjectInfo("HideRNGLottery", dcsProductStatusTable.IsHiddenProduct(ProductConstants.RNGLottery)),
            };
    }
}