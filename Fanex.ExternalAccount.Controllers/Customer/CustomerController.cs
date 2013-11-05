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
    using System;
    using System.Data;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.Caching;
    using Fanex.CustomerSetting.IP.Utils;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer;
    using Fanex.PTEngine.BusinessObjects;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CustomerController : CustomerBase
    {
        private readonly SuperService _superService = new SuperService();
        private readonly MasterService _masterService = new MasterService();
        private readonly AgentService _agentService = new AgentService();
        private readonly MemberService _memberService = new MemberService();
        private readonly CustomerService _customerService = new CustomerService();
        private string _cacheList = string.Empty;
        private ICache _cache = CacheService.CreateInstance(CacheType.MemoryCache);

        /// <summary>
        /// Checks username.
        /// </summary>
        /// <param name="username">Customer username.</param>
        /// <param name="parentRoleId">Parent role id.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.ManageCustomer, Policy = EPolicy.Read)]
        public ActionResult CheckUsername(string username, int parentRoleId)
        {
            int errCode = 0;
            string errMsg = string.Empty;
            int roleId = 0;
            bool isSubAccount = false;
            int customerId = -1;

            bool isValidUser = this._customerService.IsUsernameBelong(username.Trim(), this.CurrentSession.CustId, out customerId, out isSubAccount, out roleId);

            if (isValidUser && this.CurrentSession.UserName.Trim().ToLower() != username.Trim().ToLower() && (parentRoleId - 1) == roleId)
            {
                errMsg = customerId.ToString();
                errCode = 999;
            }
            else
            {
                errCode = 101;
                errMsg = ViewLang.notExist;
            }
            
            return this.JsonBase(
                new { errCode = errCode, errMsg = errCode }, 
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                });
        }

        #region Super Add New Master

        /// <summary>
        /// Gets add new super page.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.CreateNew, Policy = EPolicy.Read)]
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult SuperAddNewMaster()
        {
            return this.PartialViewBase();
        }

        /// <summary>
        /// Gets super add new data.
        /// </summary>
        /// <param name="custId">Master id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="parentUsername">Parent username.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.CreateNew, Policy = EPolicy.Read)]
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult SuperAddNewMasterGet(int custId, int parentId, string parentUsername)
        {
            var context = new RequestContext()
            {
                AdminId = parentId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.Add,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Master,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = parentUsername.ToUpper()
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();
            customer = new Fanex.PTEngine.Services.CustomerService().GetCustomerForAddNewMaster(context);

            return this.JsonBase(
                new { context = context, customer = customer }, 
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                });
        }

        /// <summary>
        /// Add new master.
        /// </summary>
        /// <param name="custId">Master id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="txtCustomerObject">Customer object model.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.CreateNew, Policy = EPolicy.Create)]
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Create)]
        public ActionResult SuperAddNewMasterPost(int custId, int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            Customer customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            IRequestContext requestContext = new RequestContext()
            {
                CustId = parentId,
                TargetCustId = 0,
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Master,
                FormId = FormType.Add,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            int masterId = -1;
            var result = new Fanex.PTEngine.Services.CustomerService().AddNewMaster(customer, requestContext, out masterId);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage, CustId = masterId });
        }
        #endregion Super Add New Master

        #region Master Add New Agent

        /// <summary>
        /// Gets agent page.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.CreateNew, Policy = EPolicy.Read)]
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult MasterAddNewAgent()
        {
            return this.PartialViewBase();
        }

        /// <summary>
        /// Gets agent data.
        /// </summary>
        /// <param name="custId">Agent's id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="parentUsername">Parent username.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.CreateNew, Policy = EPolicy.Read)]
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult MasterAddNewAgentGet(int custId, int parentId, string parentUsername)
        {
            var context = new RequestContext()
            {
                AdminId = parentId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.Add,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Agent,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = parentUsername.ToUpper()
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();
            customer = new Fanex.PTEngine.Services.CustomerService().GetCustomerForAddNewAgent(context);

            return this.JsonBase(
                new { context = context, customer = customer }, 
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                });
        }

        /// <summary>
        /// Add new agent.
        /// </summary>
        /// <param name="custId">Agent's id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="txtCustomerObject">Customer object model.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.CreateNew, Policy = EPolicy.Create)]
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Create)]
        public ActionResult MasterAddNewAgentPost(int custId, int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            Customer customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            IRequestContext requestContext = new RequestContext()
            {
                CustId = parentId,
                TargetCustId = 0,
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Agent,
                FormId = FormType.Add,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            int agentId = -1;
            var result = new Fanex.PTEngine.Services.CustomerService().AddNewAgent(customer, requestContext, out agentId);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage, CustId = agentId });
        }
        #endregion Master Add New Agent

        #region Agent Add New Member

        /// <summary>
        /// Get member page.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.CreateNew, Policy = EPolicy.Read)]
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult AgentAddNewMember()
        {
            return this.PartialViewBase();
        }

        /// <summary>
        /// Gets member data.
        /// </summary>
        /// <param name="custId">Member id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="parentUsername">Parent username.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.CreateNew, Policy = EPolicy.Read)]
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult AgentAddNewMemberGet(int custId, int parentId, string parentUsername)
        {
            var context = new RequestContext()
            {
                AdminId = parentId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.Add,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int)CustomerRole.Agent,
                TargetRoleId = (int)CustomerRole.Member,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = parentUsername.ToUpper()
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();
            customer = new Fanex.PTEngine.Services.CustomerService().GetCustomerForAddNewMember(context);

            return this.JsonBase(
                new { context = context, customer = customer }, 
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                });
        }

        /// <summary>
        /// Add new member.
        /// </summary>
        /// <param name="custId">Member id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="txtCustomerObject">Customer object model.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.CreateNew, Policy = EPolicy.Create)]
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Create)]
        public ActionResult AgentAddNewMemberPost(int custId, int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            Customer customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            IRequestContext requestContext = new RequestContext()
            {
                CustId = parentId,
                TargetCustId = 0,
                RoleId = (int)CustomerRole.Agent,
                TargetRoleId = (int)CustomerRole.Member,
                FormId = FormType.Add,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            int memberId = -1;
            var result = new Fanex.PTEngine.Services.CustomerService().AddNewMember(customer, requestContext, out memberId);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage, CustId = memberId });
        }
        #endregion Agent Add New Member

        /// <summary>
        /// Gets customer list page.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.ManageCustomer, Policy = EPolicy.Read, IsAjaxRequest = false)]
        public ActionResult CustomerList()
        {
            return this.PartialViewBase();
        }
                
        /// <summary>
        /// Gets setting page.
        /// </summary>
        /// <param name="id">Customer level id.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="uplineId">Parent id.</param>
        /// <param name="userName">Customer username.</param>
        /// <param name="groupName">Group name.</param>
        /// <returns>ActionResult object.</returns>
        public ActionResult Settings(int id, int custId, int uplineId, string userName, string groupName)
        {
            var result = new DataTable();
            var srecommend = 0;
            var mrecommend = 0;
            var recommend = 0;
            var roleId = 0;
            var userId = 0;
            
            this.GetDataResult(groupName, null, null, null, userName, out result, out srecommend, out mrecommend, out recommend, out userId, out roleId);
            
            ViewBag.CurrentLevel = id;
            ViewBag.UserId = custId;
            ViewBag.UplineId = uplineId;
            ViewBag.SyncCasino = (bool)result.Rows[0]["SyncCasino"];
            ViewBag.SyncBingo = (bool)result.Rows[0]["SyncBingo"];
            ViewBag.SyncLiveCasino = (bool)result.Rows[0]["SyncLiveCasino"];
            ViewBag.DisableP2P = (bool)result.Rows[0]["DisableP2P"];
            ViewBag.HideVirtualSports = (bool)result.Rows[0]["HideVirtualSports"];

            return this.PartialViewBase();
        }

        /// <summary>
        /// Sets cache list.
        /// </summary>
        /// <param name="result">Customer data.</param>
        [Authorization(Function = eFunctions.ManageCustomer, Policy = EPolicy.Read)]
        public void SetCacheList(DataTable result)
        {
            if (result != null)
            {
                this._cache.Set<DataTable>(this._cacheList, result, 120);
            }
        }

        /// <summary>
        /// Gets customer list when searching.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <param name="enabled">Enabled status.</param>
        /// <param name="closed">Closed status.</param>
        /// <param name="suspended">Suspended status.</param>
        /// <param name="username">Customer username.</param>
        /// <param name="isExportExcel">Export excel or not.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.ManageCustomer, Policy = EPolicy.Read)]
        public ActionResult SearchListPost(
            string groupName = "ACCDDD", 
            bool? enabled = null, 
            bool? closed = null, 
            bool? suspended = null, 
            string username = null, 
            bool? isExportExcel = null)
        {
            DataTable result;
            int srecommend;
            int mrecommend;
            int recommend;
            int custId;
            int roleId;
            this.GetDataResult(groupName, enabled, closed, suspended, username, out result, out srecommend, out mrecommend, out recommend, out custId, out roleId);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(
                result == null ? new DataTable() : result, 
                Formatting.None, 
                new IsoDateTimeConverter() { DateTimeFormat = "MM/dd/yyyy HH:mm:ss tt" });

            return this.JsonBase(
                new 
                { 
                    total = 1, 
                    page = 1, 
                    records = result != null ? result.Rows.Count : 0, 
                    rows = ret, 
                    roleId = roleId, 
                    srecommend = srecommend, 
                    mrecommend = mrecommend, 
                    recommend = recommend, 
                    custId = custId 
                });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = EPolicy.Read)]
        public ActionResult SuperListPost(
            string groupName = "ACCDDD", 
            int? custId = null, 
            bool? enabled = null, 
            bool? closed = null, 
            bool? suspended = null)
        {
            this._cacheList = "CacheList" + groupName + custId + enabled + closed + suspended;
            var result = this._superService.GetData(null, custId, enabled, closed, suspended, groupName);
            this.SetCacheList(result);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = "MM/dd/yyyy HH:mm:ss tt" });

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        /// <summary>
        /// Gets customer list.
        /// </summary>
        /// <param name="srecommend">Parent id.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="enabled">Enabled status.</param>
        /// <param name="closed">Closed status.</param>
        /// <param name="suspended">Suspended status.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.ManageCustomer, Policy = EPolicy.Read)]
        public ActionResult MasterListPost(int? srecommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var result = this._masterService.GetData(srecommend, custId, enabled, closed, suspended);

            var superId = Utils.ToInt32(srecommend);
            var customerInfo = new ReportService().GetCustomerInfo(Request["groupName"].ToString(), superId, null, true);
            int mrecommend = 0,
            recommend = 0,
            roleId = 0;

            if (customerInfo != null)
            {
                roleId = Utils.ToInt32(customerInfo.Rows[0]["roleid"]);
                srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);
                custId = Utils.ToInt32(customerInfo.Rows[0]["custid"]);
            }

            this._cacheList = "CacheList" + srecommend + custId + enabled + closed + suspended;
            this.SetCacheList(result);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = "MM/dd/yyyy HH:mm:ss tt" });

            return this.JsonBase(
                new 
                { 
                    total = 1, 
                    page = 1, 
                    records = result.Rows.Count, 
                    rows = ret, roleId = roleId, srecommend = srecommend, 
                    mrecommend = mrecommend, recommend = recommend, custId = custId 
                });
        }

        /// <summary>
        /// Gets customer list.
        /// </summary>
        /// <param name="mrecommend">Parent id.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="enabled">Enabled status.</param>
        /// <param name="closed">Closed status.</param>
        /// <param name="suspended">Suspended status.</param>
        /// <returns>ActionResult object.</returns>        
        [Authorization(Function = eFunctions.ManageCustomer, Policy = EPolicy.Read)]
        public ActionResult AgentListPost(int? mrecommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var result = this._agentService.GetData(mrecommend, custId, enabled, closed, suspended);

            var masterId = Utils.ToInt32(mrecommend);
            var customerInfo = new ReportService().GetCustomerInfo(Request["groupName"].ToString(), masterId, null, true);
            int srecommend = 0,
            recommend = 0,
            roleId = 0;

            if (customerInfo != null)
            {
                roleId = Utils.ToInt32(customerInfo.Rows[0]["roleid"]);
                srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);
                custId = Utils.ToInt32(customerInfo.Rows[0]["custid"]);
            }

            this._cacheList = "CacheList" + mrecommend + custId + enabled + closed + suspended;
            this.SetCacheList(result);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = "MM/dd/yyyy HH:mm:ss tt" });

            return this.JsonBase(
                new 
                { 
                    total = 1, page = 1, records = result.Rows.Count, rows = ret, roleId = roleId, srecommend = srecommend, 
                    mrecommend = mrecommend, recommend = recommend, custId = custId 
                });
        }

        /// <summary>
        /// Gets customer list.
        /// </summary>
        /// <param name="recommend">Parent id.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="enabled">Enabled status.</param>
        /// <param name="closed">Closed status.</param>
        /// <param name="suspended">Suspended status.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.ManageCustomer, Policy = EPolicy.Read)]
        public ActionResult MemberListPost(int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var result = this._memberService.GetData(recommend, custId, enabled, closed, suspended);

            var agentId = Utils.ToInt32(recommend);
            var customerInfo = new ReportService().GetCustomerInfo(Request["groupName"].ToString(), agentId, null, true);
            int mrecommend = 0,
            srecommend = 0,
            roleId = 0;

            if (customerInfo != null)
            {
                roleId = Utils.ToInt32(customerInfo.Rows[0]["roleid"]);
                srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);
                custId = Utils.ToInt32(customerInfo.Rows[0]["custid"]);
            }

            this._cacheList = "CacheList" + recommend + custId + enabled + closed + suspended;
            this.SetCacheList(result);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = "MM/dd/yyyy HH:mm:ss tt" });

            return this.JsonBase(
                new 
                { 
                    total = 1, page = 1, records = result.Rows.Count, rows = ret, roleId = roleId, 
                    srecommend = srecommend, mrecommend = mrecommend, recommend = recommend, custId = custId 
                });
        }

        /// <summary>
        /// Views IP information.
        /// </summary>
        /// <param name="ip">IP address.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.ManageCustomer, Policy = EPolicy.Read)]
        public ActionResult ViewIP(string ip)
        {
            if (ip.Equals("::1", StringComparison.OrdinalIgnoreCase))
            {
                return this.PartialViewBase(new IPInfo
                {
                    City = string.Empty,
                    Country = string.Empty,
                    Flag = "-.gif",
                    Info = string.Empty,
                    IP = ip
                });
            }

            var info = IpUtils.LocationFromIP(ip);
            var ipinfo = new IPInfo()
            {
                IP = ip,
                City = string.IsNullOrWhiteSpace(info.ipCity) ? string.Empty : info.ipCity,
                Country = string.IsNullOrWhiteSpace(info.countryLONG) ? string.Empty : info.countryLONG,
                Flag = string.IsNullOrWhiteSpace(info.countrySHORT) ? "-.gif" : info.countrySHORT + ".gif",
                Info = string.Format(ViewLang.IPNote, ip)
            };

            return this.PartialViewBase(ipinfo);
        }

        /// <summary>
        /// Config double commission.
        /// </summary>
        /// <param name="custid">Customer id.</param>
        /// <param name="allow">Allow double commission or not.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Update)]
        public ActionResult ConfigDoubleComm(int custid, bool allow)
        {
            var result = this._superService.ConfigDoubleComm(
                custid,
                allow,
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
                    UserId = CurrentSession.UserId
                });

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage, IsSuccess = result.IsSuccess });
        }

        /// <summary>
        /// Gets edit information page.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Read)]
        public ActionResult EditInfo(int custId)
        {
            var model = this._customerService.UserInfo(custId);
            model.FirstName = HttpUtility.HtmlDecode(model.FirstName);
            model.LastName = HttpUtility.HtmlDecode(model.LastName);
            model.HomePhone = HttpUtility.HtmlDecode(model.HomePhone);
            model.MobilePhone = HttpUtility.HtmlDecode(model.MobilePhone);
            model.Email = HttpUtility.HtmlDecode(model.Email);
            model.Fax = HttpUtility.HtmlDecode(model.Fax);

            return this.PartialViewBase(model);
        }

        /// <summary>
        /// Edits user information.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="homePhone">Home phone.</param>
        /// <param name="mobilePhone">Mobile phone.</param>
        /// <param name="email">Email address.</param>
        /// <param name="fax">Fax number.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Update)]
        public ActionResult EditInfoPost(int custId, string firstName, string lastName, string homePhone, string mobilePhone, string email, string fax)
        {
            var result = this._customerService.EditUserInfo(
                    custId, 
                    firstName, 
                    lastName, 
                    homePhone, 
                    mobilePhone, 
                    email,
                    fax, 
                    CurrentContext.Ip4Address, 
                    CurrentSession.CustId, 
                    0, 
                    "E");

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage, IsSuccess = result.IsSuccess });
        }

        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Read)]
        public ActionResult ConfigMultipleDoubleComm(int id)
        {
            ViewBag.RoleId = id;
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Read)]
        public ActionResult TransferCondition()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Read)]
        public ActionResult EditTransferCondition(int custId)
        {
            ViewBag.CustId = custId;
            var model = this._masterService.GetTransferOption(custId);

            return this.PartialViewBase(model);
        }

        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Update)]
        public ActionResult EditTransferConditionPost(string transferOption, int custId)
        {
            var result = this._masterService.EditTransferOption(transferOption, custId, CurrentSession.CustId, CurrentContext.Ip4Address, 0, "E");

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage, IsSuccess = result.IsSuccess });
        }

        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Read)]
        public ActionResult ResetPassword(int custId)
        {
            var user = this._customerService.UserInfo(custId);
            return this.PartialViewBase(user);
        }

        /// <summary>
        /// Update password.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="password">Password characters.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Update)]
        public ActionResult UpdatePassword(int custId, string password)
        {
            var result = this._customerService.ResetPassword(
                custId,
                password,
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

        /// <summary>
        /// Exports excel.
        /// </summary>
        /// <param name="currentLevel">Current level.</param>
        /// <param name="groupName">Group name.</param>
        /// <param name="srecommend">Super agent id.</param>
        /// <param name="mrecommend">Master agent id.</param>
        /// <param name="recommend">Agent's id.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="enabled">Enabled status.</param>
        /// <param name="closed">Closed status.</param>
        /// <param name="suspended">Suspended status.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.ManageCustomer, Policy = EPolicy.Read)]
        public ActionResult GetExportToExcel(
            int? currentLevel = null, 
            string groupName = "ACCDDD", 
            int? srecommend = null, 
            int? mrecommend = null, 
            int? recommend = null, 
            int? custId = null, 
            bool? enabled = null, 
            bool? closed = null, 
            bool? suspended = null)
        {
            string fileName = "CustomerList";

            this.SetCache(currentLevel, ref fileName, groupName, srecommend, mrecommend, recommend, custId, enabled, closed, suspended);
            
            string status = string.Empty;
            if (enabled != null || closed != null || suspended != null)
            {
                status = !Convert.ToBoolean(enabled) ? "Disabled" : Convert.ToBoolean(closed) ? "Closed" : Convert.ToBoolean(suspended) ? "Suspended" : "Open";
            }

            var builder = new Fanex.ExternalAccount.Excel.Customer.CustomerListPage.CustomerListPageBuilder(
                this._cache.Get<DataTable>(this._cacheList), 
                false, 
                status, 
                Convert.ToInt32(currentLevel));
            var excelStream = builder.Build();
            if (excelStream != null)
            {
                this.ExportToExcel(excelStream, status + fileName + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                excelStream.Dispose();
            }

            return this.PartialViewBase();
        }

        /// <summary>
        /// Set cache data for exports excel.
        /// </summary>
        /// <param name="currentLevel">Current level.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="groupName">Group name.</param>
        /// <param name="srecommend">Super agent id.</param>
        /// <param name="mrecommend">Master agent id.</param>
        /// <param name="recommend">Agent's id.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="enabled">Enabled status.</param>
        /// <param name="closed">Closed status.</param>
        /// <param name="suspended">Suspended status.</param>
        private void SetCache(
            int? currentLevel, 
            ref string fileName, 
            string groupName = "ACCDDD",
            int? srecommend = null,
            int? mrecommend = null,
            int? recommend = null,
            int? custId = null,
            bool? enabled = null,
            bool? closed = null,
            bool? suspended = null)
        {
            if (currentLevel == 4)
            {
                fileName = "SuperList";
                this._cacheList = "CacheList" + groupName + custId + enabled + closed + suspended;
                if (this._cache.Get<DataTable>(this._cacheList) == null)
                {
                    this.SetCacheList(this._superService.GetData(null, custId, enabled, closed, suspended, groupName));
                }
            }
            else if (currentLevel == 3)
            {
                fileName = "MasterList";
                this._cacheList = "CacheList" + srecommend + custId + enabled + closed + suspended;
                if (this._cache.Get<DataTable>(this._cacheList) == null)
                {
                    this.SetCacheList(this._masterService.GetData(srecommend, custId, enabled, closed, suspended));
                }
            }
            else if (currentLevel == 2)
            {
                fileName = "AgentList";
                this._cacheList = "CacheList" + mrecommend + custId + enabled + closed + suspended;
                if (this._cache.Get<DataTable>(this._cacheList) == null)
                {
                    this.SetCacheList(this._agentService.GetData(mrecommend, custId, enabled, closed, suspended));
                }
            }
            else if (currentLevel == 1)
            {
                fileName = "MemberList";
                this._cacheList = "CacheList" + recommend + custId + enabled + closed + suspended;
                if (this._cache.Get<DataTable>(this._cacheList) == null)
                {
                    this.SetCacheList(this._memberService.GetData(recommend, custId, enabled, closed, suspended));
                }
            }
        }

        /// <summary>
        /// Gets data customer.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <param name="enabled">Enabled status.</param>
        /// <param name="closed">Closed status.</param>
        /// <param name="suspended">Suspended status.</param>
        /// <param name="username">Customer username.</param>
        /// <param name="result">Result object.</param>
        /// <param name="srecommend">Super agent id.</param>
        /// <param name="mrecommend">Master agent id.</param>
        /// <param name="recommend">Agent's id.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="roleId">Customer role id.</param>
        private void GetDataResult(
            string groupName, 
            bool? enabled, 
            bool? closed, 
            bool? suspended, 
            string username, 
            out DataTable result, 
            out int srecommend, 
            out int mrecommend, 
            out int recommend, 
            out int custId, 
            out int roleId)
        {
            var customerInfo = new ReportService().GetCustomerInfo(groupName, 0, username, true);
            result = null;
            srecommend = 0;
            mrecommend = 0;
            recommend = 0;
            custId = 0;
            roleId = 0;

            if (customerInfo != null)
            {
                roleId = Utils.ToInt32(customerInfo.Rows[0]["roleid"]);
                srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);
                custId = Utils.ToInt32(customerInfo.Rows[0]["custid"]);                

                if (roleId == 4)
                {
                    result = this._superService.GetData(null, custId, enabled, closed, suspended, groupName);
                    this._cacheList = "CacheList" + groupName + custId + enabled + closed + suspended;
                }
                else if (roleId == 3)
                {
                    result = this._masterService.GetData(srecommend, custId, enabled, closed, suspended);
                    this._cacheList = "CacheList" + groupName + srecommend + custId + enabled + closed + suspended;
                }
                else if (roleId == 2)
                {
                    result = this._agentService.GetData(mrecommend, custId, enabled, closed, suspended);
                    this._cacheList = "CacheList" + groupName + mrecommend + custId + enabled + closed + suspended;
                }
                else
                {
                    result = this._memberService.GetData(recommend, custId, enabled, closed, suspended);
                    this._cacheList = "CacheList" + groupName + recommend + custId + enabled + closed + suspended;
                }
            }

            this.SetCacheList(result);
        }
    }
}