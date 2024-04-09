using System.Configuration;

namespace Fanex.ExternalAccount.Core.Configurations
{
    public static class RedisCachingRegister
    {
        public static void Register()
        {
            Fanex.Caching.CacheServiceManager.Use(() => new Fanex.Caching.Redis.RedisCache(new Fanex.Caching.Redis.RedisCacheOptions
            {
                ConnectionOptions = StackExchange.Redis.ConfigurationOptions.Parse(ConfigurationManager.AppSettings["Redis:ConnectionString"])
            }));
        }
    }
}
