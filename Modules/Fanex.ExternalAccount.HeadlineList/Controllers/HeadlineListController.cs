using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fanex.ExternalAccount.Core.Base;
using Fanex.ExternalAccount.Core.Context;
using Fanex.ExternalAccount.Core.Shared;
using Fanex.ExternalAccount.HeadlineList.Services;
using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;

namespace Fanex.ExternalAccount.HeadlineList.Controllers
{
    using System.Web.Mvc;
    public class HeadlineListController :BaseController
    {
        private readonly IHeadlineListService service;
        private readonly IHeadlineListExcel excel;

        public HeadlineListController()
            : this(null, null)
        {
        }

        public HeadlineListController(IHeadlineListService service, IHeadlineListExcel excel)
        {
            this.service = service ?? new HeadlineListService();
            this.excel = excel ?? new HeadlineListExcel();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.HeadlineList, Policy = AuthenticationType.Read)]
        public ActionResult Index()
            => PartialViewBase();

     
        [ExternalAccountAuthenticate(Function = eFunctions.HeadlineList, Policy = AuthenticationType.Read)]
        public ActionResult Search()
        {
            var mockData = new List<Object>()
            {
                new
                {
                    Id = 1,
                    Name = "Test 1"
                },
                new
                {
                    Id = 2,
                    Name = "Test 2"
                }
            };
           return Json(mockData, JsonRequestBehavior.AllowGet);
        }
    }
}