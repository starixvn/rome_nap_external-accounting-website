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
        private static readonly IReadOnlyList<BetSettingTab> BetSettingTabConfigs = new List<BetSettingTab>
        {
            new BetSettingTab(ProductConstants.AllBet, "Allbet"),
            new BetSettingTab(ProductConstants.VoidBridge, "Macau Games"),
            new BetSettingTab(ProductConstants.NG2, "NG2", true),
            new BetSettingTab(ProductConstants.Lottery, "Lottery"),
            new BetSettingTab(ProductConstants.BullFighting, "Bullfighting")
        };

        private static CustomerStatusService customerStatusService = new CustomerStatusServiceImpl();

        public static IList<BetSettingTab> GetVisibledBetSettingTabs(int userId, string siteName, string custName)
        {
            var customerStatus = customerStatusService.GetCustomerStatus(
                userId,
                siteName.ToString(),
                custName,
                0).ProductStatusDic;
            var visibleTabs = new List<BetSettingTab>();

            foreach (var bettingTab in BetSettingTabConfigs)
            {
                if(customerStatus[bettingTab.ProductId].IsShow)
                {
                    visibleTabs.Add(bettingTab);
                }
            }

            return visibleTabs;
        }
    }
}
