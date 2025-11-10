using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Fanex.ExternalAccount.ReportsAndStatements.Utilities;
using Fanex.ExternalAccount.ReportsAndStatements.Services.Reports;
using Fanex.ExternalAccount.ReportsAndStatements.Models.DTOs.Reports;

namespace Fanex.ExternalAccount.ReportsAndStatements.Controllers.Reports
{
    public class DailyWinlossController : BaseReportController
    {
        private int _custId;
        private string _groupname = string.Empty;
        private IEnumerable<Fanex.ExternalAccount.ReportsAndStatements.Models.DTOs.Reports.DailyWinLoss> _listDailyWinLoss = new List<Fanex.ExternalAccount.ReportsAndStatements.Models.DTOs.Reports.DailyWinLoss>();
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

        public IActionResult Index()
        {
            ViewBag.WillShowMessage = CanView(Config.GroupsCanViewWinLossDetailForIBCBetLiveCasino());
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetDailyWinLoss()
        {
            GetRequestPage();
            _srecommend = GetIntRequest("srecommend", 0);
            _mrecommend = GetIntRequest("mrecommend", 0);
            _recommend = GetIntRequest("recommend", 0);
            return _custId != 0 ? GetDailyWinLossDownline() : GetDailyWinLossSearch();
        }

        public IActionResult GetExportToExcel()
        {
            GetRequestPage();
            GetDailyWinLoss();
            // Export logic would be implemented here
            return File(new byte[0], "application/vnd.ms-excel", $"DailyWinLoss_{_winlostDate:yyyyMMdd}.xls");
        }

        private void GetRequestPage()
        {
            _username = GetStringRequest("username", string.Empty);
            _viewlevel = GetIntRequest("viewlevel", 0);
            _custId = GetIntRequest("custid", 0);
            _groupname = GetGroupName();
            _winlostDate = GetDateTimeRequest("date", DateTime.Today);
        }

        private bool CheckSearchCustomer()
        {
            if (!string.IsNullOrWhiteSpace(_username))
            {
                var custInfo = reportService.SearchCustomerInfo(_groupname, 0, _username, true);
                if (custInfo == null)
                {
                    return false;
                }

                _roleId = custInfo.RoleId;
                _srecommend = custInfo.SuperId;
                _mrecommend = custInfo.MasterId;
                _recommend = custInfo.AgentId;
                _memberId = custInfo.CustId;
            }

            return true;
        }

        private IActionResult GetDailyWinLossSearch()
        {
            if (!CheckSearchCustomer())
            {
                return Json(new
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
                return Json(new
                {
                    viewlevel = Role.Super,
                    supername = _username,
                    mastername = string.Empty,
                    agentname = string.Empty,
                    rows = _listDailyWinLoss
                });
            }

            if (_roleId.IsMaster())
            {
                _listDailyWinLoss = reportService.GetDailyWinLossSuper(_winlostDate, _srecommend, _mrecommend, 0);
                return Json(new
                {
                    viewlevel = Role.Master,
                    supername = string.Empty,
                    mastername = _username,
                    agentname = string.Empty,
                    srecommend = _srecommend,
                    mrecommend = _mrecommend,
                    rows = _listDailyWinLoss
                });
            }

            if (_roleId.IsAgent())
            {
                _listDailyWinLoss = reportService.GetDailyWinLossMaster(_winlostDate, _mrecommend, _recommend, 0);
                return Json(new
                {
                    viewlevel = Role.Agent,
                    supername = string.Empty,
                    mastername = string.Empty,
                    agentname = _username,
                    srecommend = _srecommend,
                    mrecommend = _mrecommend,
                    recommend = _recommend,
                    rows = _listDailyWinLoss
                });
            }

            if (_roleId.IsMember() && _recommend.IsDirectMemberOfSuper())
            {
                _listDailyWinLoss = reportService.GetDailyWinLossSuper(_winlostDate, _srecommend, 0, _memberId);
                return Json(new
                {
                    viewlevel = Role.Master,
                    supername = string.Empty,
                    mastername = _username,
                    agentname = string.Empty,
                    srecommend = _srecommend,
                    mrecommend = _mrecommend,
                    rows = _listDailyWinLoss
                });
            }

            if (_roleId.IsMember() && _recommend.IsDirectMemberOfMaster())
            {
                _listDailyWinLoss = reportService.GetDailyWinLossMaster(_winlostDate, _mrecommend, 0, _memberId);
                return Json(new
                {
                    viewlevel = Role.Agent,
                    supername = string.Empty,
                    mastername = string.Empty,
                    agentname = _username,
                    srecommend = _srecommend,
                    mrecommend = _mrecommend,
                    recommend = _recommend,
                    rows = _listDailyWinLoss
                });
            }

            _listDailyWinLoss = reportService.GetDailyWinLossAgent(_winlostDate, _recommend, _memberId);
            return Json(new
            {
                viewlevel = Role.Member,
                supername = string.Empty,
                mastername = string.Empty,
                agentname = string.Empty,
                srecommend = _srecommend,
                mrecommend = _mrecommend,
                recommend = _recommend,
                rows = _listDailyWinLoss
            });
        }

        private IActionResult GetDailyWinLossDownline()
        {
            if (_viewlevel.IsSuper())
            {
                _listDailyWinLoss = reportService.GetDailyWinLossSuper(_winlostDate, _custId, 0, 0);
                return Json(new
                {
                    viewlevel = Role.Master,
                    srecommend = _custId,
                    rows = _listDailyWinLoss
                });
            }

            if (_viewlevel.IsMaster())
            {
                _listDailyWinLoss = reportService.GetDailyWinLossMaster(_winlostDate, _custId, 0, 0);
                return Json(new
                {
                    viewlevel = Role.Agent,
                    srecommend = _srecommend,
                    mrecommend = _custId,
                    rows = _listDailyWinLoss
                });
            }

            _listDailyWinLoss = reportService.GetDailyWinLossAgent(_winlostDate, _custId, 0);
            return Json(new
            {
                viewlevel = Role.Member,
                srecommend = _srecommend,
                mrecommend = _mrecommend,
                recommend = _custId,
                rows = _listDailyWinLoss
            });
        }

        // Helper methods migrated from CheckRequest and ExtRequest
        private int GetIntRequest(string name, int defaultValue)
        {
            var request = HttpContext.Request;
            if (string.IsNullOrEmpty(request.Form[name].ToString()))
            {
                return defaultValue;
            }

            return int.TryParse(request.Form[name], out int value) ? value : defaultValue;
        }

        private DateTime GetDateTimeRequest(string name, DateTime defaultValue)
        {
            var request = HttpContext.Request;
            if (string.IsNullOrEmpty(request.Form[name].ToString()))
            {
                return defaultValue;
            }

            return DateTime.TryParse(request.Form[name], out DateTime value) ? value : defaultValue;
        }

        private string GetStringRequest(string name, string defaultValue)
        {
            var request = HttpContext.Request;
            return string.IsNullOrEmpty(request.Form[name].ToString()) ? defaultValue : request.Form[name].ToString();
        }

        private string GetGroupName()
        {
            var group = HttpContext.Request.Form["groupName"].ToString();
            return string.IsNullOrEmpty(group) ? currentSession.ParentGroupName : group;
        }
    }

    // Placeholder classes - these need to be properly migrated

    public class Role
    {
        public const int Group = 0;
        public const int Super = 1;
        public const int Master = 2;
        public const int Agent = 3;
        public const int Member = 4;
    }

}
