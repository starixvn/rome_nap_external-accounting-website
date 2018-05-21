namespace Fanex.ExternalAccount.Controllers.Home
{
    using System;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using Logging.Extensions.Web;
    using ViewModels;
    using CurrentSession = Core.Context.CurrentSession;

    public class HomeController : BaseController
    {
        private readonly CurrentSession _currentSession;
        public HomeController()
        {
            _currentSession = new CurrentSession();
        }

        [ExternalAccountAuthenticate]
        public ActionResult Index()
           => View(new HomeViewModel
           {
               Username = _currentSession.UserName,
               Code = _currentSession.OTPEncryptCode,
               EnabledOTP = _currentSession.EnabledOTP,
               ModifiedDate = _currentSession.OTPModifiedDate
           });

        public ActionResult TestLog()
        {
            ExceptionHandler.Handle(new Exception("Testing log"));

            return View();
        }

        //private void LogUserNWSpeed()
    }
}