// -----------------------------------------------------------------------
// <copyright file="MinPTController.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Controllers.Customer.MinPT
{
    using System;
    using System.Web.Mvc;
    using Fanex.Caching;
    using Fanex.ExternalAccount.Controllers.Base;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.PTEngine.BusinessObjects;
    using Fanex.PTEngine.Services;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    //using Fanex.ExternalAccount.Service.Interface.Customer;
    //using Fanex.ExternalAccount.Service.Impl.Customer;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MinPTController : BaseController
    {
        private ICustomerService customerService = new CustomerService();

        #region Edit Min PT
        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult SuperEditMinPTMaster(int custId)
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult SuperEditMinPTMasterGet(int custId, int parentId)
        {
            var context = new RequestContext()
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
                UserName = CurrentSession.UserName.ToUpper()
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            customer = customerService.GetCustomerForSuperEditMinPTMaster(context);
            customer.BettingLimit = null;
            customer.StatusSetting = null;
            customer.CommissionSetting = null;
            customer.CommissionSetting2 = null;

            return this.JsonBase(new { context = context, customer = customer }, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult SuperEditMinPTMasterPost(int custId, int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            Customer customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            IRequestContext requestContext = new RequestContext()
            {
                CustId = parentId,
                //TargetCustId = custId,
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Master,
                FormId = FormType.EditMinPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            Fanex.PTEngine.BusinessObjects.Result result = customerService.SuperUpdateMinPTMaster(customer, requestContext);
            
            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }
        #endregion Edit Min PT
    }
}