// --------------------------------------------------------------------------
// <copyright file="ProductsConfiguration.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2015.9.3">Created</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Report
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ProductsConfiguration : ConfigurationSection
    {
        private static ProductsConfiguration _instance = null;

        public static ProductsConfiguration Instance
        {
            get
            {
                _instance = _instance ?? (ProductsConfiguration)ConfigurationManager.GetSection("report") as ProductsConfiguration;
                return _instance;
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
