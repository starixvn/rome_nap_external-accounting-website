namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using Caching;
    using CasinoLibrary.Model.UserException;
    using Common;
    using Core.Base;
    using Core.Context;
    using CustomerSetting.IP.Utils;
    using CustomerSetting.Security.Authenticate;
    using DTO;
    using Excel.Customer.CustomerListPage;
    using IBCUtils;
    using Logging.Extensions.Web;
    using Module.NG2.Entities;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    using PTEngine.BusinessObjects;
    using Service.Impl.Customer;
    using Service.Impl.Customer.Casino;
    using Service.Impl.Report;
    using Utilities;
    using ViewModel.Customer;
    using Customer = PTEngine.BusinessObjects.Customer;

    public class CustomerController : CustomerBase
    {
        private readonly AgentService _agentService = new AgentService();
        private readonly ICache _cache = CacheService.CreateInstance(CacheType.MemoryCache);
        private readonly CustomerService _customerService = new CustomerService();
        private readonly MasterService _masterService = new MasterService();
        private readonly MemberService _memberService = new MemberService();
        private readonly SuperService _superService = new SuperService();
        private string _cacheList = string.Empty;

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult CheckUsername(string username)
        {
            var errCode = 0;
            string errMsg;
            var roleId = 0;
            var isSubAccount = false;
            int customerId;

            var isValidUser = _customerService.IsValidUsername(username.Trim(), CurrentSession.CustId, out customerId,
                out isSubAccount, out roleId);

            if (isValidUser)
            {
                errMsg = customerId.ToString();
                errCode = 101;
            }
            else
            {
                errCode = 999;
                errMsg = ViewLang.notExist;
            }

            return JsonBase(
                new {errCode, errMsg},
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Transfer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult SuperAddNewMaster() => PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Transfer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult SuperAddNewMasterGet(int custId, int parentId, string parentUsername, int parentCurrencyId)
        {
            var context = new RequestContext
            {
                AdminId = parentId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.Add,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int) CustomerRole.Super,
                TargetRoleId = (int) CustomerRole.Master,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = parentUsername.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            try
            {
                customer = new PTEngine.Services.CustomerService().GetCustomerForAddNewMaster(context);
                customer.CurrencyId = parentCurrencyId;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return JsonBase(
                new {context, customer},
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Transfer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult SuperAddNewMasterPost(int custId, int parentId, string txtCustomerObject)
        {
            if (!IsBelong(Request["groupName"], parentId, null, true))
            {
                return ActionDenied(Request);
            }


            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};

            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            IRequestContext requestContext = new RequestContext
            {
                CustId = parentId,
                TargetCustId = 0,
                RoleId = (int) CustomerRole.Super,
                TargetRoleId = (int) CustomerRole.Master,
                FormId = FormType.Add,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                SystemId = (byte) Config.SystemId
            };

            int masterId;
            var result = new PTEngine.Services.CustomerService().AddNewMaster(customer, requestContext, out masterId);

            if (string.IsNullOrWhiteSpace(result.ErrorCode) || result.ErrorCode == "0")
            {
                UpdateCasinoStatus(masterId, result, customer);
                Thread.Sleep(3000);
                AutoInsertNG2Product(masterId, customer.UserName, RoleId.Master, parentId, SiteId.NAP,
                    customer.CurrencyId);

                var autoInsertLiveCasinoTask = AutoInsertLiveCasinoPost(masterId, parentId, CurrentSession.UserId,
                    CurrentSession.UserName, requestContext.IP);

                AutoInsertCasinoPost(masterId, parentId);

                autoInsertLiveCasinoTask.Wait();
            }

            return JsonBase(new {result.ErrorCode, result.ErrorMessage, CustId = masterId});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Transfer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult MasterAddNewAgent() => PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Transfer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult MasterAddNewAgentGet(int custId, int parentId, string parentUsername, int parentCurrencyId)
        {
            var context = new RequestContext
            {
                AdminId = parentId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.Add,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int) CustomerRole.Master,
                TargetRoleId = (int) CustomerRole.Agent,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = parentUsername.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            try
            {
                customer = new PTEngine.Services.CustomerService().GetCustomerForAddNewAgent(context);
                customer.CurrencyId = parentCurrencyId;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return JsonBase(
                new {context, customer},
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Transfer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult MasterAddNewAgentPost(int custId, int parentId, string txtCustomerObject)
        {
            if (!IsBelong(Request["groupName"], parentId, null, true))
            {
                return ActionDenied(Request);
            }


            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};

            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            var requestContext = new RequestContext
            {
                CustId = parentId,
                TargetCustId = 0,
                RoleId = (int) CustomerRole.Master,
                TargetRoleId = (int) CustomerRole.Agent,
                FormId = FormType.Add,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                SystemId = (byte) Config.SystemId
            };

            int agentId;
            var result = new PTEngine.Services.CustomerService().AddNewAgent(customer, requestContext, out agentId);

            if (string.IsNullOrWhiteSpace(result.ErrorCode) || result.ErrorCode == "0")
            {
                UpdateCasinoStatus(agentId, result, customer);
                Thread.Sleep(3000);
                AutoInsertNG2Product(agentId, customer.UserName, RoleId.Agent, parentId, SiteId.NAP,
                    customer.CurrencyId);

                var autoInsertLiveCasinoTask = AutoInsertLiveCasinoPost(agentId, parentId, CurrentSession.UserId,
                    CurrentSession.UserName, requestContext.IP);

                AutoInsertCasinoPost(agentId, parentId);

                autoInsertLiveCasinoTask.Wait();
            }

            return JsonBase(new {result.ErrorCode, result.ErrorMessage, CustId = agentId});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Transfer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult AgentAddNewMember() => PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Transfer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult AgentAddNewMemberGet(int custId, int parentId, string parentUsername, int parentCurrencyId)
        {
            var context = new RequestContext
            {
                AdminId = parentId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.Add,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int) CustomerRole.Agent,
                TargetRoleId = (int) CustomerRole.Member,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = parentUsername.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            try
            {
                customer = new PTEngine.Services.CustomerService().GetCustomerForAddNewMember(context);
                customer.CurrencyId = parentCurrencyId;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return JsonBase(
                new {context, customer},
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ExternalAccountAuthenticate(
            Function = eFunctions.Transfer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult AgentAddNewMemberPost(int custId, int parentId, string txtCustomerObject)
        {
            if (!IsBelong(Request["groupName"], parentId, null, true))
            {
                return ActionDenied(Request);
            }


            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};

            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            IRequestContext requestContext = new RequestContext
            {
                CustId = parentId,
                TargetCustId = 0,
                RoleId = (int) CustomerRole.Agent,
                TargetRoleId = (int) CustomerRole.Member,
                FormId = FormType.Add,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                SystemId = (byte) Config.SystemId
            };

            var memberId = -1;
            var result = new PTEngine.Services.CustomerService().AddNewMember(customer, requestContext, out memberId);

            if (string.IsNullOrWhiteSpace(result.ErrorCode) || result.ErrorCode == "0")
            {
                UpdateCasinoStatus(memberId, result, customer);
                Thread.Sleep(3000);
                AutoInsertNG2Product(memberId, customer.UserName, RoleId.Member, parentId, SiteId.NAP,
                    customer.CurrencyId);

                var autoInsertLiveCasinoTask = AutoInsertLiveCasinoPost(memberId, parentId, CurrentSession.UserId,
                    CurrentSession.UserName, requestContext.IP);

                AutoInsertCasinoPost(memberId, parentId);

                autoInsertLiveCasinoTask.Wait();
            }

            return JsonBase(new {result.ErrorCode, result.ErrorMessage, CustId = memberId});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult CustomerList() => PartialViewBase();

        public ActionResult Settings(int id, int custId, int uplineId, string userName, string groupName)
        {
            DataTable result;
            var srecommend = 0;
            var mrecommend = 0;
            var recommend = 0;
            var roleId = 0;
            var userId = 0;

            GetDataResult(groupName, null, null, null, userName, out result, out srecommend, out mrecommend,
                out recommend, out userId, out roleId);

            ViewBag.CurrentLevel = id;
            ViewBag.UserId = custId;
            ViewBag.UplineId = uplineId;
            ViewBag.SyncCasino = (bool) result.Rows[0]["SyncCasino"];
            ViewBag.SyncLiveCasino = (bool) result.Rows[0]["SyncLiveCasino"];
            ViewBag.HideVirtualSports = (bool) result.Rows[0]["HideVirtualSports"];
            ViewBag.HideKeno = (bool) result.Rows[0]["HideKeno"];
            ViewBag.HideGoldDeluxe = (bool) result.Rows[0]["HideGoldDeluxe"];
            ViewBag.HideColossusBets = (bool) result.Rows[0]["HideColossusBet"];
            ViewBag.HideSportsbook2 = Converter.ToBool(result.Rows[0]["HideMuayThai"]);
            ViewBag.HideCockfighting = Converter.ToBool(result.Rows[0]["HideCockfighting"]);
            ViewBag.HideCricket = Converter.ToBool(result.Rows[0]["HideCricket"]);
            ViewBag.HideAllbet = Utils.ToBoolean(result.Rows[0]["HideAllBet"]);
            ViewBag.HideVoidbridge = Utils.ToBoolean(result.Rows[0]["HideVoidBridge"]);
            ViewBag.HideNG2 = !GetDcsStatuses(custId, id, 23).IsSyncedUpline;
            ViewBag.HideFishingWorld = Utils.ToBoolean(result.Rows[0]["HideFishingWorld"]);
            ViewBag.HideLottery = Utils.ToBoolean(result.Rows[0]["HideLottery"]);
            ViewBag.HideLucky3 = Utils.ToBoolean(result.Rows[0]["HideLucky3"]);

            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public void SetCacheList(DataTable result)
        {
            if (result != null)
            {
                _cache.Set(_cacheList, result);
            }
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult SearchListPost(
            string groupName = "ACCDDD",
            bool? enabled = null,
            bool? closed = null,
            bool? suspended = null,
            string username = null)
        {
            DataTable result;
            int srecommend;
            int mrecommend;
            int recommend;
            int custId;
            int roleId;
            GetDataResult(groupName, enabled, closed, suspended, username, out result, out srecommend, out mrecommend,
                out recommend, out custId, out roleId);

            SetEditAllBetVisibility(result);

            var ret = JsonConvert.SerializeObject(
                result == null ? new DataTable() : CheckVNIP(result),
                Formatting.None,
                new IsoDateTimeConverter {DateTimeFormat = "MM/dd/yyyy HH:mm:ss tt"});

            var syncCasino = true;
            var syncLiveCasino = true;
            var allowDoubleComm = true;
            if (roleId > 0 && roleId < 4)
            {
                DataTable userInfo = null;
                switch (roleId)
                {
                    case Master:
                        userInfo = _superService.GetData(CurrentSession.UserId, null, srecommend, null, null, null,
                            groupName);
                        break;

                    case Agent:
                        userInfo = _masterService.GetData(CurrentSession.UserId, srecommend, mrecommend);
                        break;

                    case Member:
                        userInfo = _agentService.GetData(CurrentSession.UserId, mrecommend, recommend);
                        break;
                }

                syncCasino = userInfo.Rows.Count != 0 && userInfo.Rows[0].ToBoolean("syncCasino");
                syncLiveCasino = userInfo.Rows.Count != 0 && userInfo.Rows[0].ToBoolean("syncLiveCasino");
                allowDoubleComm = userInfo.Rows.Count != 0
                                  && userInfo.Rows[0].ToBoolean(roleId > 2 ? "doubleCommLosing" : "doubleCommLosing1");
            }

            return JsonBase(
                new
                {
                    total = 1,
                    page = 1,
                    records = result?.Rows.Count ?? 0,
                    rows = ret,
                    roleId,
                    srecommend,
                    mrecommend,
                    recommend,
                    custId,
                    syncCasino,
                    syncLiveCasino,
                    allowDoubleComm
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult SuperListPost(
            string groupName = "ACCDDD",
            int? custId = null,
            bool? enabled = null,
            bool? closed = null,
            bool? suspended = null)
        {
            _cacheList = "CacheList" + groupName + custId + enabled + closed + suspended + Request["username"];
            var result = !IsBelong(Request["groupName"], Utils.ToInt32(custId), null, true)
                ? new DataTable()
                : _superService.GetData(CurrentSession.UserId, null, custId, enabled, closed, suspended, groupName);
            SetCacheList(result);

            SetEditAllBetVisibility(result);

            var ret = JsonConvert.SerializeObject(
                CheckVNIP(result), Formatting.None,
                new IsoDateTimeConverter {DateTimeFormat = "MM/dd/yyyy HH:mm:ss tt"});

            return JsonBase(new {total = 1, page = 1, records = result.Rows.Count, rows = ret});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult MasterListPost(int? srecommend = null, int? custId = null, bool? enabled = null,
            bool? closed = null, bool? suspended = null)
        {
            var result = !IsBelong(Request["groupName"], Utils.ToInt32(srecommend), null, true)
                ? new DataTable()
                : _masterService.GetData(CurrentSession.UserId, srecommend, custId, enabled, closed, suspended);

            var superId = Utils.ToInt32(srecommend);
            var customerInfo = new ReportService().GetCustomerInfo(Request["groupName"], superId, null, true);
            var mrecommend = 0;
            var
                recommend = 0;
            var
                roleId = 0;
            var hasSecurityCode = false;

            if (customerInfo != null)
            {
                roleId = Utils.ToInt32(customerInfo.Rows[0]["roleid"]);
                srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);
                custId = Utils.ToInt32(customerInfo.Rows[0]["custid"]);
                hasSecurityCode = Utils.ToBoolean(customerInfo.Rows[0]["HasSecurityCode"]);
            }

            _cacheList = "CacheList" + srecommend + custId + enabled + closed + suspended;
            SetCacheList(result);

            SetEditAllBetVisibility(result);

            var ret = JsonConvert.SerializeObject(
                CheckVNIP(result), Formatting.None,
                new IsoDateTimeConverter {DateTimeFormat = "MM/dd/yyyy HH:mm:ss tt"});

            return JsonBase(
                new
                {
                    total = 1,
                    page = 1,
                    records = result.Rows.Count,
                    rows = ret,
                    roleId,
                    srecommend,
                    mrecommend,
                    recommend,
                    custId,
                    hasSecurityCode
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult AgentListPost(int? mrecommend = null, int? custId = null, bool? enabled = null,
            bool? closed = null, bool? suspended = null)
        {
            var result = !IsBelong(Request["groupName"], Utils.ToInt32(mrecommend), null, true)
                ? new DataTable()
                : _agentService.GetData(CurrentSession.UserId, mrecommend, custId, enabled, closed, suspended);

            var masterId = Utils.ToInt32(mrecommend);
            var customerInfo = new ReportService().GetCustomerInfo(Request["groupName"], masterId, null, true);
            var srecommend = 0;
            var
                recommend = 0;
            var
                roleId = 0;
            var hasSecurityCode = false;

            if (customerInfo != null)
            {
                roleId = Utils.ToInt32(customerInfo.Rows[0]["roleid"]);
                srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);
                custId = Utils.ToInt32(customerInfo.Rows[0]["custid"]);
                hasSecurityCode = Utils.ToBoolean(customerInfo.Rows[0]["HasSecurityCode"]);
            }

            _cacheList = "CacheList" + mrecommend + custId + enabled + closed + suspended;
            SetCacheList(result);

            SetEditAllBetVisibility(result);

            var ret = JsonConvert.SerializeObject(
                CheckVNIP(result), Formatting.None,
                new IsoDateTimeConverter {DateTimeFormat = "MM/dd/yyyy HH:mm:ss tt"});

            return JsonBase(
                new
                {
                    total = 1,
                    page = 1,
                    records = result.Rows.Count,
                    rows = ret,
                    roleId,
                    srecommend,
                    mrecommend,
                    recommend,
                    custId,
                    hasSecurityCode
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult MemberListPost(int? recommend = null, int? custId = null, bool? enabled = null,
            bool? closed = null, bool? suspended = null)
        {
            var result = !IsBelong(Request["groupName"], Utils.ToInt32(recommend), null, true)
                ? new DataTable()
                : _memberService.GetData(CurrentSession.UserId, recommend, custId, enabled, closed, suspended);

            var agentId = Utils.ToInt32(recommend);
            var customerInfo = new ReportService().GetCustomerInfo(Request["groupName"], agentId, null, true);
            var mrecommend = 0;
            var
                srecommend = 0;
            var
                roleId = 0;
            if (customerInfo != null)
            {
                roleId = Utils.ToInt32(customerInfo.Rows[0]["roleid"]);
                srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);
                custId = Utils.ToInt32(customerInfo.Rows[0]["custid"]);
            }

            _cacheList = "CacheList" + recommend + custId + enabled + closed + suspended;
            SetCacheList(result);

            SetEditAllBetVisibility(result);

            var ret = JsonConvert.SerializeObject(
                CheckVNIP(result), Formatting.None,
                new IsoDateTimeConverter {DateTimeFormat = "MM/dd/yyyy HH:mm:ss tt"});

            return JsonBase(
                new
                {
                    total = 1,
                    page = 1,
                    records = result.Rows.Count,
                    rows = ret,
                    roleId,
                    srecommend,
                    mrecommend,
                    recommend,
                    custId
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult ViewIP(string ip)
        {
            if (IpUtils.IsVnIp(ip) && !IsAuthorized(CurrentSession.UserId, eFunctions.ViewVNIP,
                    AuthenticateAction.AuthenticationType.Read, Request))
            {
                return PartialViewBase("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            if (ip.Equals("::1", StringComparison.OrdinalIgnoreCase))
            {
                return PartialViewBase(new IPInfo
                {
                    City = string.Empty,
                    Country = string.Empty,
                    Flag = "-.gif",
                    Info = string.Empty,
                    IP = ip
                });
            }

            var info = IpUtils.LocationFromIP(ip);
            var ipinfo = new IPInfo
            {
                IP = ip,
                City = string.IsNullOrWhiteSpace(info.ipCity) ? string.Empty : info.ipCity,
                Country = string.IsNullOrWhiteSpace(info.countryLONG) ? string.Empty : info.countryLONG,
                Flag = string.IsNullOrWhiteSpace(info.countrySHORT) ? "-.gif" : info.countrySHORT + ".gif",
                Info = string.Format(ViewLang.IPNote, ip)
            };

            return PartialViewBase(ipinfo);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EnableDoubleComm,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult ConfigDoubleComm(int custid, bool allow)
        {
            if (!IsBelong(Request["groupName"], custid, null, true))
            {
                return ActionDenied(Request);
            }

            var result = _superService.ConfigDoubleComm(
                custid,
                allow,
                new Request
                {
                    Browser = Request.Browser.Browser,
                    CountryCode = CurrentContext.CountryCode,
                    DomainName = CurrentContext.DomainName,
                    IP = CurrentContext.Ip4Address,
                    Language = "en",
                    NetSessionId = CurrentSession.NetSessionId,
                    Platform = Request.Browser.Platform,
                    SubAccId = 0,
                    UserId = CurrentSession.UserId
                });

            return JsonBase(new {result.ErrorCode, result.ErrorMessage, result.IsSuccess});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult EditInfo(int custId)
        {
            if (!IsBelong(Request["groupName"], custId, null, true))
            {
                return ActionDenied(Request);
            }

            var model = _customerService.UserInfo(custId);
            model.FirstName = HttpUtility.HtmlDecode(model.FirstName);
            model.LastName = HttpUtility.HtmlDecode(model.LastName);
            model.HomePhone = HttpUtility.HtmlDecode(model.HomePhone);
            model.MobilePhone = HttpUtility.HtmlDecode(model.MobilePhone);
            model.Email = HttpUtility.HtmlDecode(model.Email);
            model.Fax = HttpUtility.HtmlDecode(model.Fax);
            model.Remark = HttpUtility.HtmlDecode(model.Remark);

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult EditInfoPost(
            int custId,
            string firstName,
            string lastName,
            string homePhone,
            string mobilePhone,
            string email,
            string fax,
            string remark)
        {
            if (!IsBelong(Request["groupName"], custId, null, true))
            {
                return ActionDenied(Request);
            }

            var result = _customerService.EditUserInfo(
                custId,
                firstName,
                lastName,
                homePhone,
                mobilePhone,
                email,
                fax,
                remark,
                CurrentContext.Ip4Address,
                CurrentSession.CustId,
                0,
                "E");

            return JsonBase(new {result.ErrorCode, result.ErrorMessage, result.IsSuccess});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EnableDoubleComm,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult ConfigMultipleDoubleComm(int id)
        {
            if (!IsBelong(Request["groupName"], Utils.ToInt32(Request["custId"]), null, true))
            {
                return ActionDenied(Request);
            }

            ViewBag.RoleId = id;
            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Transfer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult TransferCondition()
        {
            if (!IsBelong(Request["groupName"], Utils.ToInt32(Request["custId"]), null, true))
            {
                return ActionDenied(Request);
            }

            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Transfer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult EditTransferCondition(int custId)
        {
            if (!IsBelong(Request["groupName"], custId, null, true))
            {
                return ActionDenied(Request);
            }

            ViewBag.CustId = custId;
            var model = _masterService.GetTransferOption(custId);
            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Transfer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult EditTransferConditionPost(string transferOption, int custId)
        {
            if (!IsBelong(Request["groupName"], custId, null, true))
            {
                return ActionDenied(Request);
            }

            var result = _masterService.EditTransferOption(transferOption, custId, CurrentSession.CustId,
                CurrentContext.Ip4Address, 0, "E");

            return JsonBase(new {result.ErrorCode, result.ErrorMessage, result.IsSuccess});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult ResetPassword(int custId)
        {
            if (!IsBelong(Request["groupName"], custId, null, true))
            {
                return ActionDenied(Request);
            }

            var user = _customerService.UserInfo(custId);
            return PartialViewBase(user);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdatePassword(int custId, string password)
        {
            if (!IsBelong(Request["groupName"], custId, null, true))
            {
                return ActionDenied(Request);
            }

            var result = _customerService.ResetPassword(
                custId,
                password,
                new Request
                {
                    Browser = Request.Browser.Browser,
                    CountryCode = CurrentContext.CountryCode,
                    DomainName = CurrentContext.DomainName,
                    IP = CurrentContext.Ip4Address,
                    Language = "en",
                    NetSessionId = CurrentSession.NetSessionId,
                    Platform = Request.Browser.Platform,
                    SubAccId = 0,
                    UserId = CurrentSession.UserId
                });

            if (result.ErrorCode.Equals("0"))
            {
                var casino = CasinoManager.Instance.GetCustInfoIBC(custId, 0);
                if (casino.CasinoSync)
                {
                    var cust = new CasinoLibrary.SBAgent.Customer();
                    try
                    {
                        cust.UpdateCustPwd(custId, Encryption.CfsEnCode(password), CurrentContext.Ip4Address,
                            CurrentSession.UserId, CurrentSession.UserName, 3);
                    }
                    catch (ErrorException casinoEx)
                    {
                        Logs.CasinoError(casinoEx);
                    }
                    catch (Exception ex)
                    {
                        Logs.Error(ex);
                    }
                }
            }

            return JsonBase(new {result.ErrorCode, result.ErrorMessage, result.IsSuccess});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public void GetExportToExcel(
            int? currentLevel = null,
            string groupName = "ACCDDD",
            int? srecommend = null,
            int? mrecommend = null,
            int? recommend = null,
            int? custId = null,
            bool? enabled = null,
            bool? closed = null,
            bool? suspended = null)
        {
            var fileName = "CustomerList";

            SetCache(currentLevel, ref fileName, groupName, srecommend, mrecommend, recommend, custId, enabled, closed,
                suspended);

            var status = string.Empty;
            if (enabled != null || closed != null || suspended != null)
            {
                status = enabled == false
                    ? "Disabled"
                    : closed == true
                        ? "Closed"
                        : suspended == true
                            ? "Suspended"
                            : "Open";
            }

            var result = _cache.Get<DataTable>(_cacheList);
            var builder = new CustomerListPageBuilder(
                CheckVNIP(result),
                false,
                status,
                Convert.ToInt32(currentLevel),
                result?.Rows.Count > 0
                && (result.Rows[0].ToBoolean("ismyanmarodds") || currentLevel == 4));
            var excelStream = builder.Build();
            if (excelStream != null)
            {
                ExportToExcel(excelStream, status + fileName + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                excelStream.Dispose();
            }
        }

        private void SetCache(
            int? currentLevel,
            ref string fileName,
            string groupName = "ACCDDD",
            int? srecommend = null,
            int? mrecommend = null,
            int? recommend = null,
            int? custId = null,
            bool? enabled = null,
            bool? closed = null,
            bool? suspended = null)
        {
            var username = Request["username"];
            var id = string.IsNullOrWhiteSpace(username) ? null : custId;
            switch (currentLevel)
            {
                case 4:
                    fileName = "SuperList";
                    _cacheList = "CacheList" + groupName + id + enabled + closed + suspended + username;
                    if (_cache.Get<DataTable>(_cacheList) == null)
                    {
                        SetCacheList(!IsBelong(groupName, Utils.ToInt32(custId), null, true)
                            ? new DataTable()
                            : _superService.GetData(CurrentSession.UserId, null, id, enabled, closed, suspended,
                                groupName));
                    }

                    break;

                case 3:
                    fileName = "MasterList";
                    _cacheList = "CacheList" + groupName + srecommend + id + enabled + closed + suspended + username;
                    if (_cache.Get<DataTable>(_cacheList) == null)
                    {
                        SetCacheList(!IsBelong(groupName, Utils.ToInt32(srecommend), null, true)
                            ? new DataTable()
                            : _masterService.GetData(CurrentSession.UserId, srecommend, id, enabled, closed,
                                suspended));
                    }

                    break;

                case 2:
                    fileName = "AgentList";
                    _cacheList = "CacheList" + groupName + mrecommend + id + enabled + closed + suspended + username;
                    if (_cache.Get<DataTable>(_cacheList) == null)
                    {
                        SetCacheList(!IsBelong(groupName, Utils.ToInt32(mrecommend), null, true)
                            ? new DataTable()
                            : _agentService.GetData(CurrentSession.UserId, mrecommend, id, enabled, closed, suspended));
                    }

                    break;

                default:
                    fileName = "MemberList";
                    _cacheList = "CacheList" + groupName + recommend + id + enabled + closed + suspended + username;
                    if (_cache.Get<DataTable>(_cacheList) == null)
                    {
                        SetCacheList(!IsBelong(groupName, Utils.ToInt32(recommend), null, true)
                            ? new DataTable()
                            : _memberService.GetData(CurrentSession.UserId, recommend, id, enabled, closed, suspended));
                    }

                    break;
            }
        }

        private void GetDataResult(
            string groupName,
            bool? enabled,
            bool? closed,
            bool? suspended,
            string username,
            out DataTable result,
            out int srecommend,
            out int mrecommend,
            out int recommend,
            out int custId,
            out int roleId)
        {
            var customerInfo = new ReportService().GetCustomerInfo(groupName, 0, username, true);
            result = null;
            srecommend = 0;
            mrecommend = 0;
            recommend = 0;
            custId = 0;
            roleId = 0;
            int? id = null;
            if (!string.IsNullOrWhiteSpace(Request["roleId"]) && string.IsNullOrWhiteSpace(username))
            {
                roleId = Utils.ToInt32(Request["roleId"]);
                srecommend = Utils.ToInt32(Request["srecommend"]);
                mrecommend = Utils.ToInt32(Request["mrecommend"]);
                recommend = Utils.ToInt32(Request["recommend"]);
            }
            else
            {
                if (customerInfo != null)
                {
                    roleId = Utils.ToInt32(customerInfo.Rows[0]["roleid"]);
                    srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                    mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                    recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);
                    custId = Utils.ToInt32(customerInfo.Rows[0]["custid"]);
                }
            }

            if (customerInfo != null || !string.IsNullOrWhiteSpace(Request["roleId"]))
            {
                switch (roleId)
                {
                    case 4:
                        result = !IsBelong(groupName, custId, null, true)
                            ? null
                            : _superService.GetData(CurrentSession.UserId, null, custId == 0 ? id : custId, enabled,
                                closed, suspended, groupName);
                        _cacheList = "CacheList" + groupName + custId + enabled + closed + suspended + username;
                        break;

                    case 3:
                        result = !IsBelong(groupName, srecommend, null, true)
                            ? null
                            : _masterService.GetData(CurrentSession.UserId, srecommend, custId == 0 ? id : custId,
                                enabled, closed, suspended);
                        _cacheList = "CacheList" + groupName + srecommend + custId + enabled + closed + suspended +
                                     username;
                        break;

                    case 2:
                        result = !IsBelong(groupName, mrecommend, null, true)
                            ? null
                            : _agentService.GetData(CurrentSession.UserId, mrecommend, custId == 0 ? id : custId,
                                enabled, closed, suspended);
                        _cacheList = "CacheList" + groupName + mrecommend + custId + enabled + closed + suspended +
                                     username;
                        break;

                    default:
                        result = !IsBelong(groupName, recommend, null, true)
                            ? null
                            : _memberService.GetData(CurrentSession.UserId, recommend, custId == 0 ? id : custId,
                                enabled, closed, suspended);
                        _cacheList = "CacheList" + groupName + recommend + custId + enabled + closed + suspended +
                                     username;
                        break;
                }
            }

            SetCacheList(result);
        }

        private void SetEditAllBetVisibility(DataTable dataTable)
        {
            dataTable.AsEnumerable().ToList().ForEach(n =>
                n["DisableAllBet"] = n.Field<bool>("DisableAllBet") && n.Field<bool>("DisableAllBetVIP"));
        }
    }
}