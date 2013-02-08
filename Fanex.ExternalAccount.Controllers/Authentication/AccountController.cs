// --------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// Controller for account
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.13">Created</change>
// </history>
namespace Fanex.ExternalAccount.Controllers.Authentication
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Login;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.Login;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Account;

    /// <summary>
    /// Account controller
    /// </summary>
    public class AccountController : Controller
    {
        private ILoginService userService = null;

        private ICurrentSession currentSession = null;

        private ICurrentContext currentContext = null;

        public AccountController()
            : this(null, null, null)
        {
        }

        public AccountController(ILoginService userService, ICurrentSession currentSession, ICurrentContext currentContext)
        {
            this.userService = userService ?? new LoginService();
            this.currentSession = currentSession ?? new CurrentSession();
            this.currentContext = currentContext ?? new CurrentContext();
        }

        // GET: /Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            Response.AppendHeader("Content-Encoding", "gzip");
            Response.Filter = new System.IO.Compression.GZipStream(Response.Filter, System.IO.Compression.CompressionMode.Compress);

            var maintenanceInfo = Config.GetMaintenanceInfo();

            if (maintenanceInfo.IsDownTime)
            {
                return this.RedirectToAction("Offline", "Account");
            }

            if (!this.currentSession.IsLogin && !this.currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return this.RedirectToAction("Login", "Account");
            }

            if (this.currentSession.IsLogin && this.currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                var group = this.currentSession.IsInternalAdmin ? "ACCDDD" : this.currentSession.CurrencyGroup;

                return this.Redirect("~/Home/Index/" + group);
            }

            return this.PartialView(new Login());
        }

        // POST: /Account/Login
        [AllowAnonymous]
        [HttpPost]
        [ActionName("Login")]

        public ActionResult LoginPost(Login model)
        {
            if (this.currentSession.Captcha != null && this.currentSession.Captcha.Equals(model.Captcha, StringComparison.OrdinalIgnoreCase))
            {
                CurrentContext.LoginErrorMessage = string.Empty;

                var custId = 0;
                var onUserId = 0;
                var isIntAcc = true;

                var result = this.userService.Login(model.UserName, model.Password, new DTO.Request() { Browser = Request.Browser.Browser, CountryCode = this.currentContext.CountryCode, DomainName = this.currentContext.DomainName, IP = this.currentContext.Ip4Address, NetSessionId = this.currentSession.NetSessionId }, out custId, out onUserId, out isIntAcc);

                if (result != null && result.IsSuccess)
                {
                    this.currentSession.UserName = model.UserName;
                    this.currentSession.UserId = custId;
                    this.currentSession.CustId = custId;
                    this.currentSession.OnUserId = onUserId;
                    this.currentSession.IsInternalAdmin = isIntAcc;

                    this.currentSession.CurrencyGroup = (result.ResultData as Fanex.ExternalAccount.DTO.UserControl.Group).GroupName;
                    this.currentSession.IsITAdmin = (result.ResultData as Fanex.ExternalAccount.DTO.UserControl.Group).GroupID == 1;

                    if (isIntAcc || this.currentSession.IsITAdmin)
                    {
                        return this.RedirectToAction("ExternalGroup", "Account");
                    }

                    return this.RedirectToAction("Index", "Home");
                }
                else
                {
                    CurrentContext.LoginErrorMessage = result.ErrorMessage;

                    return this.PartialView(model);
                }
            }
            else
            {
                CurrentContext.LoginErrorMessage = ViewLang.WrongValidationCode;
                this.currentSession.UserName = string.Empty;

                return this.PartialView(model);
            }
        }

        public ActionResult CaptchaImage(string prefix, bool noisy = true)
        {
            /*
            var rand = new Random((int)DateTime.Now.Ticks);

            //generate new question
            int a = rand.Next(10, 99);
            int b = rand.Next(0, 9);
            var captcha = string.Format("{0} + {1} = ?", a, b);

            //store answer
            Session["Captcha" + prefix] = a + b;

            //image stream
            FileContentResult img = null;

            using (var mem = new MemoryStream())
            using (var bmp = new Bitmap(130, 30))
            using (var gfx = Graphics.FromImage((Image)bmp))
            {
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));

                //add noise
                if (noisy)
                {
                    int i, r, x, y;
                    var pen = new Pen(Color.Yellow);
                    for (i = 1; i < 10; i++)
                    {
                        pen.Color = Color.FromArgb(
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)));

                        r = rand.Next(0, (130 / 3));
                        x = rand.Next(0, 130);
                        y = rand.Next(0, 30);

                        gfx.DrawEllipse(pen, x - r, y - r, r, r);
                    }
                }

                //add question
                gfx.DrawString(captcha, new Font("Tahoma", 15), Brushes.Gray, 2, 3);

                //render as Jpeg
                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
                img = this.File(mem.GetBuffer(), "image/Jpeg");
            }

            return img;
             * */

            return new CaptchaResult();
        }

        #region Logout

        public ActionResult LogOff()
        {
            int onUserId = this.currentSession.OnUserId;
            if (onUserId > 0)
            {
                return this.SignOut(onUserId, this.currentSession.UserName);
            }
            else
            {
                this.currentContext.Context.Session.Abandon();
            }

            return this.RedirectToAction("Login", "Account");
        }

        public int ReCheckIn()
        {
            var maintenanceInfo = Config.GetMaintenanceInfo();
            int onUserId = this.currentSession.OnUserId;

            if (maintenanceInfo.IsDownTime)
            {
                this.currentContext.Context.Session.Abandon();
                this.SignOut(onUserId, this.currentSession.UserName);
                return (int)StatusKickedOut.Maintenance;
            }

            var re_CheckIn = Config.Recheckin;
            var isKickOut = false;

            if (!re_CheckIn)
            {
                return (int)StatusKickedOut.Continue;
            }

            if (onUserId <= 0)
            {
                this.currentContext.Context.Session.Abandon();
                return (int)StatusKickedOut.KickedOut;
            }

            this.userService.Recheckin(onUserId, out isKickOut);

            if (!isKickOut)
            {
                return (int)StatusKickedOut.Continue;
            }
            else
            {
                this.SignOut(onUserId, this.currentSession.UserName);
                return (int)StatusKickedOut.KickedOut;
            }
        }

        #endregion Logout

        #region UM Maintenance

        /// <summary>
        /// Under Maintenance
        /// </summary>
        /// <returns>ActionResult object</returns>
        public ActionResult Offline()
        {
            var maintenanceInfo = Config.GetMaintenanceInfo();

            if (maintenanceInfo.IsDownTime)
            {
                ViewBag.UmFromTime = maintenanceInfo.TimeFrom;
                ViewBag.UmToTime = maintenanceInfo.TimeTo;
                return this.PartialView();
            }
            else
            {
                return this.RedirectToAction("Login", "Account");
            }
        }

        /// <summary>
        /// Process recheck-in offline mode
        /// </summary>
        /// <returns>Integer number</returns>
        public int ReCheckInOffline()
        {
            var maintenanceInfo = Config.GetMaintenanceInfo();
            if (!maintenanceInfo.IsDownTime)
            {
                return (int)StatusKickedOut.Login;
            }

            return (int)StatusKickedOut.Maintenance;
        }

        #endregion UM Maintenance

        [HttpGet]
        public ActionResult ExternalGroup()
        {
            Response.AppendHeader("Content-Encoding", "gzip");
            Response.Filter = new System.IO.Compression.GZipStream(Response.Filter, System.IO.Compression.CompressionMode.Compress);

            var maintenanceInfo = Config.GetMaintenanceInfo();

            if (maintenanceInfo.IsDownTime)
            {
                return this.RedirectToAction("Offline", "Account");
            }

            if (!this.currentSession.IsLogin && !this.currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return this.RedirectToAction("Login", "Account");
            }

            if (this.currentSession.IsLogin && this.currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return this.RedirectToAction("Index", "Home");
            }

            if (!this.currentSession.IsInternalAdmin)
            {
                return this.RedirectToAction("Index", "Home");
            }

            ViewBag.GroupList = this.userService.GroupList();

            return this.PartialView();
        }

        public ActionResult ExternalGroupList()
        {
            var maintenanceInfo = Config.GetMaintenanceInfo();

            if (maintenanceInfo.IsDownTime)
            {
                return this.RedirectToAction("Offline", "Account");
            }

            if (!this.currentSession.IsLogin && !this.currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return this.RedirectToAction("Login", "Account");
            }

            if (this.currentSession.IsLogin && this.currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return this.RedirectToAction("Index", "Home");
            }

            var result = this.userService.GroupList();

            return this.Json(result.Select(o => o.GroupName).ToArray(), JsonRequestBehavior.AllowGet);
        }

        private ActionResult SignOut(int onUserId, string userName)
        {
            this.userService.Logout(onUserId, userName);
            CurrentContext.Instance.Context.Session.Abandon();

            return this.RedirectToAction("Login", "Account");
        }
    }
}