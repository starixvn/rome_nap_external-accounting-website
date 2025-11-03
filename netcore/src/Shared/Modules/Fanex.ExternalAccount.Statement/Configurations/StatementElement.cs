namespace Fanex.ExternalAccount.Statement.Configurations
{
    using System;
    using System.Configuration;
    using Fanex.Common;

    public class StatementElement : ConfigurationElement
    {
        [ConfigurationProperty("newlayoutdate", IsRequired = true)]
        public DateTime NewLayoutDate
        {
            get
            {
                return Converter.ToDateTime(this["newlayoutdate"]);
            }

            set
            {
                this["newlayoutdate"] = value;
            }
        }

        [ConfigurationProperty("showCreatorOnDetail", IsRequired = false, DefaultValue = false)]
        public bool ShowCreatorOnDetail
        {
            get
            {
                return Converter.ToBool(this["showCreatorOnDetail"]);
            }

            set
            {
                this["showCreatorOnDetail"] = value;
            }
        }

        [ConfigurationProperty("showExchangeRateOnDetail", IsRequired = false, DefaultValue = false)]
        public bool ShowExchangeRateOnDetail
        {
            get
            {
                return Converter.ToBool(this["showExchangeRateOnDetail"]);
            }

            set
            {
                this["showExchangeRateOnDetail"] = value;
            }
        }
    }
}
