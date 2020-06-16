namespace Fanex.ExternalAccount.Utilities.Reports.WinLossDetailIBCBetLiveCasino
{
    using System.Configuration;

    public class ProductsConfiguration : ConfigurationSection
    {
        public static ProductsConfiguration Instance =>
            ConfigurationManager.GetSection("report") as ProductsConfiguration;

        [ConfigurationProperty("products", IsRequired = true)]
        public ProductCollectionConfiguration ProductCollection => base["products"] as ProductCollectionConfiguration;
    }
}
