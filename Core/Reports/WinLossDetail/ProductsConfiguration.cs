namespace Fanex.ExternalAccount.Core.Reports.WinLossDetail
{
    using System.Configuration;

    public class ProductsConfiguration : ConfigurationSection
    {
        public static ProductsConfiguration Instance
        {
            get
            {
                return ConfigurationManager.GetSection("report") as ProductsConfiguration;
            }
        }

        [ConfigurationProperty("products", IsRequired = true)]
        public ProductCollectionConfiguration ProductCollection
        {
            get
            {
                return base["products"] as ProductCollectionConfiguration;
            }
        }
    }
}
