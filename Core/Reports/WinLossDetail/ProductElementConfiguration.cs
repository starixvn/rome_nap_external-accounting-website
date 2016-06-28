// --------------------------------------------------------------------------
// <copyright file="ProductElementConfiguration.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2015.9.3">Created</change>
// </history>

namespace Fanex.ExternalAccount.Core.Reports.WinLossDetail
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

        [ConfigurationProperty("showForGroups", IsRequired = false, DefaultValue = "*")]
        public string ShowForGroups
        {
            get
            {
                return (string)base["showForGroups"];
            }
        }
    }
}
