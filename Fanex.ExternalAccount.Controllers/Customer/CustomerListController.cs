namespace Fanex.ExternalAccount.Controllers.Customer
{
    using Fanex.ExternalAccount.Core.Base;
    using System.Web.Mvc;

    public class CustomerListController: CustomerBase
    {
        public CustomerListController()
        {

        }

        public ActionResult Index() => PartialViewBase();

        public ActionResult GetSuperList()
        {
            return View();
        }

        public ActionResult GetMasterList()
        {
            return View();
        }

        public ActionResult GetAgentList()
        {
            return View();
        }

        public ActionResult GetMemberList()
        {
            return View();
        }

        public JsonResult GetCustomerInfo(int custId)
        {
            return Json(base.GetCustomerInfo(custId, CurrentSession.CurrencyGroup));
        }
    }
}
