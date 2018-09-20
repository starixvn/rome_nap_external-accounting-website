namespace Fanex.ExternalAccount.Controllers.Customer.Sportsbook
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
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
    using AuthenticationType = CustomerSetting.Security.Authenticate.AuthenticateAction.AuthenticationType;

    [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Read)]
    public class SportsbookSettingController : BaseController
    {
        private const int NG2Start = 213;
        private const int NG2End = 220;

        private readonly ICommissionService commissionService = new CommissionService();
        private readonly IBettingLimitService bettingLimitService = new BettingLimitService();
        private readonly PTEngine.Services.ICustomerService customerService = new CustomerService();
        private readonly ISuperService superService = new SuperService();

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

            return JsonBase(new { betting = bettingLimit, context = currentContext }, settings);
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

            return JsonBase(new
            {
                betting = bettingLimit,
                context = currentContext
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

            return JsonBase(new { betting = bettingLimit, context = currentContext });
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

            var commissionSetting = commissionService.GetCommission(currentContext);

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

            Result result;

            switch (currentLevel)
            {
                case Role.Master:
                    result = commissionService.UpdateCommissionMaster(commissionSetting, requestContext);
                    break;

                case (int)CustomerRole.Agent:
                    result = commissionService.UpdateCommissionAgent(commissionSetting, requestContext);
                    break;

                default:
                    result = commissionService.UpdateCommissionMember(commissionSetting, requestContext);
                    break;
            }

            return JsonBase(new { result.ErrorCode, result.ErrorMessage });
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
            
            var customer = superService.GetCustomerForCompanyEditSuper(context);

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

            var customer = customerService.GetCustomerForSuperEditMaster(context);

            return JsonBase(new { context, customer });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult SuperUpdateMasterPTPost(int custId, int parentId, string txtCustomerObject,
            bool isUpdateAllMember, decimal newPositionTaking)
        {
            var mult = Request["isMult"];
            var para = Request["para"];
            var param = new Hashtable();
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
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
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Master,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                SystemId = (byte)Config.SystemId
            };

            var result = !string.IsNullOrWhiteSpace(mult) && mult.Equals("true")
                ? customerService.SuperUpdateMultipleMasters(customer, requestContext, isUpdateAllMember, newPositionTaking,
                    param)
                : customerService.SuperUpdateMaster(customer, requestContext, isUpdateAllMember, newPositionTaking);

            if (param.Count == 0 || (param.Count > 0 && param[0].ToString().Equals("1")))
            {
                UpdateCasinoStatus(custId, result, customer, param.Count > 0 && param[0].ToString().Equals("1"));
            }

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

            var customer = customerService.GetCustomerForMasterEditAgent(context);

            return JsonBase(new { context, customer });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult MasterUpdateAgentPTPost(int custId, int parentId, string txtCustomerObject,
            bool isUpdateAllMember, decimal newPositionTaking)
        {
            var mult = Request["isMult"];
            var para = Request["para"];
            var param = new Hashtable();
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
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
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Agent,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                SystemId = (byte)Config.SystemId
            };

            var result = !string.IsNullOrWhiteSpace(mult) && mult.Equals("true")
                ? customerService.MasterUpdateMultipleAgents(customer, requestContext, isUpdateAllMember, newPositionTaking,
                    param)
                : customerService.MasterUpdateAgent(customer, requestContext, isUpdateAllMember, newPositionTaking);

            if (param.Count == 0 || (param.Count > 0 && param[0].ToString().Equals("1")))
            {
                UpdateCasinoStatus(custId, result, customer, param.Count > 0 && param[0].ToString().Equals("1"));
            }

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

            var customer = customerService.GetCustomerForAgentUpdateMember(context);

            return JsonBase(new { context, customer });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult AgentUpdateMemberPTPost(int custId, int parentId, string txtCustomerObject)
        {
            var mult = Request["isMult"];
            var para = Request["para"];
            var param = new Hashtable();
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
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
                RoleId = (int)CustomerRole.Agent,
                TargetRoleId = (int)CustomerRole.Member,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                SystemId = (byte)Config.SystemId
            };

            var result = !string.IsNullOrWhiteSpace(mult) && mult.Equals("true")
                ? customerService.AgentUpdateMultipleMembers(customer, requestContext, param)
                : customerService.AgentUpdateMember(customer, requestContext);

            if (param.Count == 0 || (param.Count > 0 && param[0].ToString().Equals("1")))
            {
                UpdateCasinoStatus(custId, result, customer, param.Count > 0 && param[0].ToString().Equals("1"));
            }

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

            var customer = customerService.GetCustomerForSuperEditMember(context);

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

            if (param.Count == 0 || (param.Count > 0 && param[0].ToString().Equals("1")))
            {
                UpdateCasinoStatus(custId, result, customer, param.Count > 0 && param[0].ToString().Equals("1"));
            }

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

            var customer = customerService.GetCustomerForMasterEditMember(context);

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

            if (param.Count == 0 || (param.Count > 0 && param[0].ToString().Equals("1")))
            {
                UpdateCasinoStatus(custId, result, customer, param.Count > 0 && param[0].ToString().Equals("1"));
            }

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

            var customer = customerService.GetCustomerForSuperEditMinPTMaster(context);

            customer.BettingLimit = null;
            customer.StatusSetting = null;
            customer.CommissionSetting = null;
            customer.CommissionSetting2 = null;

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

            var customer = customerService.GetCustomerForMasterEditMinPTAgent(context);

            customer.BettingLimit = null;
            customer.StatusSetting = null;
            customer.CommissionSetting = null;
            customer.CommissionSetting2 = null;

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
    }
}