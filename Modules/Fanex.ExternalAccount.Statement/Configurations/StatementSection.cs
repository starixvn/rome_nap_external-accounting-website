namespace Fanex.ExternalAccount.Statement.Configurations
{
    using Fanex.Common;
    using System;
    using System.Configuration;

    public class StatementSection : ConfigurationSection
    {
        private static StatementSection _instance;

        public static StatementSection Instance
        {
            get
            {
                _instance = _instance ?? ConfigurationManager.GetSection("StatementSection") as StatementSection;
                return _instance;
            }
        }

        [ConfigurationProperty("newlayoutdate", IsRequired = true)]
        public DateTime NewLayoutDate
        {
            get
            {
                return Converter.ToDateTime(this["newlayoutdate"]);
            }
        }

        [ConfigurationProperty("showCreatorOnDetail", IsRequired = false, DefaultValue = false)]
        public bool ShowCreatorOnDetail
        {
            get
            {
                return Converter.ToBool(this["showCreatorOnDetail"]);
            }
        }

        [ConfigurationProperty("showExchangeRateOnDetail", IsRequired = false, DefaultValue = false)]
        public bool ShowExchangeRateOnDetail
        {
            get
            {
                return Converter.ToBool(this["showExchangeRateOnDetail"]);
            }
        }
    }
}
