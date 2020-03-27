namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.ExternalAccount.Utilities.Reports.WinLossDetail;
    using Shared;

    public class MultipleEditProductStatus
    {
        private static List<int> UnsupportProductStatus = new List<int>
        {
            ProductConstants.Sportsbook
        };

        protected MultipleEditProductStatus()
        {
        }

        public static IEnumerable<KeyValuePair<int, string>> GetCustomerProductList()
            => new ProductListAuthorization()
                .GetProductList()
                .Where(product => !UnsupportProductStatus.Any(i => i == product.ProductId) && product.ProductId != ProductConstants.SexyGaming && product.ProductId != ProductConstants.SAGaming)
                .Select(product => product.ProductId == ProductConstants.AllBet ? new GroupConfigurationModel
                {
                    ProductId = ProductConstants.DummyProductIdForAllbet_SAGaming_SexyGaming,
                    Currency = product.Currency,
                    CurrencyCode = product.CurrencyCode,
                    GroupId = product.GroupId,
                    GroupName = product.GroupName,
                    IsShow = product.IsShow,
                    ProductName = product.ProductName,
                    Site = product.Site
                } : product)
                .Select(product => new KeyValuePair<int, string>(
                    product.ProductId,
                    product.ProductText))
                .ToList();
    }
}
