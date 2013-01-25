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
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer;
    using Sunplus.Core.Common.IPs;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CustomerController : BaseController
    {
        private readonly SuperService superService = new SuperService();
        private readonly MasterService masterService = new MasterService();
        private readonly AgentService agentService = new AgentService();
        private readonly MemberService memberService = new MemberService();

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult CustomerList()
        {
            return this.ViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult SuperListPost(string groupName = "ACCDDD", bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var result = this.superService.GetData(null, null, enabled, closed, suspended, groupName);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult MasterListPost(int? sRecommend = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var result = this.masterService.GetData(sRecommend, null, enabled, closed, suspended);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult AgentListPost(int? mRecommend = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var result = this.agentService.GetData(mRecommend, null, enabled, closed, suspended);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult MemberListPost(int? recommend = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var result = this.memberService.GetData(recommend, null, enabled, closed, suspended);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult ViewIP(string ip)
        {
            var info = IpUtils.LocationFromIP(ip);
            var ipInfo = new IPInfo()
            {
                IP = ip,
                City = info.City,
                Country = info.CountryLong,
                Flag = info.CountryShort + ".gif",
                Info = string.Format(ViewLang.IPNote, "" + ip)
            };

            return this.PartialViewBase(ipInfo);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult ConfigDoubleComm(int custid, bool allow)
        {
            var result = this.superService.ConfigDoubleComm(custid, allow, new Request()
            {
                Browser = Request.Browser.Browser,
                CountryCode = CurrentContext.CountryCode,
                DomainName = CurrentContext.DomainName,
                IP = CurrentContext.Ip4Address,
                Language = "en",
                NetSessionId = CurrentSession.NetSessionId,
                Platform = Request.Browser.Platform,
                SubAccId = 0,
                UserId = CurrentSession.UserId
            });

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage, IsSuccess = result.IsSuccess });
        }
    }
}