namespace Fanex.ExternalAccount.Controllers.Authentication
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Text;
    using System.Web.Mvc;
    using Core.Context;

    public static class CaptchaUtil
    {
#pragma warning disable S109 // Magic numbers should not be used
        public static string GetValiationImage(ControllerContext context)
        {
            var bmp = new Bitmap(100, 30);
            var g = Graphics.FromImage(bmp);
            g.DrawImage(Image.FromFile(context.HttpContext.Server.MapPath("~/Images/Login/capcha.png")),
                new Rectangle(0, 0, 100, 30));
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
                    strCaptchaString.Append((char) varRandomNumber);
                    varCount++;
                }
            }

            return strCaptchaString.ToString();
        }
    }
}