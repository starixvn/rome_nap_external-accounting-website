// --------------------------------------------------------------------------
// <copyright file="CustomerController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.25">Created</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Controllers.Base;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;
    using Fanex.ExternalAccount.Controllers.Context;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CustomerController : BaseController
    {
        private readonly SuperService superService = new SuperService();
        private readonly MasterService masterService = new MasterService();
        private readonly AgentService agentService = new AgentService();
        private readonly MemberService memberService = new MemberService();
        private readonly CustomerService customerService = new CustomerService();

        public ActionResult CustomerList()
        {
            return this.ViewBase();
        }

        public ActionResult SuperListPost(string groupName = "ACCDDD", bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var result = this.superService.GetData(null, null, enabled, closed, suspended, groupName);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        public ActionResult MasterListPost(int? sRecommend = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var result = this.masterService.GetData(sRecommend, null, enabled, closed, suspended);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        public ActionResult AgentListPost(int? mRecommend = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var result = this.agentService.GetData(mRecommend, null, enabled, closed, suspended);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        public ActionResult MemberListPost(int? recommend = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var result = this.memberService.GetData(recommend, null, enabled, closed, suspended);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        public ActionResult EditInfo(int custId)
        {
            var model = this.customerService.UserInfo(custId);

            return this.PartialViewBase(model);
        }

        public ActionResult EditInfoPost(int custId, string firstName, string lastName, string homePhone, string mobilePhone, string email, string fax)
        {
            var result = this.customerService.EditUserInfo(custId, firstName, lastName, homePhone, mobilePhone, email, fax, CurrentContext.Ip4Address, CurrentSession.CustId, 0, "E");

            return this.JsonBase(result);
        }
    }
}