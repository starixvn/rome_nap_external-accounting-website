namespace Fanex.ExternalAccount.Core.Context
{
    using Cryptography.Aes;
    using Logging.Extensions.Web;
    using System;
    using System.Diagnostics;
    using System.Web;

    public static class Encryptor
    {
        private const string RandomCaptchaKey = "RandomCaptcha";
        private const string DefaultCaptcha = "0000";


        public static string RandomCaptcha
        {
            get { return HttpContext.Current.Session[RandomCaptchaKey] as string ?? DefaultCaptcha; }

            set { HttpContext.Current.Session[RandomCaptchaKey] = value; }
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