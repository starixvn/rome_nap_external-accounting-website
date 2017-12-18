namespace Fanex.ExternalAccount.Core.Context
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using Module.Logs;
    using Module.Logs.Services;

    public static class Encryptor
    {
        private const string HexDigit = "0123456789abcdef";
        private const string RandomCaptchaKey = "RandomCaptcha";
        private const string DefaultCaptcha = "0000";


        public static string RandomCaptcha
        {
            get { return HttpContext.Current.Session[RandomCaptchaKey] as string ?? DefaultCaptcha; }

            set { HttpContext.Current.Session[RandomCaptchaKey] = value; }
        }

        public static string HexDecode(string data)
        {
            const string b16Digits = HexDigit;
            var b16Map = new char[256];
            for (var i = 0; i < 256; i++)
            {
                b16Map[Convert.ToInt32(string.Concat(b16Digits[i >> 4], b16Digits[i & 15]), 16)] = Convert.ToChar(i);
            }

            var c = data.Length;
            if (c % 2 != 0)
            {
                data = '0' + data;
            }

            var result = new List<char>();
            for (var i = 0; i < c; i += 2)
            {
                result.Add(b16Map[Convert.ToInt32(data.Substring(i, 2), 16)]);
            }

            return new string(result.ToArray());
        }

        public static string RC4(string key, string value)
        {
            var ks = key.ToCharArray();
            var vs = value.ToCharArray();
            var c = key.Length;
            var d = value.Length;
            var s = new int[256];
            var i = 0;
            for (i = 0; i < 256; i++)
            {
                s[i] = i;
            }

            var j = 0;
            int x;
            for (i = 0; i < 256; i++)
            {
                j = (j + s[i] + ks[i % c]) % 256;
                x = s[i];
                s[i] = s[j];
                s[j] = x;
            }

            i = 0;
            j = 0;
            var ct = new StringBuilder();
            for (var y = 0; y < d; y++)
            {
                i = (i + 1) % 256;
                j = (j + s[i]) % 256;
                x = s[i];
                s[i] = s[j];
                s[j] = x;
                ct.Append(Convert.ToChar(vs[y] ^ s[(s[i] + s[j]) % 256]));
            }

            return ct.ToString();
        }

        public static string Decrypt(string encrypted, string key)
        {
            try
            {
                return RC4(key, HexDecode(encrypted));
            }
            catch (Exception ex)
            {
                ex.Write(Formater.LogType.Warning);
            }

            return string.Empty;
        }
    }
}