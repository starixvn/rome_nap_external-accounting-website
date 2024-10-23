namespace Fanex.ExternalAccount.Controllers.Customer.Sportsbook
{
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using Fanex.Logging.Extensions.Web;
    using Newtonsoft.Json;
    using PTEngine.BusinessObjects;
    using Service.Impl.Customer;
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

        private readonly PTEngine.Services.ICustomerService customerService = new CustomerService();
        private readonly Fanex.ExternalAccount.Service.Interface.Customer.ICustomerService externalCustomerService = new Fanex.ExternalAccount.Service.Impl.Customer.CustomerService();
        private readonly ISiteDBConfigService siteDBConfigService = new SiteDBConfigService();

        public ActionResult SuperMasterEditSB(int custId) => PartialViewBase(externalCustomerService.IsArchivedCustId(custId));

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

        public ActionResult EditMultiplePT(int custId) => PartialViewBase(externalCustomerService.IsArchivedCustId(custId));

        public ActionResult SuperMasterEditMultiplePTMember(int custId) => PartialViewBase(externalCustomerService.IsArchivedCustId(custId));

        public ActionResult ViewPtInfo(int custId)
        {
            var exclusiveRefSportIds = new List<int>(8);

            for (var i = NG2Start; i <= NG2End; i++)
            {
                exclusiveRefSportIds.Add(i);
            }

            ViewData["ExclusiveRefSportIds"] = exclusiveRefSportIds.ToArray();
            ViewData["SiteDB"] = siteDBConfigService.GetSiteDBConfigByCustId(custId);

            return View("~/Views/BettypeReference/Index.cshtml");
        }
    }
}