namespace Fanex.CustomerSetting.Module.BetSetting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ExternalAccount.Core.Shared;
    using ExternalAccount.Module.LeftPanel.Services;
    using ViewModels;

    public static class BetSettingProductIntegration
    {
        private static readonly IReadOnlyList<Product> BetSettingTabProductConfigs = new List<Product>
        {
            ProductSettings.SiteProducts[ProductConstants.AllBet],
            ProductSettings.SiteProducts[ProductConstants.VoidBridge],
            ProductSettings.SiteProducts[ProductConstants.NG2],
            ProductSettings.SiteProducts[ProductConstants.Lottery],
            ProductSettings.SiteProducts[ProductConstants.BullFighting],
            ProductSettings.SiteProducts[ProductConstants.Sportsbook2]
        };

        private static CustomerStatusQueryService customerStatusService = new CustomerStatusServiceImpl();

        public static IList<Product> GetVisibledBetSettingTabs(int userId, string siteName, string custName)
        {
            var customerStatus = customerStatusService.GetCustomerStatus(
                userId,
                siteName,
                custName).ProductStatusDic;
            var visibleProductTabs = new List<Product>();

            foreach (var productTab in BetSettingTabProductConfigs)
            {
                if(customerStatus[productTab.Id].IsShow)
                {
                    visibleProductTabs.Add(productTab);
                }
            }

            return visibleProductTabs;
        }
    }
}
