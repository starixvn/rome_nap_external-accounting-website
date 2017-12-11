namespace Fanex.ExternalAccount.Controllers.Authentication
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Context;

    public class CaptchaResult
    {
        public string GetValiationImage(ControllerContext context)
        {
            var bmp = new Bitmap(100, 30);
            var g = Graphics.FromImage(bmp);
            g.DrawImage(Image.FromFile(context.HttpContext.Server.MapPath("~/Images/Login/capcha.png")), new Rectangle(0, 0, 100, 30));
            var randomString = this.GetCaptchaString(4);

            CurrentSession.Instance.Captcha = randomString;
            g.DrawString(randomString, new Font("Courier", 16), new SolidBrush(Color.Black), 17, 2);

            var stream = new System.IO.MemoryStream();
            bmp.Save(stream, ImageFormat.Jpeg);
            var bytes = stream.ToArray();
            var img = Convert.ToBase64String(bytes);

            return img;
        }

        public string GetCaptchaString(int length)
        {
            var varZero = '0';
            var varNine = '9';
            var varA = 'A';
            var varZ = 'Z';
            var varCount = 0;
            var varRandomNumber = 0;
            var strCaptchaString = string.Empty;
            var random = new Random(System.DateTime.Now.Millisecond);

            while (varCount < length)
            {
                varRandomNumber = random.Next(varZero, varZ);
                if ((varRandomNumber >= varZero && varRandomNumber <= varNine) || (varRandomNumber >= varA && varRandomNumber <= varZ))
                {
                    strCaptchaString = strCaptchaString + (char)varRandomNumber;
                    varCount = varCount + 1;
                }
            }

            return strCaptchaString;
        }        
    }
}