namespace Fanex.ExternalAccount.Core.Reports.WinLossDetail
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;

    public class ProductElementConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true)]
        public string Id => base["id"] as string;

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name => base["name"] as string;

        [ConfigurationProperty("hide", IsRequired = false)]
        public bool Hide => (bool)base["hide"];

        [ConfigurationProperty("productid", IsRequired = false, DefaultValue = 0)]
        public int ProductId => (int)base["productid"];

        [ConfigurationProperty("isspecial", IsRequired = false, DefaultValue = false)]
        public bool IsSpecial => (bool)base["isspecial"];

        [ConfigurationProperty("showForGroups", IsRequired = false, DefaultValue = "*")]
        public string ShowForGroups => (string)base["showForGroups"];

        public bool ShowForAll() => ShowForGroups.Equals("*", StringComparison.OrdinalIgnoreCase);

        public string[] ShowGroupList => ShowForGroups.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    }
}