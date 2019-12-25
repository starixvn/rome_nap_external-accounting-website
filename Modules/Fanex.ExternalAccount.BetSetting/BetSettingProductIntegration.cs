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
    using ViewModels;

    public static class BetSettingProductIntegration
    {
        private static readonly int[] BetSettingTabProductConfigs = new[]
        {
            ProductConstants.AllBet,
            ProductConstants.VoidBridge,
            ProductConstants.NG2,
            ProductConstants.Lottery,
            ProductConstants.Sportsbook2
        };

        private static CustomerListService customerListService = new CustomerListService();

        public static IList<Product> GetVisibledBetSettingTabs(int custId)
        {
            var customerStatus = customerListService.GetCustomerAllStatus(custId);
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
