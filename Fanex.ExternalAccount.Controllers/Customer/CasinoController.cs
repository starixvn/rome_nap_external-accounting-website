// --------------------------------------------------------------------------
// <copyright file="CasinoController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.4.15">Created</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Controllers.Base;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Customer.Casino;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// RNG Casino controller
    /// </summary>
    public class CasinoController : BaseController
    {
        [Authorization(Function= eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditCasino(int custId, int uplineId)
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditCasinoPost(int custId, int uplineId)
        {
            var custInfo = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
            var item = CasinoManager.Instance.GetCustSettingCS(custId);
            var betting = CasinoManager.Instance.GetGameKinds(custId);

            return this.JsonBase(new { customerInfo = custInfo, casinoInfo = item}, new Newtonsoft.Json.JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }
    }
}