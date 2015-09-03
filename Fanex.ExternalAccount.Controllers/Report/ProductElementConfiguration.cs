// --------------------------------------------------------------------------
// <copyright file="ProductElementConfiguration.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2015.9.3">Created</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Report
{
    using System.Configuration;

    public class ProductElementConfiguration : ConfigurationElement 
    {
        [ConfigurationProperty("id", IsRequired = true)]
        public string Id
        {
            get
            {
                return base["id"] as string;
            }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return base["name"] as string;
            }
        }

        [ConfigurationProperty("hide", IsRequired = false)]
        public bool Hide
        {
            get
            {
                return (bool)base["hide"];
            }
        }
    }
}
