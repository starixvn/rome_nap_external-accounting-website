namespace Fanex.ExternalAccount.Utilities.Reports.WinLossDetailIBCBetLiveCasino
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    [ConfigurationCollection(typeof(ProductElementConfiguration), AddItemName = "product")]
    public class ProductCollectionConfiguration : ConfigurationElementCollection,
        IEnumerable<ProductElementConfiguration>
    {
        public ProductElementConfiguration this[int index] => BaseGet(index) as ProductElementConfiguration;

        IEnumerator<ProductElementConfiguration> IEnumerable<ProductElementConfiguration>.GetEnumerator() =>
            (from i in Enumerable.Range(0, Count)
                select this[i])
            .GetEnumerator();

        protected override ConfigurationElement CreateNewElement() => new ProductElementConfiguration();

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element is ProductElementConfiguration configElement)
            {
                return configElement.Id;
            }

            return string.Empty;
        }
    }
}
