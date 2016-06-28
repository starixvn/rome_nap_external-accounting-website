// --------------------------------------------------------------------------
// <copyright file="ProductCollectionConfiguration.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2015.9.3">Created</change>
// </history>

namespace Fanex.ExternalAccount.Core.Reports.WinLossDetail
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    [ConfigurationCollection(typeof(ProductElementConfiguration), AddItemName = "product")]
    public class ProductCollectionConfiguration : ConfigurationElementCollection, IEnumerable<ProductElementConfiguration>
    {
        public ProductElementConfiguration this[int index]
        {
            get
            {
                return BaseGet(index) as ProductElementConfiguration;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ProductElementConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var configElement = element as ProductElementConfiguration;
            if (configElement != null)
            {
                return configElement.Id;
            }

            return string.Empty;
        }

        IEnumerator<ProductElementConfiguration> IEnumerable<ProductElementConfiguration>.GetEnumerator()
        {
            return (from i in Enumerable.Range(0, this.Count)
                    select this[i])
                    .GetEnumerator();
        }     
    }
}
