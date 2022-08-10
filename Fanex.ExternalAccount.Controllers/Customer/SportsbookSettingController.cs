namespace Fanex.ExternalAccount.Controllers.Customer.Sportsbook
{
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Module.Core.Criteria;
    using CustomerSetting.Module.Core.Services.Impl;
    using CustomerSetting.Security.Authenticate;
    using Fanex.CustomerSetting.Module.Core.Constants;
    using Fanex.CustomerSetting.Module.Core.Services.Interface;
    using Fanex.Logging.Extensions.Web;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using PTEngine.BusinessObjects;
    using PTEngine.Services;
    using Service.Impl.Customer;
    using Service.Interface.Customer;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Mvc;
    using Utilities;
    using AuthenticationType = CustomerSetting.Security.Authenticate.AuthenticateAction.AuthenticationType;
    using CustomerService = PTEngine.Services.CustomerService;

    [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Read)]
    public class SportsbookSettingController : BaseController
    {
        private const int NG2Start = 213;
        private const int NG2End = 220;

        private readonly ICommissionService commissionService = new CommissionService();
        private readonly IBettingLimitService bettingLimitService = new BettingLimitService();
        private readonly PTEngine.Services.ICustomerService customerService = new CustomerService();
        private readonly ISuperService superService = new SuperService();
        private readonly IServiceBase serviceBase = new ServiceBase();

        public ActionResult EditBetSetting() => PartialViewBase();

        [AntiXSRFFilter]
        public ActionResult EditBetSettingMasterGet(int custId, int parentId)
        {
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
                RoleId = (int)CustomerRole.Super,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = SiteName,
                Language = "En",
                LangKey = "E"
            };

            var bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);
            var defaultMinBetValue = GetDefaultMinBetValue();

            return JsonBase(new { betting = bettingLimit, context = currentContext, defaultMinBetValue }, settings);
        }

        [AntiXSRFFilter]
        public ActionResult EditBetSettingAgentGet(int custId, int parentId)
        {
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

            var bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);
            var defaultMinBetValue = GetDefaultMinBetValue();

            return JsonBase(new
            {
                betting = bettingLimit,
                context = currentContext,
                defaultMinBetValue
            });
        }

        [AntiXSRFFilter]
        public ActionResult EditBetSettingMemberGet(int custId, int parentId)
        {
            var currentContext = new RequestContext
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

            var bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);
            var defaultMinBetValue = GetDefaultMinBetValue();

            return JsonBase(new { betting = bettingLimit, context = currentContext, defaultMinBetValue });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult EditBetSettingPost(int custId, int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

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

            var result = bettingLimitService.UpdateBetting(bettingLimit, requestContext);

            return JsonBase(new { result.ErrorCode, result.ErrorMessage });
        }

        public ActionResult EditCommission() => PartialViewBase();

        public ActionResult EditCommissionDetail() => PartialViewBase();

        public ActionResult EditCommissionMember() => PartialViewBase();

        public ActionResult EditCommissionMemberDetail() => PartialViewBase();

        [AntiXSRFFilter]
        public ActionResult EditCommissionGet(int custId, int parentId)
        {
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
                commissionSetting = commissionService.GetCommission(currentContext);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return JsonBase(new { commission = commissionSetting, context = currentContext });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult EditCommissionPost(int custId, int parentId, int currentLevel, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

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

            PTEngine.BusinessObjects.Result result;
            var checkUmResult = serviceBase.CheckUM(requestContext.TargetCustId, NonDcsProduct.Sportsbook, requestContext.LangKey);
            if (!checkUmResult.IsSuccess)
            {
                result = new PTEngine.BusinessObjects.Result(checkUmResult.ErrorCode.ToString(), checkUmResult.ErrorMessage);
                return JsonBase(new { result.ErrorCode, result.ErrorMessage });
            }

            DTO.Result updateResult;

            switch (currentLevel)
            {
                case Role.Master:
                    updateResult = commissionService.UpdateCommissionMaster(commissionSetting, requestContext);
                    break;

                case (int)CustomerRole.Agent:
                    updateResult = commissionService.UpdateCommissionAgent(commissionSetting, requestContext);
                    break;

                default:
                    updateResult = commissionService.UpdateCommissionMember(commissionSetting, requestContext);
                    break;
            }

            return JsonBase(new { updateResult.ErrorCode, updateResult.ErrorMessage });
        }

        public ActionResult EditPT() => PartialViewBase();

        public ActionResult SuperMasterEditSB() => PartialViewBase();

        [AntiXSRFFilter]
        public ActionResult EditPTSuperGet(int custId, int parentId)
        {
            var context = new RequestContext
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                TargetRoleId = (int)CustomerRole.Super,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = null;
            try
            {
                customer = superService.GetCustomerForCompanyEditSuper(context);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return JsonBase(new { context, customer });
        }

        [AntiXSRFFilter]
        public ActionResult EditPTMasterGet(int custId, int parentId)
        {
            var context = new RequestContext
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Master,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = null;
            try
            {
                customer = customerService.GetCustomerForSuperEditMaster(context);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            var defaultMinBetValue = GetDefaultMinBetValue();

            return JsonBase(new { context, customer, defaultMinBetValue });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult SuperUpdateMasterPTPost(int custId, int parentId, string txtCustomerObject,
            bool isUpdateAllMember, decimal newPositionTaking)
        {
            var param = GetParamFromRequest();
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);

            var requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Master,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                SystemId = (byte)Config.SystemId
            };

            var mult = Request["isMult"];
            var result = !string.IsNullOrWhiteSpace(mult) && mult.Equals("true")
                ? customerService.SuperUpdateMultipleMasters(customer, requestContext, isUpdateAllMember, newPositionTaking,
                    param)
                : customerService.SuperUpdateMaster(customer, requestContext, isUpdateAllMember, newPositionTaking);

            return JsonBase(new { result.ErrorCode, result.ErrorMessage });
        }

        [AntiXSRFFilter]
        public ActionResult EditPTAgentGet(int custId, int parentId)
        {
            var context = new RequestContext
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Agent,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = null;
            try
            {
                customer = customerService.GetCustomerForMasterEditAgent(context);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            var defaultMinBetValue = GetDefaultMinBetValue();

            return JsonBase(new { context, customer, defaultMinBetValue });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult MasterUpdateAgentPTPost(int custId, int parentId, string txtCustomerObject,
            bool isUpdateAllMember, decimal newPositionTaking)
        {
            var param = GetParamFromRequest();
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            var requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Agent,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                SystemId = (byte)Config.SystemId
            };

            var mult = Request["isMult"];
            var result = !string.IsNullOrWhiteSpace(mult) && mult.Equals("true")
                ? customerService.MasterUpdateMultipleAgents(customer, requestContext, isUpdateAllMember, newPositionTaking,
                    param)
                : customerService.MasterUpdateAgent(customer, requestContext, isUpdateAllMember, newPositionTaking);

            return JsonBase(new { result.ErrorCode, result.ErrorMessage });
        }

        [AntiXSRFFilter]
        public ActionResult EditPTMemberGet(int custId, int parentId)
        {
            var context = new RequestContext
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int)CustomerRole.Agent,
                TargetRoleId = (int)CustomerRole.Member,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = null;
            try
            {
                customer = customerService.GetCustomerForAgentUpdateMember(context);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            var defaultMinBetValue = GetDefaultMinBetValue();

            return JsonBase(new { context, customer, defaultMinBetValue });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult AgentUpdateMemberPTPost(int custId, int parentId, string txtCustomerObject)
        {
            var param = GetParamFromRequest();
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);

            var requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                RoleId = (int)CustomerRole.Agent,
                TargetRoleId = (int)CustomerRole.Member,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                SystemId = (byte)Config.SystemId
            };

            var mult = Request["isMult"];
            var result = !string.IsNullOrWhiteSpace(mult) && mult.Equals("true")
                ? customerService.AgentUpdateMultipleMembers(customer, requestContext, param)
                : customerService.AgentUpdateMember(customer, requestContext);

            return JsonBase(new { result.ErrorCode, result.ErrorMessage });
        }

        [AntiXSRFFilter]
        public ActionResult SuperEditPTMemberGet(int custId, int parentId, int agentId)
        {
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
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Member,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = null;
            try
            {
                customer = customerService.GetCustomerForSuperEditMember(context);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return JsonBase(new { context, customer });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult SuperEditPTMemberPost(int custId, int parentId, string txtCustomerObject)
        {
            var para = Request["para"];
            var param = new Hashtable();
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var customer =
                JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            var requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Member,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                SystemId = (byte)Config.SystemId
            };

            if (!string.IsNullOrWhiteSpace(para))
            {
                for (var i = 0; i < para.Length; i++)
                {
                    param.Add(i, para[i].ToString());
                }
            }

            var result = param.Count > 0
                ? customerService.SuperUpdateMultipleMembers(customer, requestContext, param)
                : customerService.SuperUpdateMember(customer, requestContext);

            return JsonBase(new { result.ErrorCode, result.ErrorMessage });
        }

        [AntiXSRFFilter]
        public ActionResult MasterEditPTMemberGet(int custId, int parentId, int agentId)
        {
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
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Member,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = null;
            try
            {
                customer = customerService.GetCustomerForMasterEditMember(context);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return JsonBase(new { context, customer });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult MasterEditPTMemberPost(int custId, int parentId, string txtCustomerObject)
        {
            var para = Request["para"];
            var param = new Hashtable();
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var customer =
                JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            var requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Member,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                SystemId = (byte)Config.SystemId
            };

            if (!string.IsNullOrWhiteSpace(para))
            {
                for (var i = 0; i < para.Length; i++)
                {
                    param.Add(i, para[i].ToString());
                }
            }

            var result = param.Count > 0
                ? customerService.MasterUpdateMultipleMembers(customer, requestContext, param)
                : customerService.MasterUpdateMember(customer, requestContext);

            return JsonBase(new { result.ErrorCode, result.ErrorMessage });
        }

        public ActionResult EditMultiplePT() => PartialViewBase();

        public ActionResult SuperMasterEditMultiplePTMember() => PartialViewBase();

        public ActionResult EditMinPT() => PartialViewBase();

        [AntiXSRFFilter]
        public ActionResult SuperEditMinPTMasterGet(int custId, int parentId)
        {
            var context = new RequestContext
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditMinPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Master,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = null;
            try
            {
                customer = customerService.GetCustomerForSuperEditMinPTMaster(context);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            if (customer != null)
            {
                customer.BettingLimit = null;
                customer.StatusSetting = null;
                customer.CommissionSetting = null;
                customer.CommissionSetting2 = null;
            }

            return JsonBase(new { context, customer });
        }

        [AntiXSRFFilter]
        public ActionResult MasterEditMinPTAgentGet(int custId, int parentId)
        {
            var context = new RequestContext
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditMinPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Agent,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper(CultureInfo.InvariantCulture)
            };

            ICustomer customer = null;
            try
            {
                customer = customerService.GetCustomerForMasterEditMinPTAgent(context);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            if (customer != null)
            {
                customer.BettingLimit = null;
                customer.StatusSetting = null;
                customer.CommissionSetting = null;
                customer.CommissionSetting2 = null;
            }

            return JsonBase(new { context, customer });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult SuperEditMinPTMasterPost(int custId, int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            var requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Master,
                FormId = FormType.EditMinPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            var result = customerService.SuperUpdateMinPTMaster(customer, requestContext);

            return JsonBase(new { result.ErrorCode, result.ErrorMessage });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult MasterEditMinPTAgentPost(int custId, int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            var requestContext = new RequestContext
            {
                CustId = CurrentSession.UserId,
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Agent,
                FormId = FormType.EditMinPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            var result = customerService.MasterUpdateMinPTAgent(customer, requestContext);

            return JsonBase(new { result.ErrorCode, result.ErrorMessage });
        }

        public ActionResult ViewPtInfo()
        {
            if (!IsSpecialSiteCustomer())
            {
                var exclusiveRefSportIds = new List<int>(8);

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
            var hasNG2Permission = AuthorizationService.IsAuthorized(CurrentSession.UserId, eFunctions.NG2Setting);
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

        private Hashtable GetParamFromRequest()
        {
            var para = Request["para"];
            var param = new Hashtable();
            if (!string.IsNullOrWhiteSpace(para))
            {
                for (var i = 0; i < para.Length; i++)
                {
                    param.Add(i, para[i].ToString());
                }
            }

            return param;
        }
    }
}