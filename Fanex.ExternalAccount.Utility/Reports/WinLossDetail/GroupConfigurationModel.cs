namespace Fanex.ExternalAccount.Utilities.Reports.WinLossDetail
{
    using System.Linq;

    public class GroupConfigurationModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string Currency { get; set; }

        public string FormatedCurrency => string.IsNullOrEmpty(Currency) ? string.Empty : Currency.Replace(",", ", ");

        public string CurrencyCode { get; set; }

        public string Site { get; set; }

        public string FormatedSite => string.IsNullOrEmpty(Site) ? string.Empty : Site.Replace(",", ", ");

        public bool IsShow { get; set; }

        public bool IsHide => !IsShow;

        public string Id
        {
            get
            {
                var product = ProductsConfiguration.Instance.ProductCollection.FirstOrDefault(i => i.ProductId == ProductId);

                return product != null ? product.Id : string.Empty;
            }
        }

        public bool IsHideInactiveProduct
        {
            get
            {
                var product = ProductsConfiguration.Instance.ProductCollection.FirstOrDefault(i => i.ProductId == ProductId);

                return product != null && product.IsHideInactiveProduct;
            }
        }

        public bool IsSpecial
        {
            get
            {
                var product = ProductsConfiguration.Instance.ProductCollection.FirstOrDefault(i => i.ProductId == ProductId);

                return product != null && product.IsSpecial;
            }
        }
    }
}
