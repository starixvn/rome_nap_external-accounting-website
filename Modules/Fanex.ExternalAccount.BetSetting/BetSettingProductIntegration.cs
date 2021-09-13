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
    using ViewModels;

    public static class BetSettingProductIntegration
    {
        private static readonly int[] BetSettingTabProductConfigs = new[]
        {
            ProductConstants.LiveCasino,
            ProductConstants.VoidBridge,
            ProductConstants.NG2,
            ProductConstants.RNGLottery,
            ProductConstants.Sportsbook2,
            ProductConstants.VirtualGames,
            ProductConstants.Sabaclub,
            ProductConstants.VGaming,
            ProductConstants.Bitcoin
        };

        private static readonly int[] DummyProducts = new[]
        {
            ProductConstants.LiveCasino,
            ProductConstants.RNGLottery
        };

        private static CustomerService customerService = new CustomerService();

        public static IList<Product> GetVisibledBetSettingTabs(int custId, byte settingStatus)
        {
            var customerStatus = customerService.GetCustomerAllStatus(custId, settingStatus);
            var visibleProductTabs = new List<Product>();
            int index = 0;
            foreach (var product in customerStatus.DCSProductsSetting)
            {
                if (Array.Exists(BetSettingTabProductConfigs, element => element == product.ProductID)
                    && !product.IsHidden && (DummyProducts.Any(p => p == product.ProductID) || product.IsSynced))
                {
                    visibleProductTabs.Add(new Product(index++, product.ProductID, product.ProductName));
                }
            }

            return visibleProductTabs;
        }
    }
}
