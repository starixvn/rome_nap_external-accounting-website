namespace Fanex.Tarus.Infrastructure.Configurations.InternalLogin
{
    using ExternalAccount.Core.Configurations.InternalLogin;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;

    public static class InternalLoginManager
    {
        public static string GetInternalLoginUrl(string site)
        {
            var internalUrlSection = ConfigurationManager.GetSection("InternalUrlSection") as InternalUrlSection;
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            foreach (InternalUrlElement element in internalUrlSection.InternalUrlCollection)
            {
                if (culture.CompareInfo.IndexOf(element.Sites, site, CompareOptions.IgnoreCase) >= 0)
                {
                    return element.Url;
                }
            }

            return (from InternalUrlElement e in internalUrlSection.InternalUrlCollection
                    where e.IsDefault
                    select e).FirstOrDefault()?.Url;
        }
    }
}
