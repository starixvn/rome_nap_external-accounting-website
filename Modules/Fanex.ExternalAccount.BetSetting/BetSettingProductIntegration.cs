namespace Fanex.CustomerSetting.Module.BetSetting
{
    using ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.Service.Products;
    using System.Collections.Generic;
    using System.Linq;

    public static class BetSettingProductIntegration
    {
        private static ProductStatusService productStatusService = new ProductStatusServiceImpl();

        public static IEnumerable<Product> GetVisibledProductGroupBetSettingTabs(int custId, int roleId, byte settingStatus)
        {
            return productStatusService.GetVisibleProductGroups(custId, roleId, settingStatus).Where(p => p.Id != ProductGroupConstants.Sportsbook);
        }
    }
}