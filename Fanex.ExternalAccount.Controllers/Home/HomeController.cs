namespace Fanex.ExternalAccount.Controllers.Home
{
    using System;
using System.Collections.Generic;
using System.Configuration;
    using System.Web.Mvc;
    using Core.Context;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.Logging.Extensions.Web;

    public class HomeController : BaseController
    {
        [ExternalAccountAuthenticate]
        public ActionResult Index()
        {
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