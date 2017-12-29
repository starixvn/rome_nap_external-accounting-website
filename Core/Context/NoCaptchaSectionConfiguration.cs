namespace Fanex.ExternalAccount.Core.Context
{
    using System.Configuration;

    public class NoCaptchaSectionConfiguration : ConfigurationSection
    {
        private static NoCaptchaSectionConfiguration _instance;

        public static NoCaptchaSectionConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (NoCaptchaSectionConfiguration) ConfigurationManager.GetSection("NoCaptchaSection");
                }

                return _instance;
            }
        }

        [ConfigurationProperty("allowIPs", IsRequired = false)]
        public string AllowIPs => (string) base["allowIPs"];

        [ConfigurationProperty("allowAdmins", IsRequired = false)]
        public string AllowAdmins => (string) base["allowAdmins"];
    }
}