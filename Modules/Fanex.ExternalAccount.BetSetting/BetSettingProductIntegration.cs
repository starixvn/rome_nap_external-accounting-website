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
            var productStatusList = productStatusService.GetProductStatusList(custId, roleId, settingStatus);
            var visibledProductGroups = productStatusList
                .Where(p => p.GroupId != null && p.GroupId != ProductGroupConstants.Sportsbook && !p.IsHidden)
                .Select(p => p.GroupId.Value)
                .Distinct()
                .Select(groupId => new Product(groupId, groupId, CacheRef.GetProductGroupName(groupId)));

            return visibledProductGroups;
        }

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