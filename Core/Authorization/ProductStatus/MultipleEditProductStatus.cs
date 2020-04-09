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
            ProductConstants.Sportsbook,
            ProductConstants.SexyGaming,
            ProductConstants.SAGaming,
            ProductConstants.TableGames
        };

        protected MultipleEditProductStatus()
        {
        }

        public static IEnumerable<KeyValuePair<int, string>> GetCustomerProductList()
            => new ProductListAuthorization()
                .GetProductList()
                .Where(product => !UnsupportProductStatus.Any(i => i == product.ProductId))
                .Select(product => MapToDummyProducts(product))
                .Select(product => new KeyValuePair<int, string>(
                    product.ProductId,
                    product.ProductText))
                .ToList();

        private static GroupConfigurationModel MapToDummyProducts(GroupConfigurationModel product)
        {
            var productId = product.ProductId;

            if (productId == ProductConstants.AllBet)
            {
                productId = ProductConstants.DummyProductIdForAllbet_SAGaming_SexyGaming;
            }
            else if (productId == ProductConstants.Lottery)
            {
                productId = ProductConstants.RNGLottery;
            }

            return new GroupConfigurationModel
            {
                ProductId = productId,
                Currency = product.Currency,
                CurrencyCode = product.CurrencyCode,
                GroupId = product.GroupId,
                GroupName = product.GroupName,
                IsShow = product.IsShow,
                ProductName = product.ProductName,
                Site = product.Site
            };
        }
    }
}
