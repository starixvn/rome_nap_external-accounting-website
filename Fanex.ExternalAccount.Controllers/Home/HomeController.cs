namespace Fanex.ExternalAccount.Controllers.Home
{
    using System;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using Logging.Extensions.Web;

    public class HomeController : BaseController
    {
        [ExternalAccountAuthenticate]
        public ActionResult Index(string _cd)
        {
            ViewBag._cd = _cd;
            return View();
        }

        public ActionResult TestLog()
        {
            ExceptionHandler.Handle(new Exception("Testing log"));

            return View();
        }

        

        //private void LogUserNWSpeed()
    }
}