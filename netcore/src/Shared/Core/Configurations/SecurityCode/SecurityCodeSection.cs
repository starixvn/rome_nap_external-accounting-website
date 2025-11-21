namespace Fanex.ExternalAccount.Core.Configurations.SecurityCode
{
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;
    using System.Linq;

    public class SecurityCodeSection
    {
        private readonly IConfiguration _configuration;

        public SecurityCodeSection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<ActionFilterElement> ActionFilterCollection =>
            _configuration.GetSection("SecurityCodeSection:ActionFilterCollection").GetChildren()
                .Select(section => new ActionFilterElement(section));

        public IEnumerable<UserCodeElement> UserCodeCollection =>
            _configuration.GetSection("SecurityCodeSection:UserCodeCollection").GetChildren()
                .Select(section => new UserCodeElement(section));

        public string ScAliveTime => _configuration["SecurityCodeSection:scAliveTime"];

        public int ScMaxAllowAttempt => int.Parse(_configuration["SecurityCodeSection:scMaxAllowAttempt"] ?? "0");
    }


    public class ActionFilterElement
    {
        private readonly IConfiguration _configuration;

        public ActionFilterElement(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Name => _configuration["name"];
        public string Action => _configuration["action"];
        public bool IsIgnored => bool.Parse(_configuration["isIgnored"] ?? "false");
    }

    public class UserCodeElement
    {
        private readonly IConfiguration _configuration;

        public UserCodeElement(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string UserName => _configuration["user"];
        public string SecurityCode => _configuration["code"];
    }
}