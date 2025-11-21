namespace Fanex.ExternalAccount.Core.Context
{
    using Microsoft.Extensions.Configuration;

    public class NoCaptchaSectionConfiguration
    {
        private readonly IConfiguration _configuration;

        public NoCaptchaSectionConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string AllowIPs => _configuration["NoCaptchaSection:allowIPs"];

        public string AllowAdmins => _configuration["NoCaptchaSection:allowAdmins"];
    }
}