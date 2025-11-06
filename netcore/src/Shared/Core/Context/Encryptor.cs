namespace Fanex.ExternalAccount.Core.Context
{
    using Cryptography.Aes;
    using Logging.Extensions.Web;
    using System;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    public static class Encryptor
    {
        private const string RandomCaptchaKey = "RandomCaptcha";
        private const string DefaultCaptcha = "0000";

        private static IHttpContextAccessor? _httpContextAccessor;

        public static void SetHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private static ISession? Session
        {
            get
            {
                if (_httpContextAccessor?.HttpContext?.Session != null)
                {
                    return _httpContextAccessor.HttpContext.Session;
                }
                
                // Fallback
                try
                {
                    var serviceProvider = new ServiceCollection().BuildServiceProvider();
                    var accessor = serviceProvider.GetService<IHttpContextAccessor>();
                    return accessor?.HttpContext?.Session;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static string RandomCaptcha
        {
            get 
            { 
                var session = Session;
                if (session == null)
                    return DefaultCaptcha;

                return session.GetString(RandomCaptchaKey) ?? DefaultCaptcha;
            }

            set 
            { 
                var session = Session;
                if (session != null)
                {
                    session.SetString(RandomCaptchaKey, value);
                }
            }
        }
                      
        public static string Decrypt(string encrypted, string key)
        {
            try
            {
                return AesExtension.Decrypt(encrypted, key);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, TraceEventType.Warning);
            }

            return string.Empty;
        }
    }
}
