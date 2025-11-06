using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Fanex.ExternalAccount.Core.Context;
using Microsoft.Extensions.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats;

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

            // Load background image using SixLabors.ImageSharp
            using var bmp = SixLabors.ImageSharp.Image.Load<Rgba32>(imagePath);
            bmp.Mutate(x => x.Resize(100, 30));

            var randomString = GetCaptchaString(4);
            CurrentSession.Instance.Captcha = randomString;

            // Get font (fallback to Arial if Courier not available)
            var fontFamily = SystemFonts.Get("Courier New");
            if (fontFamily == null) fontFamily = SystemFonts.Get("Courier");
            if (fontFamily == null) fontFamily = SystemFonts.Get("Arial");
            if (fontFamily == null) fontFamily = SystemFonts.Families.FirstOrDefault();
            var font = fontFamily.CreateFont(16, FontStyle.Regular);

            // Draw text on image
            var textOptions = new RichTextOptions(font)
            {
                Origin = new PointF(17, 2),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            bmp.Mutate(x => x.DrawText(textOptions, randomString, SixLabors.ImageSharp.Color.Black));

            // Save to memory stream as JPEG
            var stream = new MemoryStream();
            bmp.SaveAsJpeg(stream);
            var bytes = stream.ToArray();
            return Convert.ToBase64String(bytes);
        }

        private static string CreateCaptchaImage()
        {
            // Create a new image with white background
            using var bmp = new SixLabors.ImageSharp.Image<Rgba32>(100, 30, SixLabors.ImageSharp.Color.White);

            var randomString = GetCaptchaString(4);
            CurrentSession.Instance.Captcha = randomString;

            // Get font (fallback to Arial if Courier not available)
            var fontFamily = SystemFonts.Get("Courier New");
            if (fontFamily == null) fontFamily = SystemFonts.Get("Courier");
            if (fontFamily == null) fontFamily = SystemFonts.Get("Arial");
            if (fontFamily == null) fontFamily = SystemFonts.Families.FirstOrDefault();
            var font = fontFamily.CreateFont(16, FontStyle.Regular);

            // Draw text on image
            var textOptions = new RichTextOptions(font)
            {
                Origin = new PointF(17, 2),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            bmp.Mutate(x => x.DrawText(textOptions, randomString, SixLabors.ImageSharp.Color.Black));

            // Save to memory stream as JPEG
            var stream = new MemoryStream();
            bmp.SaveAsJpeg(stream);
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
