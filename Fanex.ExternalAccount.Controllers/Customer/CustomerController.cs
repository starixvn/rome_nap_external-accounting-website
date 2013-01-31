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
    using Fanex.Caching;
    using Fanex.ExternalAccount.Controllers.Base;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer;
    using Sunplus.Core.Common.IPs;
    using System;

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
        private string cacheList = string.Empty;
        private ICache cache = CacheService.CreateInstance(CacheType.MemoryCache);

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult CustomerList()
        {
            return this.ViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public void SetCacheList(DataTable result)
        {
            if (result != null)
            {
                cache.Set<DataTable>(this.cacheList, result, 120);
            }
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult SearchListPost(string groupName = "ACCDDD", bool? enabled = null, bool? closed = null, bool? suspended = null, string username = null)
        {
            DataTable result;
            int sRecommend;
            int mRecommend;
            int recommend;
            int roleId;
            this.GetDataResult(groupName, enabled, closed, suspended, username, out result, out sRecommend, out mRecommend, out recommend, out roleId);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result != null ? result.Rows.Count : 0, rows = ret, roleId = roleId, srecommend = sRecommend, mrecommend = mRecommend, recommend = recommend });
        }

        private void GetDataResult(string groupName, bool? enabled, bool? closed, bool? suspended, string username, out DataTable result, out int sRecommend, out int mRecommend, out int recommend, out int roleId)
        {
            var customerInfo = new ReportService().GetCustomerInfo(groupName, 0, username, true);
            result = new DataTable();
            sRecommend = 0;
            mRecommend = 0;
            recommend = 0;
            roleId = 0;

            if (customerInfo != null)
            {
                roleId = Utils.ToInt32(customerInfo.Rows[0]["roleid"]);
                var custId = Utils.ToInt32(customerInfo.Rows[0]["custid"]);
                sRecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                mRecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);
                this.cacheList = "CacheList" + groupName + custId + enabled + closed + suspended;

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

            this.SetCacheList(result);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult SuperListPost(string groupName = "ACCDDD", int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            this.cacheList = "CacheList" + groupName + custId + enabled + closed + suspended;
            var result = this.superService.GetData(null, custId, enabled, closed, suspended, groupName);
            this.SetCacheList(result);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult MasterListPost(int? sRecommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            this.cacheList = "CacheList" + sRecommend + custId + enabled + closed + suspended;
            var result = this.masterService.GetData(sRecommend, custId, enabled, closed, suspended);
            this.SetCacheList(result);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult AgentListPost(int? mRecommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            this.cacheList = "CacheList" + mRecommend + custId + enabled + closed + suspended;
            var result = this.agentService.GetData(mRecommend, custId, enabled, closed, suspended);
            this.SetCacheList(result);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult MemberListPost(int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            this.cacheList = "CacheList" + recommend + custId + enabled + closed + suspended;
            var result = this.memberService.GetData(recommend, custId, enabled, closed, suspended);
            this.SetCacheList(result);

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

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditTransferCondition(int custId)
        {
            var model = this.masterService.GetTransferOption(custId);

            return this.PartialViewBase(model);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        {
            DataTable result;
            
            // Expired Cache
            if (currentLevel == 1)
            {
                this.cacheList = "CacheList" + custId + enabled + closed + suspended;
                if (this.cache.Get<DataTable>(this.cacheList) == null)
                {
                    //this.GetDataResult(groupName, enabled, closed, suspended, username, out result, out sRecommend, out mRecommend, out recommend, out roleId);
                }
            }

            string status = Convert.ToBoolean(closed) ? "Closed" : (Convert.ToBoolean(suspended) ? "Suspended" : (Convert.ToBoolean(enabled) ? "Open" : "Disabled"));

            var builder = new Fanex.ExternalAccount.Excel.Customer.CustomerListPage.CustomerListPageBuilder(this.cache.Get<DataTable>(this.cacheList), false, status);

            var excelStream = builder.Build();
            if (excelStream != null)
            {
                this.ExportToExcel(excelStream, "CustomerList_" + status + ".xls");
                excelStream.Dispose();
            }
            var user = this.customerService.UserInfo(custId);

            return this.PartialViewBase(user);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult UpdatePassword(int custId, string password)
        {
            var result = this.customerService.ResetPassword(custId, password,
                new Request()
                {
                    Browser = Request.Browser.Browser,
                    CountryCode = CurrentContext.CountryCode,
                    DomainName = CurrentContext.DomainName,
                    IP = CurrentContext.Ip4Address,
                    Language = "en",
                    NetSessionId = CurrentSession.NetSessionId,
                    Platform = Request.Browser.Platform,
                    SubAccId = 0,
                    UserId = custId
                });

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage, IsSuccess = result.IsSuccess });
        }
    }
}