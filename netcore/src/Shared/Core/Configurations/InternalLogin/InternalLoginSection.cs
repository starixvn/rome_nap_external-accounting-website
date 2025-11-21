namespace Fanex.ExternalAccount.Core.Configurations.InternalLogin
{
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;
    using System.Linq;

    public class InternalUrlSection
    {
        private readonly IConfiguration _configuration;

        public InternalUrlSection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<InternalUrlElement> InternalUrlCollection =>
            _configuration.GetSection("InternalUrlSection:InternalUrlCollection").GetChildren()
                .Select(section => new InternalUrlElement(section));
    }

    public class InternalUrlElement
    {
        private readonly IConfiguration _configuration;

        public InternalUrlElement(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Name => _configuration["name"];
        public bool IsDefault => bool.Parse(_configuration["isdefault"] ?? "false");
        public string Sites => _configuration["sites"];
        public string Url => _configuration["url"];
    }
}
