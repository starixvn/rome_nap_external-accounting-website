namespace Fanex.ExternalAccount.Controllers.Customer
{
    using CasinoLibrary.Model.UserException;
    using Core.Authorization;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DataAccess.Criteria.Customer;
    using DataAccess.Criteria.Customer.CustomerStatus;
    using Fanex.Module.NG2.Entities;
    using IBCUtils;
    using Logging.Extensions.Web;
    using Newtonsoft.Json;
    using PTEngine.BusinessObjects;
    using Service;
    using Service.Impl.Customer;
    using Service.Impl.Customer.Casino;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using Utilities;
    using ViewModel;
    using ViewModel.Customer;
    using ViewModel.Customer.Suspend;
    using AuthenticationType = CustomerSetting.Security.Authenticate.AuthenticateAction.AuthenticationType;
    using Customer = PTEngine.BusinessObjects.Customer;

    public partial class CustomerController : CustomerBase //DevSkim: ignore DS184626
    {
        private readonly AgentService _agentService = new AgentService();
        private readonly CustomerService _customerService = new CustomerService();
        private readonly PTEngine.Services.ICustomerService ptCustomerService = new PTEngine.Services.CustomerService();
        private readonly MasterService _masterService = new MasterService();
        private readonly SuperService _superService = new SuperService();
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

            var limitNewDownlineResult = _customerService.CheckLimitNewDownline(custId, Role.Super);

            if (!limitNewDownlineResult.IsSuccess)
            {
                return LimitationPage(limitNewDownlineResult.ErrorMessage);
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
            var defaultMinBetValue = GetDefaultMinBetValue();

            return JsonBase(new { context, customer, defaultMinBetValue });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult SuperAddNewMasterPost(int parentId, string txtCustomerObject)
        {
            var limitNewDownlineResult = _customerService.CheckLimitNewDownline(parentId, Role.Super);

            if (!limitNewDownlineResult.IsSuccess)
            {
                return JsonBase(limitNewDownlineResult);
            }

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
                SystemId = (byte)Config.SystemId,
                SettingStatus = Config.SettingStatus
            };

            var result = ptCustomerService.AddNewMaster(customer, requestContext, out int masterId);
            if (string.IsNullOrWhiteSpace(result.ErrorCode) || result.ErrorCode == "0")
            {
#pragma warning disable S109 // Magic numbers should not be used
                Thread.Sleep(3000);
#pragma warning restore S109 // Magic numbers should not be used
                AutoInsertNG2Product(masterId, customer.UserName, RoleId.Master, parentId, SiteId.NAP,
                    customer.CurrencyId, Config.SettingStatus);
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

            var limitNewDownlineResult = _customerService.CheckLimitNewDownline(custId, Role.Master);

            if (!limitNewDownlineResult.IsSuccess)
            {
                return LimitationPage(limitNewDownlineResult.ErrorMessage);
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
            var defaultMinBetValue = GetDefaultMinBetValue();

            return JsonBase(new { context, customer, defaultMinBetValue });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult MasterAddNewAgentPost(int parentId, string txtCustomerObject)
        {
            var limitNewDownlineResult = _customerService.CheckLimitNewDownline(parentId, Role.Master);

            if (!limitNewDownlineResult.IsSuccess)
            {
                return JsonBase(limitNewDownlineResult);
            }

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
                SystemId = (byte)Config.SystemId,
                SettingStatus = Config.SettingStatus
            };

            var result = ptCustomerService.AddNewAgent(customer, requestContext, out int agentId);

            if (string.IsNullOrWhiteSpace(result.ErrorCode) || result.ErrorCode == "0")
            {
#pragma warning disable S109 // Magic numbers should not be used
                Thread.Sleep(3000);
#pragma warning restore S109 // Magic numbers should not be used
                AutoInsertNG2Product(agentId, customer.UserName, RoleId.Agent, parentId, SiteId.NAP,
                    customer.CurrencyId, Config.SettingStatus);

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

            var limitNewDownlineResult = _customerService.CheckLimitNewDownline(custId, Role.Agent);

            if (!limitNewDownlineResult.IsSuccess)
            {
                return LimitationPage(limitNewDownlineResult.ErrorMessage);
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
            var defaultMinBetValue = GetDefaultMinBetValue();

            return JsonBase(new { context, customer, defaultMinBetValue });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult AgentAddNewMemberPost(int parentId, string txtCustomerObject)
        {
            var limitNewDownlineResult = _customerService.CheckLimitNewDownline(parentId, Role.Agent);

            if (!limitNewDownlineResult.IsSuccess)
            {
                return JsonBase(limitNewDownlineResult);
            }

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
                SystemId = (byte)Config.SystemId,
                SettingStatus = Config.SettingStatus
            };

            var result = ptCustomerService.AddNewMember(customer, requestContext, out int memberId);

            if (string.IsNullOrWhiteSpace(result.ErrorCode) || result.ErrorCode == "0")
            {
#pragma warning disable S109 // Magic numbers should not be used
                Thread.Sleep(3000);
#pragma warning restore S109 // Magic numbers should not be used
                AutoInsertNG2Product(memberId, customer.UserName, RoleId.Member, parentId, SiteId.NAP,
                    customer.CurrencyId, Config.SettingStatus);

                AutoInsertCasinoPost(memberId, parentId);
            }

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, CustId = memberId });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult CustomerList() => PartialViewBase();

        public ActionResult Settings(int id, int custId, int uplineId, string userName, string groupName)
        {
            var custStatus = _customerService.GetCustomerAllStatus(custId, Config.SettingStatus);
            ViewBag.CurrentLevel = id;
            ViewBag.UserId = custId;
            ViewBag.UplineId = uplineId;
            ViewBag.SyncCasino = custStatus.SyncedCasino;
            ViewBag.HideVirtualSports = custStatus.HideVirtualSport;
            ViewBag.HideKeno = custStatus.HideKeno;
            ViewBag.HideGoldDeluxe = custStatus.HideGoldDeluxe;
            var visibledProducts = GetVisibledProducts(id, custId);

            return PartialViewBase(visibledProducts);
        }

        private List<Product> GetVisibledProducts(int roleId, int custId)
        {
            var visibledProducts = new List<Product>();
            var productStatusList = productStatusService.GetProductStatusList(custId, roleId, Config.SettingStatus);

            foreach (var product in ProductSettings.SiteProducts.Values)
            {
                var productStatus = productStatusList.FirstOrDefault(p => p.ProductId == product.Id);
                if (productStatus != null
                    && !productStatus.IsHidden
                    && productStatus.IsSyncedUpline
                    && ProductSettings.IsShowProduct(product.Id)
                    && product.Id != ProductConstants.SabaVirtualSports)
                {
                    visibledProducts.Add(product);
                }
            }

            return visibledProducts;
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
            var model = _customerService.UserInfo(custId, Config.SettingStatus);
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
        public ActionResult TransferCondition()
        {
            var custId = int.Parse(Request["CustId"]);
            var isSuperInAutomateTransfer = _masterService.GetSuperInAutomateTransfer(custId).Any();

            return PartialViewBase(isSuperInAutomateTransfer);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
        public ActionResult EditTransferCondition(int custId, int roleId)
        {
            ViewBag.CustId = custId;
            var model = _masterService.GetTransferOption(custId);
            var isSuperInAutomateTransfer = _masterService.GetSuperInAutomateTransfer(custId).Any();

            return PartialViewBase(new TransferConditionViewModel(model, isSuperInAutomateTransfer, roleId));
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
            var user = _customerService.UserInfo(custId, Config.SettingStatus);
            return PartialViewBase(user);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdatePassword(int custId, string password, byte roleId)
        {
            var rawPassword = Encryptor.Decrypt(password, Encryptor.RandomCaptcha);
            var result = _customerService.ResetPassword(
                new ResetPasswordCriteria(
                    custId,
                    rawPassword,
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
                        cust.UpdateCustPwd(custId, Encryption.CfsEnCode(rawPassword), CurrentContext.Ip4Address,
                            CurrentSession.UserId, CurrentSession.UserName, CurrentSession.FromSystem);
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

                try
                {
                    _customerService.KickoutCustomer(custId, roleId, CurrentContext.Ip4Address);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex);
                }
            }

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }

        public ActionResult AccessDenied() => ActionDenied(Request);

        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult UnsuspendSingle(int custId, string userName)
        {
            return PartialViewBase(_customerService.GetSingleUpdateSuspendViewModel(custId, userName));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult MultipleSuspend(string custIdList, int roleId)
        {
            return PartialViewBase(new MultipleUpdateViewModel(roleId, _customerService.GetMultipleSuspendCustomerViewModel(new GetMultipleSuspendCustomerInfoCriteria(custIdList, roleId))));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public JsonResult GetSuspendDownlinesDetail(int custId, bool isAffected, int downlineRoleId, int pageSize = 50, int pageIndex = 0)
        {
            return Json(_customerService.GetSuspendDownlinesDetail(new GetSuspendCustomerDetailInfoCriteria(custId, isAffected, downlineRoleId, pageSize, pageIndex)));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        public JsonResult UpdateSuspendSingle(int custId, bool suspended)
        {
            return Json(_customerService.SuspendCustomer(new UpdateSuspendCustomerCriteria(CurrentSession.UserId, custId, suspended, "E", CurrentContext.Ip4Address)));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        public JsonResult UpdateSuspendMultiple(
            List<MultipleUpdateSuspendRequest> request,
            List<int> custIds,
            string groupName,
            int roleId)
        {
            if (!AuthorizationService.IsBelong(groupName, custIds))
            {
                return Json(new DTO.Result(1, "You are not authorized on this function!"));
            }

            return Json(_customerService.SuspendMultipleCustomer(request, CurrentSession.UserId, CurrentContext.Ip4Address, roleId));
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