namespace Fanex.ExternalAccount.Statement.Configurations
{
    using Fanex.Common;
    using System;
    using Microsoft.Extensions.Configuration;

    public class StatementSection
    {
        private readonly IConfiguration _configuration;

        public StatementSection(IConfiguration? configuration)
        {
            _configuration = configuration ?? new ConfigurationBuilder().Build();
        }

        public DateTime NewLayoutDate => Converter.ToDateTime(_configuration?["StatementSection:newlayoutdate"]);

        public bool ShowCreatorOnDetail => Converter.ToBool(_configuration?["StatementSection:showCreatorOnDetail"]);

        public bool ShowExchangeRateOnDetail => Converter.ToBool(_configuration?["StatementSection:showExchangeRateOnDetail"]);
    }
}
