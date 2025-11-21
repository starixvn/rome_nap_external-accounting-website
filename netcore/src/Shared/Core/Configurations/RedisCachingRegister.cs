using Microsoft.Extensions.Configuration;

namespace Fanex.ExternalAccount.Core.Configurations
{
    public static class RedisCachingRegister
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static void Register()
        {
            Fanex.Caching.CacheServiceManager.Use(() => new Fanex.Caching.Redis.RedisCache(new Fanex.Caching.Redis.RedisCacheOptions
            {
                ConnectionOptions = StackExchange.Redis.ConfigurationOptions.Parse(_configuration["Redis:ConnectionString"])
            }));
        }
    }
}
