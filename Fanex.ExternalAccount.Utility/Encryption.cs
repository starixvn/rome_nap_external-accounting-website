using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic;

/*
 * Created 20100330@Lee - Encryption functions
 * Revision:
 * ?@? - ...
 *
 */

namespace Fanex.ExternalAccount.Utilities
{
    public class Encryption
    {
        public Encryption()
        {
        }

        public static string PearsonEncode(string key)
        {
            if (string.IsNullOrEmpty(key) || key.Length == 1) return key;

            var h = 0L;
            var c = key.Length;
            var keys = key.ToCharArray();

            for (var i = 0; i < c; i++)
            {
                h += keys[i].GetHashCode();
            }

            return h.ToString();
        }

        private const string cfsKey = "&％#@?,:*!~^.";

        private static string CfsCode(string nWord)
        {
            string number = "";
            for (int i = 1; i <= nWord.Length; i++)
            {
                number = number + Strings.Asc(Strings.Mid(nWord, i, 1));
            }
            return Conversion.Hex(number);
        }

        /// <summary>
        /// One-way encryption
        /// </summary>
        /// <param name="codeStr"></param>
        /// <returns></returns>
        public static string CfsEnCode(string codeStr)
        {
            int num = 30;
            int num2 = num - codeStr.Length;
            if (num2 > 1)
            {
                for (int m = 1; m <= num2; m++)
                {
                    codeStr = codeStr + Strings.Chr(0x15);
                }
            }
            double num3 = 1.0;
            for (int i = 1; i <= num; i++)
            {
                int num5 = num + (Strings.Asc(Strings.Mid(codeStr, i, 1)) * i);
                num3 *= num5;
            }
            codeStr = num3.ToString();
            string str = "";
            for (int j = 1; j <= codeStr.Length; j++)
            {
                str = str + CfsCode(Strings.Mid(codeStr, j, 3));
            }
            string str2 = "";
            for (int k = 20; k <= (str.ToString().Length - 0x12); k += 2)
            {
                str2 = str2 + Strings.Mid(str, k, 1);
            }
            return str2;
        }

        /// <summary>
        /// One-way encryption
        /// </summary>
        /// <param name="codeStr"></param>
        /// <returns></returns>
        public static string CfsEnCode(string codeStr, int number)
        {
            int num = number < 6 ? 6 : number;
            int num2 = num - codeStr.Length;
            if (num2 > 1)
            {
                for (int m = 1; m <= num2; m++)
                {
                    codeStr = codeStr + Strings.Chr(0x15);
                }
            }
            double num3 = 1.0;
            for (int i = 1; i <= num; i++)
            {
                int num5 = num + (Strings.Asc(Strings.Mid(codeStr, i, 1)) * i);
                num3 *= num5;
            }
            codeStr = num3.ToString();
            string str = "";
            for (int j = 1; j <= codeStr.Length; j++)
            {
                str = str + CfsCode(Strings.Mid(codeStr, j, 3));
            }
            string str2 = "";
            for (int k = 20; k <= (str.ToString().Length - 0x12); k += 2)
            {
                str2 = str2 + Strings.Mid(str, k, 1);
            }
            return str2;
        }

        public static string CfsDeCrypt(string strText)
        {
            try
            {
                byte[] rgbIV = new byte[0];
                byte[] rgbKey = new byte[] { 0x21, 0x63, 0x54, 0x5a, 0x91, 0xac, 110, 0xcc };
                byte[] buffer = new byte[strText.Length];
                rgbIV = Encoding.UTF8.GetBytes(cfsKey);
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                buffer = Convert.FromBase64String(strText);
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                stream2.Write(buffer, 0, buffer.Length);
                stream2.FlushFinalBlock();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public static string CfsEnCrypt(string strText)
        {
            try
            {
                byte[] rgbIV = new byte[0];
                byte[] rgbKey = new byte[] { 0x21, 0x63, 0x54, 0x5a, 0x91, 0xac, 110, 0xcc };
                rgbIV = Encoding.UTF8.GetBytes(cfsKey);
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                byte[] bytes = Encoding.UTF8.GetBytes(strText);
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                stream2.Write(bytes, 0, bytes.Length);
                stream2.FlushFinalBlock();
                return Convert.ToBase64String(stream.ToArray());
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public static string RemoveSpecialChars(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return str.Replace("\n", "\\n").Replace("\"", "\\\"").Replace("'", "\\'");
        }

        /// <summary>
        /// Generate a random string
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            return RandomString(length, false);
        }

        public static string RandomString(int length, bool noZeroO)
        {
            string chars;
            if (noZeroO)
                chars = "qwertyuipasdfghjklzxcvbnm123456789";
            else
                chars = "qwertyuiopasdfghjklzxcvbnm0123456789";
            string rndStr = string.Empty;
            Random rnd = new Random(DateTime.Now.Second + DateTime.Now.Millisecond);

            for (int i = 0; i < length; ++i)
            {
                rndStr += chars[rnd.Next() % chars.Length];
            }
            return rndStr;
        }

        public static string RandomNumberString(int length)
        {
            string chars = "0123456789";
            string rndStr = string.Empty;
            Random rnd = new Random();

            for (int i = 0; i < length; ++i)
            {
                rndStr += chars[rnd.Next() % chars.Length];
            }
            return rndStr;
        }
    }
}