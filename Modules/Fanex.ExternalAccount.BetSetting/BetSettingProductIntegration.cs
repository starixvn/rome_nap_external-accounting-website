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
            ProductConstants.AllBet,
            ProductConstants.VoidBridge,
            ProductConstants.NG2,
            ProductConstants.Lottery,
            ProductConstants.Sportsbook2,
            ProductConstants.VirtualGames,
            ProductConstants.Sabaclub
        };

        private static CustomerService customerService = new CustomerService();

        public static IList<Product> GetVisibledBetSettingTabs(int custId)
        {
            var customerStatus = customerService.GetCustomerAllStatus(custId);
            var visibleProductTabs = new List<Product>();
            int index = 0;
            foreach (var product in customerStatus.DCSProductsSetting)
            {
                if (Array.Exists(BetSettingTabProductConfigs, element => element == product.ProductID)
                    && !product.IsHidden && product.IsSynced)
                {
                    visibleProductTabs.Add(new Product(index++, product.ProductID, product.ProductName));
                }
            }

            return visibleProductTabs;
        }
    }
}
