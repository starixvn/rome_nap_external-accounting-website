namespace Fanex.Tarus.Infrastructure.Configurations.InternalLogin
{
    using ExternalAccount.Core.Configurations.InternalLogin;
    using Microsoft.Extensions.Configuration;
    using System.Linq;

    public class InternalLoginManager
    {
        private readonly InternalUrlSection _internalUrlSection;

        public InternalLoginManager(IConfiguration configuration)
        {
            _internalUrlSection = new InternalUrlSection(configuration);
        }

        public string GetInternalLoginUrl(string site)
        {
            site = site.Trim();
            foreach (InternalUrlElement element in _internalUrlSection.InternalUrlCollection)
            {
                string[] configSites = element.Sites.Split(',');
                foreach (var configSite in configSites)
                {
                    if (configSite.Trim().Contains(site))
                    {
                        return element.Url;
                    }
                }
            }

            return (from InternalUrlElement e in _internalUrlSection.InternalUrlCollection
                    where e.IsDefault
                    select e).FirstOrDefault()?.Url;
        }
    }
}
