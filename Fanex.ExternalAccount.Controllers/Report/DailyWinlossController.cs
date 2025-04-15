namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DTO.Report;
    using Excel.Reports.DailyWinLossReport;
    using Newtonsoft.Json;
    using Service.Interface.Report;
    using Utilities;

    public class DailyWinlossController : BaseReportController
    {
        private int _custId;
        private string _groupname = string.Empty;
        private List<DailyWinLoss> _listDailyWinLoss;
        private int _memberId;
        private int _mrecommend;
        private int _recommend;
        private int _roleId = Role.Group;
        private int _srecommend;
        private string _username = string.Empty;
        private int _viewlevel = Role.Group;
        private DateTime _winlostDate = DateTime.Today;

        public DailyWinlossController(
            CurrentSession currentSession,
            IReportService reportService)
            : base(currentSession, reportService)
        {
        }

        public DailyWinlossController() : this(null, null)
        {
        }

        [ExternalAccountAuthenticate(Function = eFunctions.DailyWinloss,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult Index()
        {
            ViewBag.WillShowMessage = CanView(Config.GroupsCanViewWinLossDetailForIBCBetLiveCasino());

            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.DailyWinloss,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult GetDailyWinLoss()
        {
            GetRequestPage();
            _srecommend = CheckRequest.GetIntRequest("srecommend", 0);
            _mrecommend = CheckRequest.GetIntRequest("mrecommend", 0);
            _recommend = CheckRequest.GetIntRequest("recommend", 0);
            return _custId != 0 ? GetDailyWinLossDownline() : GetDailyWinLossSearch();
        }

        public void GetExportToExcel()
        {
            GetRequestPage();
            GetDailyWinLoss();
            var builder = new DailyWinLossReportBuilder(
                _listDailyWinLoss ?? new List<DailyWinLoss>(),
                false,
                _viewlevel,
                CheckRequest.GetStringRequest("date", string.Empty));

            var excelStream = builder.Build();
            if (excelStream != null)
            {
                ExportToExcel(excelStream,
                    (_viewlevel - 1).DisplayRoleName() + "DailyWinLoss_" + _winlostDate.ToString("yyyyMMdd") + ".xls");
                excelStream.Dispose();
            }
        }

        public void GetRequestPage()
        {
            _username = CheckRequest.GetStringRequest("username", string.Empty);
            _viewlevel = CheckRequest.GetIntRequest("viewlevel", 0);
            _custId = CheckRequest.GetIntRequest("custid", 0);
            _groupname = ExtRequest.GetGroupName();
            _winlostDate = CheckRequest.GetDateTimeRequest("date", DateTime.Today);
        }

        private bool CheckSearchCustomer()
        {
            if (!string.IsNullOrWhiteSpace(_username))
            {
                var custInfo = reportService.GetCustomerInfo(_groupname, 0, _username, true);
                if (custInfo == null || custInfo.Rows.Count == 0)
                {
                    return false;
                }
                
                _roleId = custInfo.Rows[0].ToInt32("roleid");
                _srecommend = custInfo.Rows[0].ToInt32("srecommend");
                _mrecommend = custInfo.Rows[0].ToInt32("mrecommend");
                _recommend = custInfo.Rows[0].ToInt32("recommend");
                _memberId = custInfo.Rows[0].ToInt32("custid");
            }

            return true;
        }

        private ActionResult GetDailyWinLossSearch()
        {
            if (!CheckSearchCustomer())
            {
                return JsonBase(new
                {
                    viewlevel = Role.Super,
                    supername = _username,
                    mastername = string.Empty,
                    agentname = string.Empty,
                    rows = string.Empty
                });
            }

            if (_roleId > Role.Master)
            {
                _listDailyWinLoss = reportService.GetDailyWinLoss(_winlostDate, _srecommend, _groupname);
                return JsonBase(
                    new
                    {
                        viewlevel = Role.Super,
                        supername = _username,
                        mastername = string.Empty,
                        agentname = string.Empty,
                        rows = _listDailyWinLoss
                    },
                    new JsonSerializerSettings());
            }
            if (_roleId.IsMaster())
            {
                _listDailyWinLoss = reportService.GetDailyWinLossSuper(_winlostDate, _srecommend, _mrecommend, 0);
                return JsonBase(
                    new
                    {
                        viewlevel = Role.Master,
                        supername = string.Empty,
                        mastername = _username,
                        agentname = string.Empty,
                        srecommend = _srecommend,
                        mrecommend = _mrecommend,
                        rows = _listDailyWinLoss
                    },
                    new JsonSerializerSettings());
            }
            if (_roleId.IsAgent())
            {
                _listDailyWinLoss = reportService.GetDailyWinLossMaster(_winlostDate, _mrecommend, _recommend, 0);
                return JsonBase(
                    new
                    {
                        viewlevel = Role.Agent,
                        supername = string.Empty,
                        mastername = string.Empty,
                        agentname = _username,
                        srecommend = _srecommend,
                        mrecommend = _mrecommend,
                        recommend = _recommend,
                        rows = _listDailyWinLoss
                    },
                    new JsonSerializerSettings());
            }

            if (_roleId.IsMember() && _recommend.IsDirectMemberOfSuper())
            {
                _listDailyWinLoss = reportService.GetDailyWinLossSuper(_winlostDate, _srecommend, 0, _memberId);
                return JsonBase(
                    new
                    {
                        viewlevel = Role.Master,
                        supername = string.Empty,
                        mastername = _username,
                        agentname = string.Empty,
                        srecommend = _srecommend,
                        mrecommend = _mrecommend,
                        rows = _listDailyWinLoss
                    },
                    new JsonSerializerSettings());
            }
            
            if (_roleId.IsMember() && _recommend.IsDirectMemberOfMaster())
            {
                _listDailyWinLoss = reportService.GetDailyWinLossMaster(_winlostDate, _mrecommend, 0, _memberId);
                return JsonBase(
                    new
                    {
                        viewlevel = Role.Agent,
                        supername = string.Empty,
                        mastername = string.Empty,
                        agentname = _username,
                        srecommend = _srecommend,
                        mrecommend = _mrecommend,
                        recommend = _recommend,
                        rows = _listDailyWinLoss
                    },
                    new JsonSerializerSettings());
            }
            _listDailyWinLoss = reportService.GetDailyWinLossAgent(_winlostDate, _recommend, _memberId);
            return JsonBase(
                new
                {
                    viewlevel = Role.Member,
                    supername = string.Empty,
                    mastername = string.Empty,
                    agentname = string.Empty,
                    srecommend = _srecommend,
                    mrecommend = _mrecommend,
                    recommend = _recommend,
                    rows = _listDailyWinLoss
                },
                new JsonSerializerSettings());
        }

        private ActionResult GetDailyWinLossDownline()
        {
            if (_viewlevel.IsSuper())
            {
                _listDailyWinLoss = reportService.GetDailyWinLossSuper(_winlostDate, _custId, 0,0);
                return JsonBase(
                    new
                    {
                        viewlevel = Role.Master,
                        srecommend = _custId,
                        rows = _listDailyWinLoss
                    },
                    new JsonSerializerSettings());
            }
            if (_viewlevel.IsMaster())
            {
                _listDailyWinLoss = reportService.GetDailyWinLossMaster(_winlostDate, _custId, 0, 0);
                return JsonBase(
                    new
                    {
                        viewlevel = Role.Agent,
                        srecommend = _srecommend,
                        mrecommend = _custId,
                        rows = _listDailyWinLoss
                    },
                    new JsonSerializerSettings());
            }

            _listDailyWinLoss = reportService.GetDailyWinLossAgent(_winlostDate, _custId, 0);
            return JsonBase(
                new
                {
                    viewlevel = Role.Member,
                    srecommend = _srecommend,
                    mrecommend = _mrecommend,
                    recommend = _custId,
                    rows = _listDailyWinLoss
                },
                new JsonSerializerSettings());
        }
    }
}