namespace Fanex.CustomerSetting.Module.BetSetting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ExternalAccount.Core.Shared;
    using ExternalAccount.Module.LeftPanel.Services;
    using Fanex.ExternalAccount.Service.Customer.CustomerList;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Service.Products;
    using ViewModels;

    public static class BetSettingProductIntegration
    {
        private static readonly int[] BetSettingTabProductConfigs = new[]
        {
            ProductConstants.RNGLottery,
            ProductConstants.Bitcoin
        };

        private static readonly int[] DummyProducts = new[]
        {
            ProductConstants.RNGLottery
        };

        private static CustomerService customerService = new CustomerService();
        private static ProductStatusService productStatusService = new ProductStatusServiceImpl();

        public static IEnumerable<Product> GetVisibledProductGroupBetSettingTabs(int custId, int roleId, byte settingStatus)
        {
            return productStatusService.GetVisibleProductGroups(custId, roleId, settingStatus).Where(p => p.Id != ProductGroupConstants.Sportsbook);
        }
    }
}