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
    using System.Data;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Controllers.Base;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Service.Impl.Report;
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
        private readonly CustomerService customerService = new CustomerService();

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult CustomerList()
        {
            return this.ViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult SearchListPost(string groupName = "ACCDDD", bool? enabled = null, bool? closed = null, bool? suspended = null, string username = null)
        {
            var customerInfo = new ReportService().GetCustomerInfo(groupName, 0, username, true);
            var result = new DataTable();
            var sRecommend = 0;
            var mRecommend = 0;
            var recommend = 0;
            var roleId = 0;

            if (customerInfo != null)
            {
                roleId = Utils.ToInt32(customerInfo.Rows[0]["roleid"]);
                var custId = Utils.ToInt32(customerInfo.Rows[0]["custid"]);
                sRecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                mRecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);

                if (roleId == 4)
                {
                    result = this.superService.GetData(null, custId, enabled, closed, suspended, groupName);
                }
                else if (roleId == 3)
                {
                    result = this.masterService.GetData(sRecommend, custId, enabled, closed, suspended);
                }
                else if (roleId == 2)
                {
                    result = this.agentService.GetData(mRecommend, custId, enabled, closed, suspended);
                }
                else
                {
                    result = this.memberService.GetData(recommend, custId, enabled, closed, suspended);
                }
            }

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result != null ? result.Rows.Count : 0, rows = ret, roleId = roleId, srecommend = sRecommend, mrecommend = mRecommend, recommend = recommend });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult SuperListPost(string groupName = "ACCDDD", int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var result = this.superService.GetData(null, custId, enabled, closed, suspended, groupName);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult MasterListPost(int? sRecommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var result = this.masterService.GetData(sRecommend, custId, enabled, closed, suspended);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult AgentListPost(int? mRecommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var result = this.agentService.GetData(mRecommend, custId, enabled, closed, suspended);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult MemberListPost(int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var result = this.memberService.GetData(recommend, custId, enabled, closed, suspended);

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

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditInfo(int custId)
        {
            var model = this.customerService.UserInfo(custId);

            return this.PartialViewBase(model);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult EditInfoPost(int custId, string firstName, string lastName, string homePhone, string mobilePhone, string email, string fax)
        {
            var result = this.customerService.EditUserInfo(custId, firstName, lastName, homePhone, mobilePhone, email, fax, CurrentContext.Ip4Address, CurrentSession.CustId, 0, "E");

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage, IsSuccess = result.IsSuccess });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult ConfigMultipleDoubleComm()
        {
            return this.PartialViewBase();
        }
    }
}