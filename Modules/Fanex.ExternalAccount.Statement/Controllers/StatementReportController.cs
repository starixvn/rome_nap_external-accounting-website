namespace Fanex.ExternalAccount.Statement.Controllers
{
    using CustomerSetting.Module.Core.Criteria;
    using CustomerSetting.Module.Core.Services.Impl;
    using CustomerSetting.Module.Core.Web;
    using Fanex.CustomerSetting.Module.Core.Web.Controllers;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Statement.Constants;
    using Fanex.ExternalAccount.Statement.Criteria;
    using Fanex.ExternalAccount.Statement.Criteria.StatementReport;
    using Fanex.ExternalAccount.Statement.Models.StatementReport;
    using Fanex.ExternalAccount.Statement.Resources;
    using Fanex.ExternalAccount.Statement.Services.StatementReport;
    using System.Web.Mvc;

    public class StatementReportController : BaseController
    {
        public const string SessionKeyStatement = "Fanex.Razorbill.Report.Statement";

        public const string SessionKeyTransferDetails = "Fanex.Razorbill.Report.TransferDetails";

        private readonly IStatementReportService _statementReportService;

        public StatementReportController() :
            this(null)
        {
        }

        public StatementReportController(IStatementReportService statementReportService)
        {
            _statementReportService = statementReportService ?? new StatementReportService();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return this.View("~/Areas/Statement/Views/Report/Index.cshtml");
        }

        [HttpGet]
        public ActionResult Guideline(int roleId = RoleIdConstants.Member, bool canViewBetListStatement = false)
        {
            if (roleId == RoleIdConstants.Member)
            {
                return View("~/Areas/Statement/Views/Guideline/MemberGuildeline.cshtml", canViewBetListStatement);
            }

            return View("~/Areas/Statement/Views/Guideline/SMAGuildeline.cshtml", canViewBetListStatement);
        }

        [HttpPost, AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "Statement")]
        public JsonResult SMAStatement(SMAStatementCriteria criteria)
        {
            var statementList = _statementReportService.SMAStatement(criteria);
            Session[SessionKeyStatement] = statementList;
            return Json(statementList);
        }

        [HttpPost, AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "Statement")]
        public JsonResult MemberStatement(MemberStatementCriteria criteria)
        {
            var statementList = _statementReportService.MemberStatement(criteria);
            Session[SessionKeyStatement] = statementList;
            return Json(statementList);
        }

        [HttpGet, AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "Statement")]
        public ActionResult TransferDetails(TransferDetailsCriteria criteria)
        {
            var statementList = _statementReportService.TransferDetails(criteria);
            Session[SessionKeyTransferDetails] = statementList;
            ViewBag.TransferDetailsCriteria = criteria;
            return View("~/Areas/Statement/Views/StatementDetails/Index.cshtml", statementList);
        }

        [HttpGet, AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "Statement")]
        public ActionResult ExportMemberStatement(MemberStatementCriteria criteria)
        {
            var fileDownloadName = string.Format(Labels.StatementReportName, criteria.FromDate.ToString("MM_dd_yyyy"), criteria.ToDate.ToString("MM_dd_yyyy"));
            return File(
                _statementReportService.ExportMemberStatement(criteria, (StatementReportModel)Session[SessionKeyStatement]).ToArray(),
                "application/vnd.ms-excel",
                fileDownloadName);
        }

        [HttpGet, AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "Statement")]
        public ActionResult ExportSMAStatement(SMAStatementCriteria criteria)
        {
            var fileDownloadName = string.Format(Labels.StatementReportName, criteria.FromDate.ToString("MM_dd_yyyy"), criteria.ToDate.ToString("MM_dd_yyyy"));
            return File(
                _statementReportService.ExportSMAStatement(criteria, (StatementReportModel)Session[SessionKeyStatement]).ToArray(),
                "application/vnd.ms-excel",
                fileDownloadName);
        }

        [HttpGet, AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "Statement")]
        public ActionResult ExportTransferDetails(TransferDetailsCriteria criteria)
        {
            var fileDownloadName = string.Format(Labels.TransferDetails_ReportName, criteria.TransferDate);
            return File(
                _statementReportService.ExportTransferDetails(criteria, (StatementReportModel)Session[SessionKeyTransferDetails]).ToArray(),
                "application/vnd.ms-excel",
                fileDownloadName);
        }

        public JsonResult GetCustomerInfo(GetCustomerInformationByUsername criteria)
        {
            return Json(_statementReportService.GetCustomerInfo(criteria));
        }

        public JsonResult CheckInternalByUsername(CheckInternalByUsernameCriteria criteria)
        {
            return Json(_statementReportService.CheckInternalByUsername(criteria));
        }
       
    }
}
