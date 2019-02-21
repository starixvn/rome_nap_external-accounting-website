namespace Fanex.ExternalAccount.Utilities.Reports.WinLossDetail
{
    using System.Configuration;

    public class ProductElementConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true)]
        public string Id => base["id"] as string;

        [ConfigurationProperty("name", IsRequired = false)]
        public string Name => base["name"] as string;

        [ConfigurationProperty("productid", IsRequired = false, DefaultValue = 0)]
        public int ProductId => (int) base["productid"];

        [ConfigurationProperty("isspecial", IsRequired = false, DefaultValue = false)]
        public bool IsSpecial => (bool) base["isspecial"];

        [ConfigurationProperty("isHideInactiveProduct", IsRequired = false, DefaultValue = false)]
        public bool IsHideInactiveProduct => (bool)base["isHideInactiveProduct"];       
    }
}