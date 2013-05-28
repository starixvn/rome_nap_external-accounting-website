// --------------------------------------------------------------------------
// <copyright file="ReportRequestTicketController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

using Fanex.Puffin.Core.Web.Controllers;
using Fanex.Puffin.Core.Web.Security;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using Fanex.Puffin.Report.Criteria.ReportRequestTicket;
using Fanex.Puffin.Report.Services.Implement;
using Fanex.Puffin.Report.Services.Interface;

namespace Fanex.Puffin.Report.Controllers
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class ReportRequestTicketController : BaseController
    {
        private IReportRequestTicketService _reportService;

        private const string FunctionKeyName = "ReportRequestTicket";

        public ReportRequestTicketController() :
            this(null)
        {

            var test = GetActionListByController<ReportRequestTicketController>();

        }

        public ReportRequestTicketController(IReportRequestTicketService reportService)
        {
            this._reportService = reportService ?? new ReportRequestTicketService();
        }

        [HttpGet]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "Index")]
        public ActionResult Index()
        {
            return base.View("~/Areas/Report/Views/ReportRequestTicket/Index.cshtml");
        }

        [HttpPost]
        public JsonResult GetRequestTicketReport(GetRequestTicketReportCriteria criteria)
        {
            var report = this._reportService.GetRequestTicketReport(criteria);
            return base.Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMaxRequestNo(GetMaxRequestNoCriteria criteria)
        {
            var report = this._reportService.GetMaxRequestNo(criteria);
            return base.Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "Index")]
        public JsonResult AddNewRequestTicket(AddNewRequestTicketCriteria criteria)
        {
            var report = this._reportService.AddNewRequestTicket(criteria);
            return base.Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "Index")]
        public JsonResult DeleteRequestTicket(DeleteRequestTicketCriteria criteria)
        {
            var report = this._reportService.DeleteRequestTicket(criteria);
            return base.Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "Index")]
        public JsonResult UpdateRequestTicket(UpdateRequestTicketCriteria criteria)
        {
            var report = this._reportService.UpdateRequestTicket(criteria);
            return base.Json(report, JsonRequestBehavior.AllowGet);
        }
    }
}
