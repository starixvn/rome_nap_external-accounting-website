namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Caching;
    using Core.Base;
    using Core.Context;
    using CustomerSetting.Security.Authenticate;
    using DTO.Report;
    using Excel.Reports.DailyWinLossReport;
    using Newtonsoft.Json;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Utilities;

    public class DailyWinlossController : BaseController
    {
        private readonly ICache _cache = CacheService.CreateInstance(CacheType.MemoryCache);
        private readonly IReportService _reportService = new ReportService();
        private string _cacheList = string.Empty;
        private int _custId;
        private string _groupname = string.Empty;
        private List<DailyWinLoss> _listDailyWinLoss;
        private int _memberId;
        private int _mrecommend;
        private int _recommend;
        private int _roleId = 5;
        private int _srecommend;
        private string _username = string.Empty;
        private int _viewlevel = 5;
        private DateTime _winlostDate = DateTime.Today;

        [ExternalAccountAuthenticate(Function = eFunctions.DailyWinloss,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult Index() => PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.DailyWinloss,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult GetDailyWinLoss()
        {
            GetRequestPage();
            _srecommend = CheckRequest.GetIntRequest("srecommend", 0);
            _mrecommend = CheckRequest.GetIntRequest("mrecommend", 0);
            _recommend = CheckRequest.GetIntRequest("recommend", 0);
            _cacheList = "CacheList" + _username + _groupname + _custId + _winlostDate;

            return _custId != 0 ? GetDailyWinLossDownline() : GetDailyWinLossSearch();
        }

        public void GetExportToExcel()
        {
            GetRequestPage();
            _cacheList = "CacheList" + _username + _groupname + _custId + _winlostDate;

            GetDailyWinLoss();

            var builder = new DailyWinLossReportBuilder(
                _listDailyWinLoss,
                false,
                _viewlevel,
                CheckRequest.GetStringRequest("date", string.Empty));

            var excelStream = builder.Build();
            if (excelStream != null)
            {
                ExportToExcel(excelStream,
                    LevelName(_viewlevel - 1) + "DailyWinLoss_" + _winlostDate.ToString("yyyyMMdd") + ".xls");
                excelStream.Dispose();
            }
        }

        public void GetRequestPage()
        {
            _username = CheckRequest.GetStringRequest("username", string.Empty);
            _viewlevel = CheckRequest.GetIntRequest("viewlevel", 0);
            _custId = CheckRequest.GetIntRequest("custid", 0);
            _groupname = CurrentSession.IsInternalAdmin || CurrentSession.IsITAdmin
                ? CheckRequest.GetStringRequest("groupname", string.Empty)
                : CurrentSession.CurrencyGroup;
            _winlostDate = CheckRequest.GetDateTimeRequest("date", DateTime.Today);
        }

        private bool CheckSearchCustomer()
        {
            if (!string.IsNullOrWhiteSpace(_username))
            {
                var custInfo = _reportService.GetCustomerInfo(_groupname, 0, _username, true);
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
                    viewlevel = 4,
                    supername = _username,
                    mastername = string.Empty,
                    agentname = string.Empty,
                    rows = string.Empty
                });
            }

            if (_roleId > 3)
            {
                _listDailyWinLoss = _reportService.GetDailyWinLoss(_winlostDate, _srecommend, _groupname);
                _cache.Set(_cacheList, _listDailyWinLoss);
                return JsonBase(
                    new
                    {
                        viewlevel = 4,
                        supername = _username,
                        mastername = string.Empty,
                        agentname = string.Empty,
                        rows = _listDailyWinLoss
                    },
                    new JsonSerializerSettings());
            }
            if (_roleId == 3)
            {
                _listDailyWinLoss = _reportService.GetDailyWinLossSuper(_winlostDate, _srecommend, _mrecommend);
                _cache.Set(_cacheList, _listDailyWinLoss);
                return JsonBase(
                    new
                    {
                        viewlevel = 3,
                        supername = string.Empty,
                        mastername = _username,
                        agentname = string.Empty,
                        srecommend = _srecommend,
                        mrecommend = _mrecommend,
                        rows = _listDailyWinLoss
                    },
                    new JsonSerializerSettings());
            }
            if (_roleId == 2)
            {
                _listDailyWinLoss = _reportService.GetDailyWinLossMaster(_winlostDate, _mrecommend, _recommend);
                _cache.Set(_cacheList, _listDailyWinLoss);
                return JsonBase(
                    new
                    {
                        viewlevel = 2,
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
            _listDailyWinLoss = _reportService.GetDailyWinLossAgent(_winlostDate, _recommend, _memberId);
            _cache.Set(_cacheList, _listDailyWinLoss);
            return JsonBase(
                new
                {
                    viewlevel = 1,
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
            if (_viewlevel == 4)
            {
                _listDailyWinLoss = _reportService.GetDailyWinLossSuper(_winlostDate, _custId, 0);
                _cache.Set(_cacheList, _listDailyWinLoss);
                return JsonBase(
                    new {viewlevel = 3, srecommend = _custId, rows = _listDailyWinLoss},
                    new JsonSerializerSettings());
            }
            if (_viewlevel == 3)
            {
                _listDailyWinLoss = _reportService.GetDailyWinLossMaster(_winlostDate, _custId, 0);
                _cache.Set(_cacheList, _listDailyWinLoss);
                return JsonBase(
                    new
                    {
                        viewlevel = 2,
                        srecommend = _srecommend,
                        mrecommend = _custId,
                        rows = _listDailyWinLoss
                    },
                    new JsonSerializerSettings());
            }
            _listDailyWinLoss = _reportService.GetDailyWinLossAgent(_winlostDate, _custId, 0);
            _cache.Set(_cacheList, _listDailyWinLoss);
            return JsonBase(
                new
                {
                    viewlevel = 1,
                    srecommend = _srecommend,
                    mrecommend = _mrecommend,
                    recommend = _custId,
                    rows = _listDailyWinLoss
                },
                new JsonSerializerSettings());
        }
    }
}