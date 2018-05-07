namespace Fanex.ExternalAccount.Controllers.Home
{
    using System;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using Logging.Extensions.Web;
    using CurrentSession = Core.Context.CurrentSession;

    public class HomeController : BaseController
    {
        private readonly CurrentSession _currentSession;
        public HomeController()
        {
            _currentSession = new CurrentSession();
        }

        [ExternalAccountAuthenticate]
        public ActionResult Index(string _cd)
        {
            ViewBag._cd = _cd;
            ViewBag._enabledOTP = _currentSession.EnaledOTP;

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