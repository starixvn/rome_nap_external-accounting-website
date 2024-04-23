namespace Fanex.ExternalAccount.HeadlineList.Controllers
{
    using System;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using Services;
    using ViewModels;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;
    
    public class HeadlineListController : BaseController
    {
        private readonly IHeadlineListService service;
        private readonly IHeadlineListExcel excel;

        public HeadlineListController() : this(null, null) { }

        public HeadlineListController(IHeadlineListService service, IHeadlineListExcel excel)
        {
            this.service = service ?? new HeadlineListService();
            this.excel = excel ?? new HeadlineListExcel();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.HeadlineList, Policy = AuthenticationType.Read)]
        public ActionResult Index() => PartialViewBase();


        [ExternalAccountAuthenticate(Function = eFunctions.HeadlineList, Policy = AuthenticationType.Read)]
        public ActionResult Search(SearchHeadlineQuery query)
        {
            var headlineResult = service.GetHeadlines(query, CurrentSession.UserId, ExtRequest.GetGroupName());
            return Json(headlineResult, JsonRequestBehavior.AllowGet);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.HeadlineList, Policy = AuthenticationType.Read)]
        public ActionResult ExportHeadlines(SearchHeadlineQuery query)
        {
            query.PageSize = int.MaxValue;
            query.Page = 1;
            var data = service.GetHeadlines(query, CurrentSession.UserId,ExtRequest.GetGroupName());
            var excelFile = excel.ExportHeadlinesExcel(data).ToArray();
            var today = DateTime.Now.ToString("yyyyMMdd");
            var fileName = $"SMAListByHeadline{today}.xls";
           
            return File(excelFile, "application/vnd.ms-excel", fileName);
        }
    }
}