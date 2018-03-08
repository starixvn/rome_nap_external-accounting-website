namespace Fanex.CustomerSetting.IP.Utils
{
    using System;
    using System.Configuration;
    using System.Data.EntityClient;
    using Fanex.Caching;
    using Fanex.Common;

    public static class Utility
    {
        public static string GetEFConnectionString()
        {
            ICache cache = CacheService.CreateInstance(CacheType.MemoryCache);
            string str = cache.Get<string>("IPEntities_ConnectionString");
            if (string.IsNullOrEmpty(str))
            {
                var cnn = ConfigurationManager.ConnectionStrings["IPEntities"].ConnectionString;
                var pwds = cnn.Split(';');
                var pwd = string.Empty;
                var pwdPhrase = string.Empty;

                for (int i = 0; i < pwds.Length; i++)
                {
                    if (pwds[i].Contains("password"))
                    {
                        pwdPhrase = pwds[i];
                        pwd = pwds[i].Replace("password=", string.Empty);
                        break;
                    }
                }

                try
                {
                    cnn = cnn.Replace(pwdPhrase, "password=" + Cryptography.CfsDeCrypt(pwd));
                }
                catch
                {
                    cnn = cnn.Replace(pwdPhrase, "password=" + Cryptography.CfsDeCrypt(pwd.Replace("=", string.Empty)));
                }

                str = new EntityConnectionStringBuilder(cnn).ToString();
                cache.Set<string>("IPEntities_ConnectionString", str, TimeSpan.FromHours(1.0));
            }

            return str;
        }
    }
}