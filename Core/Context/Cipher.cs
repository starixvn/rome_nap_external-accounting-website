using System;
using System.Web.UI;

namespace Fanex.ExternalAccount.Core.Context
{
    public class Cipher : Page
    {
        public static Cipher Instance
        {
            get
            {
                return new Cipher();
            }
        }

        public int Code
        {
            get
            {
                var code = 0;
                var cipherKey = Session.SessionID + SecretKey;
                foreach (var key in cipherKey)
                {
                    code += (int)key;
                }

                return code;
            }
        }

        private string SecretKey
        {
            get
            {
                return "!@#$%^&*()qwertyuiopASDFGHJKLzxcvbnm";
            }
        }

        public bool IsValidCode(int code)
        {
            return code == Code;
        }

        public string Decrypt(string encrypted, int code)
        {
            if (!IsValidCode(code))
            {
                throw new InvalidOperationException(nameof(code));
            }

            var sessionId = Session.SessionID;
            var cipherText = string.Empty;
            var plainText = string.Empty;
            var cipher = code.ToString();            

            for (int i = 0; i < encrypted.Length; i++)
            {
                if (i % 2 == 0)
                {
                    cipherText += encrypted[i];
                }
                else
                {
                    plainText += encrypted[i];
                }
            }

            cipherText = cipherText
                .Replace(sessionId, string.Empty)
                .Replace(cipher, string.Empty);

            if (cipherText != string.Empty)
            {
                throw new InvalidOperationException(nameof(code));
            }

            plainText = plainText.Replace("_", string.Empty);

            return plainText;
        }
    }
}
