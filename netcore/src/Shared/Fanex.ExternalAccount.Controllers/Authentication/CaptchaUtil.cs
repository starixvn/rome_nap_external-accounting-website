namespace Fanex.ExternalAccount.Controllers.Authentication
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.AspNetCore.Mvc;
    using Core.Context;

    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using SixLabors.Fonts;
    using SixLabors.ImageSharp.Drawing.Processing;

    public static class CaptchaUtil
    {
#pragma warning disable S109 // Magic numbers should not be used
        public static string GetValiationImage(ControllerContext context)
        {
            const int width = 100;
            const int height = 30;
            var randomString = GetCaptchaString(4);

            CurrentSession.Instance.Captcha = randomString;

            // Load background image
            string bgPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Login", "capcha.png");
            using var image = File.Exists(bgPath)
                ? Image.Load<Rgba32>(bgPath)
                : new Image<Rgba32>(width, height);

            using var ms = new MemoryStream();
            image.SaveAsJpeg(ms);
            return DrawImage(ms.ToArray(), randomString);
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

        public static string DrawImage(byte[] imageBytes, string waterMarkText)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return string.Empty;
            }
            
            using var image = Image.Load<Rgba32>(imageBytes);
            var font = SystemFonts.CreateFont("Arial", 24, FontStyle.Bold);

            image.Mutate(x => ApplyScalingWaterMark(x, waterMarkText, font));

            using var ms = new MemoryStream();
            image.SaveAsJpeg(ms);

            return $"data:image/jpeg;base64,{Convert.ToBase64String(ms.ToArray())}";
        }

        private static IImageProcessingContext ApplyScalingWaterMark(IImageProcessingContext processingContext,
            string text,
            Font font)
        {

            var x = 50;
            var y = 50;

            var textPosition = new PointF(x, y);
            var textSize = TextMeasurer.MeasureSize(text, new TextOptions(font));

            var backgroundRectangle = new Rectangle(
                (int)textPosition.X - 5,
                (int)textPosition.Y,
                (int)textSize.Width + 12,
                (int)textSize.Height + 10);

            return processingContext.Fill(Color.LightBlue, backgroundRectangle)
                .DrawText(text, font, Color.Black, textPosition);
        }
    }
}