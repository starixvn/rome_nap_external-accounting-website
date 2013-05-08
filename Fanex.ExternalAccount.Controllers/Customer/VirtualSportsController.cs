// -----------------------------------------------------------------------
// <copyright file="VirtualSportsController.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Controllers.Customer.VirtualSports
{
    using System;
    using System.Collections;
    using System.Web.Mvc;
    using Fanex.Caching;
    using Fanex.ExternalAccount.Controllers.Base;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Fanex.ExternalAccount.ViewModel.Customer;
    using Fanex.PTEngine.BusinessObjects;
    using Fanex.PTVirtualSports.Services;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VirtualSportsController : BaseController
    {
        [Authorization(Function = eFunctions.VirtualSports, Policy = ePolicy.Read)]
        [HttpGet]
        public ActionResult EditSingle(PTVirtualSportsModel model)
        {
            return this.PartialViewBase(model);
        }

        [Authorization(Function = eFunctions.VirtualSports, Policy = ePolicy.Read)]
        [HttpGet]
        public ActionResult DataSource(PTVirtualSportsModel model)
        {
            var currentContext = new RequestContext()
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = model.TargetCustId,
                CustId = model.CustId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = model.RoleId,
                TargetRoleId = model.TargetRoleId,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper()
            };

            var customerService = new CustomerService();
            var customer = customerService.GetCustomer(currentContext);
            Hashtable ht = new Hashtable();
            ht.Add("customer", customer);
            ht.Add("context", currentContext);
            var result = SerializeToJs(ht);

            return this.Content(result);
        }

        internal static string SerializeToJs(Hashtable tb)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return SerializeToJs(tb, settings);
        }

        internal static string SerializeToJs(Hashtable tb, JsonSerializerSettings settings)
        {
            string result = string.Empty;

            foreach (var key in tb.Keys)
            {
                var value = tb[key];
                result += "var " + key + " = " + JsonConvert.SerializeObject(value, settings) + ";\n";
            }

            return result;
        }

        [Authorization(Function = eFunctions.VirtualSports, Policy = ePolicy.Update)]
        [ActionName("EditSingle"), HttpPost]
        public ActionResult EditSinglePost(PTVirtualSportsUpdateModel model)
        {
            var currentContext = new RequestContext()
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = model.TargetCustId,
                CustId = model.CustId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = model.RoleId,
                TargetRoleId = model.TargetRoleId,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper()
            };

            var customerService = new CustomerService();

            return this.PartialViewBase();
        }
    }
}
