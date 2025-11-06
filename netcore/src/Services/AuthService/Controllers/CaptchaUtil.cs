using Microsoft.AspNetCore.Http;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Fanex.ExternalAccount.Core.Context;
using Microsoft.Extensions.Hosting;

namespace Fanex.ExternalAccount.Controllers.Authentication
{
    public static class CaptchaUtil
    {
#pragma warning disable S109 // Magic numbers should not be used
        public static string GetValiationImage(HttpContext context)
        {
            var webHostEnvironment = context.RequestServices.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;
            if (webHostEnvironment == null)
            {
                return string.Empty;
            }

            var imagePath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", "Login", "capcha.png");
            
            if (!System.IO.File.Exists(imagePath))
            {
                // Create a simple captcha image if the file doesn't exist
                return CreateCaptchaImage();
            }

            var bmp = new Bitmap(100, 30);
            var g = Graphics.FromImage(bmp);
            g.DrawImage(Image.FromFile(imagePath), new Rectangle(0, 0, 100, 30));
            var randomString = GetCaptchaString(4);

            CurrentSession.Instance.Captcha = randomString;
            g.DrawString(randomString, new Font("Courier", 16), new SolidBrush(Color.Black), 17, 2);

            var stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Jpeg);
            var bytes = stream.ToArray();
            return Convert.ToBase64String(bytes);
        }

        private static string CreateCaptchaImage()
        {
            var bmp = new Bitmap(100, 30);
            var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            var randomString = GetCaptchaString(4);

            CurrentSession.Instance.Captcha = randomString;
            g.DrawString(randomString, new Font("Courier", 16), new SolidBrush(Color.Black), 17, 2);

            var stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Jpeg);
            var bytes = stream.ToArray();
            return Convert.ToBase64String(bytes);
        }
#pragma warning restore S109 // Magic numbers should not be used

        public static string GetCaptchaString(int length)
        {
            const char varZero = '0';
            const char varNine = '9';
            const char varA = 'A';
            const char varZ = 'Z';
            var varCount = 0;
            var strCaptchaString = new StringBuilder();
            var random = new Random();

            while (varCount < length)
            {
                var varRandomNumber = random.Next(varZero, varZ);
                if ((varRandomNumber >= varZero && varRandomNumber <= varNine)
                    || (varRandomNumber >= varA
                        && varRandomNumber <= varZ))
                {
                    strCaptchaString.Append((char)varRandomNumber);
                    varCount++;
                }
            }

            return strCaptchaString.ToString();
        }
    }
}
