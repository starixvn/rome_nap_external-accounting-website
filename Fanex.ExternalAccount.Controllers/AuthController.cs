using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using System.Web.Security;

namespace ParlayRMS.Controllers
{
    public class AuthController : Controller
    {
        public RedirectToRouteResult Index()
        {
            return RedirectToAction("Login");
        }
    }
}