// --------------------------------------------------------------------------
// <copyright file="CaptchaResult.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// Controller for Captcha
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.13">Created</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Context;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CaptchaResult : ActionResult
    {
        /// <summary>
        /// Gets captcha string.
        /// </summary>
        /// <param name="length">Length of captcha.</param>
        /// <returns>String object.</returns>
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

        /// <summary>
        /// Execute result.
        /// </summary>
        /// <param name="context">ControllerContext object.</param>
        public override void ExecuteResult(ControllerContext context)
        {            
        }

        public string GetValiationImage(ControllerContext context)
        {
            var bmp = new Bitmap(100, 30);
            var g = Graphics.FromImage(bmp);
            g.DrawImage(Image.FromFile(context.HttpContext.Server.MapPath("~/Images/Login/capcha.png")), new Rectangle(0, 0, 100, 30));
            var randomString = this.GetCaptchaString(4);

            CurrentSession.Instance.Captcha = randomString;
            g.DrawString(randomString, new Font("Courier", 16), new SolidBrush(Color.Black), 17, 2);

            var response = context.HttpContext.Response;
            //response.ContentType = "image/jpeg";
            //bmp.Save(response.OutputStream, ImageFormat.Jpeg);
            var stream = new System.IO.MemoryStream();
            bmp.Save(stream, ImageFormat.Jpeg);
            var bytes = stream.ToArray();
            var img = Convert.ToBase64String(bytes);

            return img;
        }
    }
}