namespace Fanex.ExternalAccount.Controllers.Customer.Sportsbook
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using CustomerSetting.Module.Core.Criteria;
    using CustomerSetting.Module.Core.Services.Impl;
    using CustomerSetting.Security.Authenticate;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using PTEngine.BusinessObjects;
    using PTEngine.Services;
    using Service.Impl.Customer;
    using Service.Interface.Customer;
    using Utilities;
    using CustomerService = PTEngine.Services.CustomerService;
    using Result = DTO.Result;

    public class SportsbookSettingController : BaseController
    {
        private const int NG2Start = 213;
        private const int NG2End = 220;

        private readonly ICommissionService _commissionService = new CommissionService();

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult EditBetSetting() =>
            !IsBelong(GetGroupname(), Utils.ToInt32(Request["custId"]), null, true)
                ? ActionDenied(Request)
                : PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult EditBetSettingMasterGet(int custId, int parentId)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var bettingLimitService = new BettingLimitService();

            var currentContext = new RequestContext
            {
                FormId = FormType.EditPT,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture),
                CustId = parentId,
                TargetCustId = custId,
                RoleId = (int) CustomerRole.Super,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = SiteName,
                Language = "En",
                LangKey = "E"
            };

            IBettingLimit<BettingLimitItem> bettingLimit = null;

            try
            {
                bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return JsonBase(new {betting = bettingLimit, context = currentContext}, settings);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult EditBetSettingAgentGet(int custId, int parentId)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var bettingLimitService = new BettingLimitService();

            var currentContext = new RequestContext
            {
                FormId = FormType.EditPT,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture),
                CustId = parentId,
                TargetCustId = custId,
                RoleId = (int)CustomerRole.Master,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = SiteName,
                Language = "En",
                LangKey = "E"
            };

            IBettingLimit<BettingLimitItem> bettingLimit = null;

            try
            {
                bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return JsonBase(new {betting = bettingLimit, context = currentContext}, settings);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult EditBetSettingMemberGet(int custId, int parentId)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var bettingLimitService = new BettingLimitService();

            IRequestContext currentContext = new RequestContext
            {
                FormId = FormType.EditPT,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture),
                CustId = parentId,
                TargetCustId = custId,
                RoleId = (int)CustomerRole.Agent,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = SiteName,
                Language = "En",
                LangKey = "E"
            };

            IBettingLimit<BettingLimitItem> bettingLimit = null;

            try
            {
                bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return JsonBase(new {betting = bettingLimit, context = currentContext}, settings);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult EditBetSettingPost(int custId, int parentId, string txtCustomerObject)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};

            var bettingLimit = JsonConvert.DeserializeObject<BettingLimit>(txtCustomerObject, settings);

            var requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                TargetCustId = custId,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            IBettingLimitService bettingLimitService = new BettingLimitService();

            var result = bettingLimitService.UpdateBetting(bettingLimit, requestContext);

            return JsonBase(new {result.ErrorCode, result.ErrorMessage});
        }

        private string GetGroupname()
        {
            return Request["groupName"];
        }

        public ActionResult ActionDenied() => new ViewResult
        {
            ViewName = "~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml"
        };

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult EditCommission() =>
            !IsBelong(GetGroupname(), Utils.ToInt32(Request["custId"]), null, true)
                ? ActionDenied(Request)
                : PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult EditCommissionDetail() =>
            !IsBelong(GetGroupname(), Utils.ToInt32(Request["custId"]), null, true)
                ? ActionDenied(Request)
                : PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult EditCommissionMember() =>
            !IsBelong(GetGroupname(), Utils.ToInt32(Request["custId"]), null, true)
                ? ActionDenied(Request)
                : PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult EditCommissionMemberDetail() =>
            !IsBelong(GetGroupname(), Utils.ToInt32(Request["custId"]), null, true)
                ? ActionDenied(Request)
                : PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult EditCommissionGet(int custId, int parentId)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var currentContext = new RequestContext
            {
                FormId = FormType.EditPT,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture),
                CustId = parentId,
                TargetCustId = custId,
                RoleId = Utils.ToInt32(Request["roleId"]),
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = SiteName,
                Language = "En",
                LangKey = "E"
            };

            ICommissionSetting commissionSetting = null;

            try
            {
                commissionSetting = _commissionService.GetCommission(currentContext);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return JsonBase(new {commission = commissionSetting, context = currentContext}, settings);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult EditCommissionPost(int custId, int parentId, int currentLevel, string txtCustomerObject)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};

            var commissionSetting = JsonConvert.DeserializeObject<CommissionSetting>(txtCustomerObject, settings);

            var requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                TargetCustId = custId,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            Result result = null;

            switch (currentLevel)
            {
                case (int) CustomerRole.Master:
                    result = _commissionService.UpdateCommissionMaster(commissionSetting, requestContext);
                    break;

                case (int) CustomerRole.Agent:
                    result = _commissionService.UpdateCommissionAgent(commissionSetting, requestContext);
                    break;

                case (int) CustomerRole.Member:
                    result = _commissionService.UpdateCommissionMember(commissionSetting, requestContext);
                    break;
            }

            return JsonBase(new {result.ErrorCode, result.ErrorMessage});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult EditPT() => !IsBelong(GetGroupname(), Utils.ToInt32(Request["custId"]), null, true)
            ? ActionDenied(Request)
            : PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult SuperMasterEditSB() =>
            !IsBelong(GetGroupname(), Utils.ToInt32(Request["custId"]), null, true)
                ? ActionDenied(Request)
                : PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult EditPTSuperGet(int custId, int parentId)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var setting = new CustomerService();
            var context = new RequestContext
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                TargetRoleId = (int) CustomerRole.Super,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            try
            {
                customer = setting.GetCustomerForCompanyEditSuper(context);
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

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult EditPTMasterGet(int custId, int parentId)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var setting = new CustomerService();
            var context = new RequestContext
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int) CustomerRole.Super,
                TargetRoleId = (int) CustomerRole.Master,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            try
            {
                customer = setting.GetCustomerForSuperEditMaster(context);
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

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult SuperUpdateMasterPTPost(int custId, int parentId, string txtCustomerObject,
            bool isUpdateAllMember, decimal newPositionTaking)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var mult = Request["isMult"];
            var para = Request["para"];
            var param = new Hashtable();
            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            if (!string.IsNullOrWhiteSpace(para))
            {
                for (var i = 0; i < para.Length; i++)
                {
                    param.Add(i, para[i].ToString());
                }
            }

            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);

            var requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                RoleId = (int) CustomerRole.Super,
                TargetRoleId = (int) CustomerRole.Master,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                SystemId = (byte) Config.SystemId
            };

            var custSrv = new CustomerService();
            var result = !string.IsNullOrWhiteSpace(mult) && mult.Equals("true")
                ? custSrv.SuperUpdateMultipleMasters(customer, requestContext, isUpdateAllMember, newPositionTaking,
                    param)
                : custSrv.SuperUpdateMaster(customer, requestContext, isUpdateAllMember, newPositionTaking);

            if (param.Count == 0 || (param.Count > 0 && param[0].ToString().Equals("1")))
            {
                UpdateCasinoStatus(custId, result, customer, param.Count > 0 && param[0].ToString().Equals("1"));
            }

            return JsonBase(new {result.ErrorCode, result.ErrorMessage});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult EditPTAgentGet(int custId, int parentId)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var customerService = new CustomerService();
            var context = new RequestContext
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int) CustomerRole.Master,
                TargetRoleId = (int) CustomerRole.Agent,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            try
            {
                customer = customerService.GetCustomerForMasterEditAgent(context);
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

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult MasterUpdateAgentPTPost(int custId, int parentId, string txtCustomerObject,
            bool isUpdateAllMember, decimal newPositionTaking)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var mult = Request["isMult"];
            var para = Request["para"];
            var param = new Hashtable();
            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            if (!string.IsNullOrWhiteSpace(para))
            {
                for (var i = 0; i < para.Length; i++)
                {
                    param.Add(i, para[i].ToString());
                }
            }

            var customer =
                JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            IRequestContext requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                RoleId = (int) CustomerRole.Master,
                TargetRoleId = (int) CustomerRole.Agent,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                SystemId = (byte) Config.SystemId
            };

            var custSrv = new CustomerService();
            var result = !string.IsNullOrWhiteSpace(mult) && mult.Equals("true")
                ? custSrv.MasterUpdateMultipleAgents(customer, requestContext, isUpdateAllMember, newPositionTaking,
                    param)
                : custSrv.MasterUpdateAgent(customer, requestContext, isUpdateAllMember, newPositionTaking);

            if (param.Count == 0 || (param.Count > 0 && param[0].ToString().Equals("1")))
            {
                UpdateCasinoStatus(custId, result, customer, param.Count > 0 && param[0].ToString().Equals("1"));
            }

            return JsonBase(new {result.ErrorCode, result.ErrorMessage});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult EditPTMemberGet(int custId, int parentId)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var setting = new CustomerService();
            var context = new RequestContext
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int) CustomerRole.Agent,
                TargetRoleId = (int) CustomerRole.Member,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            try
            {
                customer = setting.GetCustomerForAgentUpdateMember(context);
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

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult AgentUpdateMemberPTPost(int custId, int parentId, string txtCustomerObject)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var mult = Request["isMult"];
            var para = Request["para"];
            var param = new Hashtable();
            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            if (!string.IsNullOrWhiteSpace(para))
            {
                for (var i = 0; i < para.Length; i++)
                {
                    param.Add(i, para[i].ToString());
                }
            }

            var customer =
                JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);

            var requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                RoleId = (int) CustomerRole.Agent,
                TargetRoleId = (int) CustomerRole.Member,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                SystemId = (byte) Config.SystemId
            };

            var custSrv = new CustomerService();
            var result = !string.IsNullOrWhiteSpace(mult) && mult.Equals("true")
                ? custSrv.AgentUpdateMultipleMembers(customer, requestContext, param)
                : custSrv.AgentUpdateMember(customer, requestContext);

            if (param.Count == 0 || (param.Count > 0 && param[0].ToString().Equals("1")))
            {
                UpdateCasinoStatus(custId, result, customer, param.Count > 0 && param[0].ToString().Equals("1"));
            }

            return JsonBase(new {result.ErrorCode, result.ErrorMessage});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult SuperEditPTMemberGet(int custId, int parentId, int agentId)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var setting = new CustomerService();
            var context = new RequestContext
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                AgentId = agentId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int) CustomerRole.Super,
                TargetRoleId = (int) CustomerRole.Member,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            try
            {
                customer = setting.GetCustomerForSuperEditMember(context);
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

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult SuperEditPTMemberPost(int custId, int parentId, string txtCustomerObject)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var para = Request["para"];
            var param = new Hashtable();
            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            var customer =
                JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            IRequestContext requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                RoleId = (int) CustomerRole.Super,
                TargetRoleId = (int) CustomerRole.Member,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                SystemId = (byte) Config.SystemId
            };

            if (!string.IsNullOrWhiteSpace(para))
            {
                for (var i = 0; i < para.Length; i++)
                {
                    param.Add(i, para[i].ToString());
                }
            }

            var custSrv = new CustomerService();
            var result = param.Count > 0
                ? custSrv.SuperUpdateMultipleMembers(customer, requestContext, param)
                : custSrv.SuperUpdateMember(customer, requestContext);

            if (param.Count == 0 || (param.Count > 0 && param[0].ToString().Equals("1")))
            {
                UpdateCasinoStatus(custId, result, customer, param.Count > 0 && param[0].ToString().Equals("1"));
            }

            return JsonBase(new {result.ErrorCode, result.ErrorMessage});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult MasterEditPTMemberGet(int custId, int parentId, int agentId)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var setting = new CustomerService();
            var context = new RequestContext
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                AgentId = agentId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int) CustomerRole.Master,
                TargetRoleId = (int) CustomerRole.Member,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            try
            {
                customer = setting.GetCustomerForMasterEditMember(context);
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

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult MasterEditPTMemberPost(int custId, int parentId, string txtCustomerObject)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var para = Request["para"];
            var param = new Hashtable();
            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            var customer =
                JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            var requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                RoleId = (int) CustomerRole.Master,
                TargetRoleId = (int) CustomerRole.Member,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                SystemId = (byte) Config.SystemId
            };

            if (!string.IsNullOrWhiteSpace(para))
            {
                for (var i = 0; i < para.Length; i++)
                {
                    param.Add(i, para[i].ToString());
                }
            }

            var custSrv = new CustomerService();
            var result = param.Count > 0
                ? custSrv.MasterUpdateMultipleMembers(customer, requestContext, param)
                : custSrv.MasterUpdateMember(customer, requestContext);

            if (param.Count == 0 || (param.Count > 0 && param[0].ToString().Equals("1")))
            {
                UpdateCasinoStatus(custId, result, customer, param.Count > 0 && param[0].ToString().Equals("1"));
            }

            return JsonBase(new {result.ErrorCode, result.ErrorMessage});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult EditMultiplePT() =>
            !IsBelong(GetGroupname(), Utils.ToInt32(Request["custId"]), null, true)
                ? ActionDenied(Request)
                : PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult SuperMasterEditMultiplePTMember() =>
            !IsBelong(GetGroupname(), Utils.ToInt32(Request["custId"]), null, true)
                ? ActionDenied(Request)
                : PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult EditMinPT() => !IsBelong(GetGroupname(), Utils.ToInt32(Request["custId"]), null, true)
            ? ActionDenied(Request)
            : PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult SuperEditMinPTMasterGet(int custId, int parentId)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var context = new RequestContext
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditMinPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int) CustomerRole.Super,
                TargetRoleId = (int) CustomerRole.Master,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            var customerService = new CustomerService();

            try
            {
                customer = customerService.GetCustomerForSuperEditMinPTMaster(context);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            customer.BettingLimit = null;
            customer.StatusSetting = null;
            customer.CommissionSetting = null;
            customer.CommissionSetting2 = null;

            return JsonBase(
                new {context, customer},
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult MasterEditMinPTAgentGet(int custId, int parentId)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var context = new RequestContext
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditMinPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int) CustomerRole.Master,
                TargetRoleId = (int) CustomerRole.Agent,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            var customerService = new CustomerService();

            try
            {
                customer = customerService.GetCustomerForMasterEditMinPTAgent(context);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            customer.BettingLimit = null;
            customer.StatusSetting = null;
            customer.CommissionSetting = null;
            customer.CommissionSetting2 = null;

            return JsonBase(
                new {context, customer},
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult SuperEditMinPTMasterPost(int custId, int parentId, string txtCustomerObject)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};

            var customer =
                JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            var requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                RoleId = (int) CustomerRole.Super,
                TargetRoleId = (int) CustomerRole.Master,
                FormId = FormType.EditMinPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            var result = new CustomerService().SuperUpdateMinPTMaster(customer, requestContext);

            return JsonBase(new {result.ErrorCode, result.ErrorMessage});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult MasterEditMinPTAgentPost(int custId, int parentId, string txtCustomerObject)
        {
            if (!IsBelong(GetGroupname(), custId, null, true))
            {
                return ActionDenied(Request);
            }

            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};

            var customer =
                JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            var requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                RoleId = (int) CustomerRole.Master,
                TargetRoleId = (int) CustomerRole.Agent,
                FormId = FormType.EditMinPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            var result = new CustomerService().MasterUpdateMinPTAgent(customer, requestContext);

            return JsonBase(new {result.ErrorCode, result.ErrorMessage});
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult ViewPtInfo()
        {
            if (!IsSpecialSiteCustomer())
            {
                IList<int> exclusiveRefSportIds = new List<int>(8);

                for (var i = NG2Start; i <= NG2End; i++)
                {
                    exclusiveRefSportIds.Add(i);
                }

                ViewData["ExclusiveRefSportIds"] = exclusiveRefSportIds.ToArray();
            }

            return View("~/Views/BettypeReference/Index.cshtml");
        }

        private bool IsSpecialSiteCustomer()
        {
            var hasNG2Permission = IsAuthorized(CurrentSession.UserId, eFunctions.NG2Setting,
                AuthenticateAction.AuthenticationType.Read, Request);
            if (!hasNG2Permission)
            {
                return false;
            }

            var permisisonService = new ServiceBase();
            var checkCustomerIsInternalAdminResult = permisisonService.CheckCustomerIsInternalAdmin(
                new CheckCustomerIsInternalAdminCriteria
                {
                    CustID = CurrentSession.CustId
                });

            if (checkCustomerIsInternalAdminResult.IsInternalAdmin)
            {
                if (!checkCustomerIsInternalAdminResult.IsAllowAccess)
                {
                    return false;
                }

                return permisisonService.CheckCustomerHasSpecialSitePermission(
                    new CheckCustomerHasSpecialSiteFunctionPermissionCriteria
                    {
                        CustID = CurrentSession.CustId
                    });
            }

            return true;
        }
    }
}