namespace Fanex.ExternalAccount.Statement.Controllers
{
    using Fanex.CustomerSetting.Module.Core.Web.Controllers;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Statement.Constants;
    using Fanex.ExternalAccount.Statement.Criteria;
    using Fanex.ExternalAccount.Statement.Criteria.StatementReport;
    using Fanex.ExternalAccount.Statement.Models.StatementReport;
    using Fanex.ExternalAccount.Statement.Resources;
    using Fanex.ExternalAccount.Statement.Services.StatementReport;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Statement.ViewModels;
    using Fanex.ExternalAccount.Core.Shared;
    
    public class StatementReportController : BaseController
    {
        public const string SessionKeyStatement = "Fanex.Razorbill.Report.Statement";

        public const string SessionKeyTransferDetails = "Fanex.Razorbill.Report.TransferDetails";

        private readonly IStatementReportService _statementReportService;
        
        private readonly IReportUtils _reportUtils = new ReportUtils();

        public StatementReportController() : this(null, null)
        {
        }

        public StatementReportController(IStatementReportService statementReportService, IReportUtils reportUtils)
        {
            _statementReportService = statementReportService ?? new StatementReportService();
            _reportUtils = reportUtils ?? new ReportUtils();
        }

        // [HttpGet]
        // public ActionResult Index()
        // {
        //     return this.View("~/Areas/Statement/Views/Report/Index.cshtml");
        // }

        public ActionResult Index()
        {
            return PartialView(new StatementViewModel
            {
                BalanceUpDate = _reportUtils.BalanceUpDate,
                MoveBetsDate = _reportUtils.MoveBetsDate
            });
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
            return File(_statementReportService.ExportMemberStatement(criteria, (StatementReportModel)Session[SessionKeyStatement]).ToArray(), "application/vnd.ms-excel", fileDownloadName);
        }

        [HttpGet, AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "Statement")]
        public ActionResult ExportSMAStatement(SMAStatementCriteria criteria)
        {
            var fileDownloadName = string.Format(Labels.StatementReportName, criteria.FromDate.ToString("MM_dd_yyyy"), criteria.ToDate.ToString("MM_dd_yyyy"));
            return File(_statementReportService.ExportSMAStatement(criteria, (StatementReportModel)Session[SessionKeyStatement]).ToArray(), "application/vnd.ms-excel", fileDownloadName);
        }

        [HttpGet, AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "Statement")]
        public ActionResult ExportTransferDetails(TransferDetailsCriteria criteria)
        {
            var fileDownloadName = string.Format(Labels.TransferDetails_ReportName, criteria.TransferDate);
            return File(_statementReportService.ExportTransferDetails(criteria, (StatementReportModel)Session[SessionKeyTransferDetails]).ToArray(), "application/vnd.ms-excel", fileDownloadName);
        }

        public JsonResult GetCustomerInfo(GetCustomerInformationByUsername criteria)
        {
            return Json(_statementReportService.GetCustomerInfo(criteria));
        }
    }
}