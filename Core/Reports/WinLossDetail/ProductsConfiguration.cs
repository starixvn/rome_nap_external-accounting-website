// --------------------------------------------------------------------------
// <copyright file="ProductsConfiguration.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2015.9.3">Created</change>
// </history>

namespace Fanex.ExternalAccount.Core.Reports.WinLossDetail
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
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
