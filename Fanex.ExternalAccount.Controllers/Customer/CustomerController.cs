namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using CasinoLibrary.Model.UserException;
    using Common;
    using Core.Authorization;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DataAccess.Criteria.Customer;
    using DTO;
    using DTO.Customer;
    using Excel.Customer.CustomerListPage;
    using Fanex.ExternalAccount.Module.LeftPanel;
    using Fanex.Module.NG2.Entities;
    using IBCUtils;
    using Logging.Extensions.Web;
    using Module.LeftPanel.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using PTEngine.BusinessObjects;
    using Service;
    using Service.Impl.Customer;
    using Service.Impl.Customer.Casino;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Utilities;
    using ViewModel.Customer;
    using AuthenticationType = CustomerSetting.Security.Authenticate.AuthenticateAction.AuthenticationType;
    using Customer = PTEngine.BusinessObjects.Customer;
    using CustomerDTO = Fanex.ExternalAccount.DTO.Customer.Customer;

    public class CustomerController : CustomerBase //DevSkim: ignore DS184626
    {
        private const string DateTimeFormat = "MM/dd/yyyy HH:mm:ss tt";
        private readonly AgentService _agentService = new AgentService();
        private readonly CustomerService _customerService = new CustomerService();
        private readonly PTEngine.Services.ICustomerService ptCustomerService = new PTEngine.Services.CustomerService();
        private readonly MasterService _masterService = new MasterService();
        private readonly MemberService _memberService = new MemberService();
        private readonly SuperService _superService = new SuperService();
        private readonly IReportService reportService = new ReportService();
        private readonly IIPInformatonService ipInformationService = new IPInformationService();

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        public ActionResult IsValidUsername(string username)
        {
            var isValidUser = _customerService.IsValidUsername(username.Trim(), CurrentSession.CustId);

            return Json(isValidUser);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
        public ActionResult SuperAddNewMaster(int custId, string username)
        {
            var reachLimitationResult = CheckReachLimitationResult(custId, Role.Super, username);

            if (!reachLimitationResult.IsSuccess)
            {
                return LimitationPage(reachLimitationResult.ErrorMessage);
            }

            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
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
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Master,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = parentUsername.ToUpper(CultureInfo.InvariantCulture)
            };

            var customer = ptCustomerService.GetCustomerForAddNewMaster(context);
            customer.CurrencyId = parentCurrencyId;

            return JsonBase(new { context, customer });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult SuperAddNewMasterPost(int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            var requestContext = new RequestContext
            {
                CustId = parentId,
                TargetCustId = 0,
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Master,
                FormId = FormType.Add,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                SystemId = (byte)Config.SystemId
            };

            int masterId;
            var result = ptCustomerService.AddNewMaster(customer, requestContext, out masterId);

            if (string.IsNullOrWhiteSpace(result.ErrorCode) || result.ErrorCode == "0")
            {
                UpdateCasinoStatus(masterId, result, customer);
                Thread.Sleep(3000);
                AutoInsertNG2Product(masterId, customer.UserName, RoleId.Master, parentId, SiteId.NAP,
                    customer.CurrencyId);
                AutoInsertCasinoPost(masterId, parentId);
            }

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, CustId = masterId });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
        public ActionResult MasterAddNewAgent(int custId, string username)
        {
            var reachLimitationResult = CheckReachLimitationResult(custId, Role.Master, username);

            if (!reachLimitationResult.IsSuccess)
            {
                return LimitationPage(reachLimitationResult.ErrorMessage);
            }

            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
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
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Agent,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = parentUsername.ToUpper(CultureInfo.InvariantCulture)
            };

            var customer = ptCustomerService.GetCustomerForAddNewAgent(context);
            customer.CurrencyId = parentCurrencyId;

            return JsonBase(new { context, customer });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult MasterAddNewAgentPost(int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            var requestContext = new RequestContext
            {
                CustId = parentId,
                TargetCustId = 0,
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Agent,
                FormId = FormType.Add,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                SystemId = (byte)Config.SystemId
            };

            int agentId;
            var result = ptCustomerService.AddNewAgent(customer, requestContext, out agentId);

            if (string.IsNullOrWhiteSpace(result.ErrorCode) || result.ErrorCode == "0")
            {
                UpdateCasinoStatus(agentId, result, customer);
                Thread.Sleep(3000);
                AutoInsertNG2Product(agentId, customer.UserName, RoleId.Agent, parentId, SiteId.NAP,
                    customer.CurrencyId);

                AutoInsertCasinoPost(agentId, parentId);
            }

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, CustId = agentId });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
        public ActionResult AgentAddNewMember(int custId, string username)
        {
            var reachLimitationResult = CheckReachLimitationResult(custId, Role.Agent, username);
            if (!reachLimitationResult.IsSuccess)
            {
                return LimitationPage(reachLimitationResult.ErrorMessage);
            }

            var limitedMemberBySuperResult = CheckMemberLimitedBySuperResult(custId);

            if (!limitedMemberBySuperResult.IsSuccess)
            {
                return LimitationPage(limitedMemberBySuperResult.ErrorMessage);
            }
            
            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
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
                RoleId = (int)CustomerRole.Agent,
                TargetRoleId = (int)CustomerRole.Member,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = parentUsername.ToUpper(CultureInfo.InvariantCulture)
            };

            var customer = ptCustomerService.GetCustomerForAddNewMember(context);
            customer.CurrencyId = parentCurrencyId;

            return JsonBase(new { context, customer });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult AgentAddNewMemberPost(int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            var requestContext = new RequestContext
            {
                CustId = parentId,
                TargetCustId = 0,
                RoleId = (int)CustomerRole.Agent,
                TargetRoleId = (int)CustomerRole.Member,
                FormId = FormType.Add,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                SystemId = (byte)Config.SystemId
            };

            int memberId;
            var result = ptCustomerService.AddNewMember(customer, requestContext, out memberId);

            if (string.IsNullOrWhiteSpace(result.ErrorCode) || result.ErrorCode == "0")
            {
                UpdateCasinoStatus(memberId, result, customer);
                Thread.Sleep(3000);
                AutoInsertNG2Product(memberId, customer.UserName, RoleId.Member, parentId, SiteId.NAP,
                    customer.CurrencyId);

                AutoInsertCasinoPost(memberId, parentId);
            }

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, CustId = memberId });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult CustomerList() => PartialViewBase();

        public ActionResult Settings(int id, int custId, int uplineId, string userName, string groupName)
        {
            var customerSetting = GetLineInfo(ExtRequest.GetGroupName(groupName), userName);
            var result = GetCustomerList(ExtRequest.GetGroupName(groupName), null, null, null, customerSetting);

            ViewBag.CurrentLevel = id;
            ViewBag.UserId = custId;
            ViewBag.UplineId = uplineId;
            ViewBag.SyncCasino = (bool)result.Rows[0]["SyncCasino"];
            ViewBag.HideVirtualSports = (bool)result.Rows[0]["HideVirtualSports"];
            ViewBag.HideKeno = (bool)result.Rows[0]["HideKeno"] || !ProductSettings.ShowKenoSetting;
            ViewBag.HideGoldDeluxe = (bool)result.Rows[0]["HideGoldDeluxe"] || !ProductSettings.IsShowGoldDeluxe;
            ViewBag.HideColossusBets = (bool)result.Rows[0]["HideColossusBet"] || !ProductSettings.IsShowColossusBets;
            ViewBag.HideCricket = Converter.ToBool(result.Rows[0]["HideCricket"]) || !ProductSettings.IsShowCricket;
            var visibledProducts = GetVisibledProducts(id, custId);

            return PartialViewBase(visibledProducts);
        }

        private List<Product> GetVisibledProducts(int roleId, int custId)
        {
            var visibledProducts = new List<Product>();
            var productStatusList = productStatusService.GetProductStatusList(custId, roleId);

            foreach (var product in ProductSettings.SiteProducts.Values)
            {
                var productStatus = productStatusList.FirstOrDefault(p => p.ProductId == product.Id);
                if (productStatus != null
                    && !productStatus.IsHidden
                    && productStatus.IsSyncedUpline
                    && ProductSettings.IsShowProduct(product.Id))
                {
                    visibledProducts.Add(product);
                }
            }

            return visibledProducts;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult SearchListPost(
            string groupName,
            bool? enabled = null,
            bool? closed = null,
            bool? suspended = null,
            string username = null,
            bool? withDataLimited = null)
        {
            var lineInfo = GetLineInfo(ExtRequest.GetGroupName(groupName), username, BuildCustomerSetting(username));
            var customers = GetCustomerList(ExtRequest.GetGroupName(groupName), enabled, closed, suspended, lineInfo, withDataLimited);
            var customerProductStatus = TransformToCustomerProductStatusList(customers);
            var customers2String = Customers2Json(customers);

            var hasCustomerData = customers.Rows.Count != 0;
            var syncCasino = hasCustomerData && customers.Rows[0].ToBoolean("syncCasino");
            var allowDoubleComm = hasCustomerData && customers.Rows[0].ToBoolean(lineInfo.RoleId == Role.Super ? "doubleCommLosing" : "doubleCommLosingUpline");

            return JsonBase(
                new
                {
                    records = customers?.Rows.Count ?? 0,
                    rows = customers2String,
                    lineInfo.RoleId,
                    srecommend = lineInfo.SuperId,
                    mrecommend = lineInfo.MasterId,
                    recommend = lineInfo.AgentId,
                    lineInfo.CustId,
                    syncCasino,
                    allowDoubleComm,
                    customerProductStatus
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult SuperListPost(
            string groupName,
            int? custId = null,
            bool? enabled = null,
            bool? closed = null,
            bool? suspended = null,
            bool? withDataLimited = null)
        {
            var supers = _superService.GetData(CurrentSession.UserId, ExtRequest.GetGroupName(groupName), custId, enabled, closed, suspended, withDataLimited);
            var customerProductStatus = TransformToCustomerProductStatusList(supers);
            var supers2Json = Customers2Json(supers);

            return JsonBase(new { records = supers.Rows.Count, rows = supers2Json, customerProductStatus });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult MasterListPost(int? srecommend = null, int? custId = null, bool? enabled = null,
            bool? closed = null, bool? suspended = null)
        {
            var masters = _masterService.GetData(CurrentSession.UserId, srecommend, custId, enabled, closed, suspended);
            var superId = Utils.ToInt32(srecommend);
            var customerSetting = reportService.SearchCustomerInfo(ExtRequest.GetGroupName(), superId, null, isGetUplineInfo: true);
            var customerProductStatus = TransformToCustomerProductStatusList(masters);
            var master2Json = Customers2Json(masters);

            return JsonBase(
                new
                {
                    records = masters.Rows.Count,
                    rows = master2Json,
                    customerSetting.RoleId,
                    srecommend = customerSetting.SuperId,
                    mrecommend = customerSetting.MasterId,
                    recommend = customerSetting.AgentId,
                    customerSetting.CustId,
                    customerSetting.HasSecurityCode,
                    customerProductStatus
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult AgentListPost(int? mrecommend = null, int? custId = null, bool? enabled = null,
            bool? closed = null, bool? suspended = null)
        {
            var agents = _agentService.GetData(CurrentSession.UserId, mrecommend, custId, enabled, closed, suspended);

            var masterId = Utils.ToInt32(mrecommend);
            var customerSetting = reportService.SearchCustomerInfo(ExtRequest.GetGroupName(), masterId, null, isGetUplineInfo: true);
            var customerProductStatus = TransformToCustomerProductStatusList(agents);
            var ret = Customers2Json(agents);

            return JsonBase(
                new
                {
                    records = agents.Rows.Count,
                    rows = ret,
                    customerSetting.RoleId,
                    SuperRecommend = customerSetting.SuperId,
                    MasterRecommend = customerSetting.MasterId,
                    Recommend = customerSetting.AgentId,
                    customerSetting.CustId,
                    customerSetting.HasSecurityCode,
                    customerProductStatus
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult MemberListPost(string groupName, int? recommend = null, int? custId = null, bool? enabled = null,
            bool? closed = null, bool? suspended = null)
        {
            var members = _memberService.GetData(CurrentSession.UserId, ExtRequest.GetGroupName(groupName), recommend, custId, enabled, closed, suspended);
            var agentId = Utils.ToInt32(recommend);
            var customerSetting = reportService.SearchCustomerInfo(ExtRequest.GetGroupName(groupName), agentId, null, isGetUplineInfo: true);
            var customerProductStatus = TransformToCustomerProductStatusList(members);
            var member2Json = Customers2Json(members);

            return JsonBase(
                new
                {
                    records = members.Rows.Count,
                    rows = member2Json,
                    customerSetting.RoleId,
                    srecommend = customerSetting.SuperId,
                    mrecommend = customerSetting.MasterId,
                    recommend = customerSetting.AgentId,
                    customerSetting.CustId,
                    customerProductStatus
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        public ActionResult ViewIP(string ip)
        {
            var ipInfo = new IPInfo { Flag = "-.gif", IP = ip };
            if (Utils.IsValidIpAddress(ip))
            {
                if (!AuthorizationService.CanViewVNIP(ip))
                {
                    return PartialViewBase("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
                }

                var info = ipInformationService.GetFromIP(ip);
                ipInfo = new IPInfo
                {
                    IP = ip,
                    City = string.IsNullOrWhiteSpace(info.IPCity) ? string.Empty : info.IPCity,
                    Country = string.IsNullOrWhiteSpace(info.CountryLong) ? string.Empty : info.CountryLong,
                    Flag = string.IsNullOrWhiteSpace(info.CountryShort) ? "-.gif" : info.CountryShort + ".gif",
                    Info = string.Format(ViewLang.IPNote, ip)
                };
            }

            return PartialViewBase(ipInfo);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EnableDoubleComm, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult ConfigDoubleComm(int custid, bool allow)
        {
            var result = _superService.ConfigDoubleComm(
                new UpdateDoubleCommCriteria(
                    custid,
                    allow,
                    CurrentSession.UserId,
                    CurrentContext.Ip4Address));

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Read)]
        public ActionResult EditInfo(int custId)
        {
            var model = _customerService.UserInfo(custId);
            model.FirstName = HttpUtility.HtmlDecode(model.FirstName);
            model.LastName = HttpUtility.HtmlDecode(model.LastName);
            model.Phone = HttpUtility.HtmlDecode(model.Phone);
            model.MobilePhone = HttpUtility.HtmlDecode(model.MobilePhone);
            model.Email = HttpUtility.HtmlDecode(model.Email);
            model.Fax = HttpUtility.HtmlDecode(model.Fax);
            model.Remark = HttpUtility.HtmlDecode(model.Remark);

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult EditInfoPost(DTO.Customer.Customer customer)
        {
            var result = _customerService.EditUserInfo(customer, CurrentContext.Ip4Address, CurrentSession.CustId, 0, "E");

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EnableDoubleComm, Policy = AuthenticationType.Read)]
        public ActionResult ConfigMultipleDoubleComm(int id) => PartialViewBase(id);

        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
        public ActionResult TransferCondition() => PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
        public ActionResult EditTransferCondition(int custId)
        {
            ViewBag.CustId = custId;
            var model = _masterService.GetTransferOption(custId);
            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult EditTransferConditionPost(string transferOption, int custId)
        {
            var result = _masterService.EditTransferOption(transferOption, custId, CurrentSession.CustId,
                CurrentContext.Ip4Address, 0, "E");

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Read)]
        public ActionResult ResetPassword(int custId)
        {
            var user = _customerService.UserInfo(custId);
            return PartialViewBase(user);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdatePassword(int custId, string password)
        {
            var result = _customerService.ResetPassword(
                new ResetPasswordCriteria(
                    custId,
                    password,
                    CurrentSession.UserId,
                    CurrentContext.Ip4Address)
                );

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
                        ExceptionHandler.Handle(new Exception(casinoEx.Message, casinoEx), TraceEventType.Information);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.Handle(ex);
                    }
                }
            }

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        public void GetExportToExcel(
            string groupName,
            bool? enabled = null,
            bool? closed = null,
            bool? suspended = null,
            string username = null,
            bool? withDataLimited = null)
        {
            var customerSetting = BuildCustomerSetting(username);

            var customers = GetCustomerList(ExtRequest.GetGroupName(groupName), enabled, closed, suspended, customerSetting, withDataLimited);
            TransformToCustomerProductStatusList(customers);

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

            var viewSpecialIPPermission = AuthorizationService.IsAuthorized(CurrentSession.UserId, eFunctions.ViewVNIP);
            var newCustomers = ipInformationService.HideSpecialIP(customers, viewSpecialIPPermission);

            var builder = new CustomerListPageBuilder(
                newCustomers,
                false,
                status,
                Convert.ToInt32(customerSetting.RoleId),
                customers?.Rows.Count > 0
                && (customers.Rows[0].ToBoolean("ismyanmarodds") || customerSetting.RoleId.IsSuper()),
                withDataLimited.HasValue && withDataLimited.Value);
            var excelStream = builder.Build();
            var fileName = $"{status}{customerSetting.RoleId.DisplayRoleName()}List{DateTime.Now.ToString("yyyyMMdd")}.xls";

            ExportToExcel(excelStream, fileName);
        }

        public ActionResult AccessDenied() => ActionDenied(Request);

        private DataTable GetCustomerList(
            string groupName,
            bool? enabled,
            bool? closed,
            bool? suspended,
            LineInfo lineInfo,
            bool? withDataLimited = null)
        {
            int? id = null;
            DataTable result;

            switch (lineInfo.RoleId)
            {
                case Role.Super:
                    result = _superService.GetData(CurrentSession.UserId, groupName, lineInfo.CustId == 0
                        ? id
                        : lineInfo.CustId, enabled, closed, suspended, withDataLimited);
                    break;

                case Role.Master:
                    result = _masterService.GetData(CurrentSession.UserId, lineInfo.SuperId, lineInfo.CustId == 0
                        ? id
                        : lineInfo.CustId, enabled, closed, suspended);
                    break;

                case Role.Agent:
                    result = _agentService.GetData(CurrentSession.UserId, lineInfo.MasterId, lineInfo.CustId == 0
                        ? id
                        : lineInfo.CustId, enabled, closed, suspended);
                    break;

                default:
                    result = _memberService.GetData(CurrentSession.UserId, groupName, lineInfo.AgentId, lineInfo.CustId == 0
                        ? id
                        : lineInfo.CustId, enabled, closed, suspended);
                    break;
            }

            return result;
        }

        private LineInfo BuildCustomerSetting(string username)
            => new LineInfo
            {
                RoleId = Utils.ToInt32(Request["roleId"]),
                SuperId = Utils.ToInt32(Request["srecommend"]),
                MasterId = Utils.ToInt32(Request["mrecommend"]),
                AgentId = Utils.ToInt32(Request["recommend"]),
                CustId = string.IsNullOrWhiteSpace(username) ? 0 : Utils.ToInt32(Request["custId"])
            };

        private LineInfo GetLineInfo(string groupName, string userName, LineInfo defaultValue = null)
        {
            var customerInfo = reportService.GetCustomerInfo(groupName, 0, userName, isGetUplineInfo: true);
            if (customerInfo != null)
            {
                return new LineInfo
                {
                    RoleId = Utils.ToInt32(customerInfo.Rows[0]["roleid"]),
                    SuperId = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]),
                    MasterId = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]),
                    AgentId = Utils.ToInt32(customerInfo.Rows[0]["recommend"]),
                    CustId = Utils.ToInt32(customerInfo.Rows[0]["custid"])
                };
            }

            return string.IsNullOrWhiteSpace(userName) ? defaultValue ?? new LineInfo() : new LineInfo();
        }

        private static IDictionary<int, IDictionary<int, ProductStatus>> TransformToCustomerProductStatusList(DataTable dataTable)
        {
            int custId;
            IDictionary<int, ProductStatus> customerProductStatus;
            var customerProductStatusList = new Dictionary<int, IDictionary<int, ProductStatus>>();

            var customerList = dataTable.AsEnumerable().ToList();

            foreach (var customerRow in customerList)
            {
                customerRow["DisableAllBet"] = customerRow.Field<bool>("DisableAllBet");
                custId = customerRow.Field<int>("custid");
                customerProductStatus = new Dictionary<int, ProductStatus>();

                foreach (var productStatus in AccountInfoProductConfiguration.TransformToProductStatus(customerRow, true))
                {
                    customerProductStatus.Add(productStatus.ProductId, productStatus);
                }

                customerProductStatusList.Add(custId, customerProductStatus);
            }

            return customerProductStatusList;
        }

        private string Customers2Json(DataTable customers)
        {
            var viewSpecialIPPermission = AuthorizationService.IsAuthorized(CurrentSession.UserId, eFunctions.ViewVNIP);
            var newCustomers = ipInformationService.HideSpecialIP(customers, viewSpecialIPPermission);

            return JsonConvert.SerializeObject(
                newCustomers,
                Formatting.None,
                new IsoDateTimeConverter { DateTimeFormat = DateTimeFormat });
        }

        private ActionResult LimitationPage(string message)
            => PartialView("~/Views/Customer/LimitMemberCreation.cshtml", message);

        private DTO.Result CheckReachLimitationResult(int custId, int roleId, string username)
            => _customerService.CheckNewCustomerIsReachLimitation(new CheckNewCustomerLimitationCriteria
            {
                CustId = custId,
                RoleId = roleId,
                Username = username
            });

        private DTO.Result CheckMemberLimitedBySuperResult(int custId)
            => _agentService.IsLimitMemberCreationBySuper(new CheckMemberCreationCriteria
            {
                AgentId = custId
            });
    }
}