// -----------------------------------------------------------------------
// <copyright file="NoCaptchaSectionConfiguration.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Core.Context
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NoCaptchaSectionConfiguration : ConfigurationSection
    {
        private static NoCaptchaSectionConfiguration _instance;
        
        public static NoCaptchaSectionConfiguration Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = (NoCaptchaSectionConfiguration)ConfigurationManager.GetSection("NoCaptchaSection");
                }

                return _instance;
            }
        }

        [ConfigurationProperty("allowIPs", IsRequired = false)]
        public string AllowIPs
        {
            get
            {
                return (string)base["allowIPs"];
            }
        }

        [ConfigurationProperty("allowAdmins", IsRequired = false)]
        public string AllowAdmins
        {
            get
            {
                return (string)base["allowAdmins"];
            }
        }
    }
}
