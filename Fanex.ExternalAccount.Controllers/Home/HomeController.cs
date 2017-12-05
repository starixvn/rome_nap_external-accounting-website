namespace Fanex.ExternalAccount.Controllers.Home
{
    using System;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Utilities;
using Fanex.Logging.Extensions.Web;

    [CheckUMFilter]
    public class HomeController : BaseController
    {
        [Authorize]
        public ActionResult Index() => View();

        public ActionResult TestLog()
        {
            ExceptionHandler.Handle(new Exception("Testing log"));

            return View();
        }
    }
}