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
    using Fanex.ExternalAccount.Controllers.Base;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer;
    using Fanex.PTEngine.BusinessObjects;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Sunplus.Core.Common.IPs;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CustomerController : CustomerBase
    {
        private readonly SuperService superService = new SuperService();
        private readonly MasterService masterService = new MasterService();
        private readonly AgentService agentService = new AgentService();
        private readonly MemberService memberService = new MemberService();
        private readonly CustomerService customerService = new CustomerService();
        private string cacheList = string.Empty;
        private ICache cache = CacheService.CreateInstance(CacheType.MemoryCache);

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult CheckUsername(string username, int parentRoleId)
        {
            int errCode = 0;
            string errMsg = string.Empty;
            int roleId = 0;
            bool isSubAccount = false;
            int customerId = -1;

            bool isValidUser = this.customerService.IsUsernameBelong(username.Trim(), this.CurrentSession.CustId, out customerId, out isSubAccount, out roleId);

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
                    Formatting = Formatting.Indented
                });
        }

        #region Super Add New Master
        [Authorization(Function = eFunctions.CreateNew, Policy = ePolicy.Read)]
        public ActionResult SuperAddNewMaster()
        {
            return this.ViewBase();
        }

        [Authorization(Function = eFunctions.CreateNew, Policy = ePolicy.Read)]
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
                    Formatting = Formatting.Indented
                });
        }

        [Authorization(Function = eFunctions.CreateNew, Policy = ePolicy.Create)]
        public ActionResult SuperAddNewMasterPost(int custId, int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;

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
        [Authorization(Function = eFunctions.CreateNew, Policy = ePolicy.Read)]
        public ActionResult MasterAddNewAgent()
        {
            return this.ViewBase();
        }

        [Authorization(Function = eFunctions.CreateNew, Policy = ePolicy.Read)]
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
                    Formatting = Formatting.Indented
                });
        }

        [Authorization(Function = eFunctions.CreateNew, Policy = ePolicy.Create)]
        public ActionResult MasterAddNewAgentPost(int custId, int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;

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
        [Authorization(Function = eFunctions.CreateNew, Policy = ePolicy.Read)]
        public ActionResult AgentAddNewMember()
        {
            return this.ViewBase();
        }

        [Authorization(Function = eFunctions.CreateNew, Policy = ePolicy.Read)]
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
                    Formatting = Formatting.Indented
                });
        }

        [Authorization(Function = eFunctions.CreateNew, Policy = ePolicy.Create)]
        public ActionResult AgentAddNewMemberPost(int custId, int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;

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

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult CustomerList()
        {
            return this.ViewBase();
        }
                
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

            return this.ViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public void SetCacheList(DataTable result)
        {
            if (result != null)
            {
                this.cache.Set<DataTable>(this.cacheList, result, 120);
            }
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult SearchListPost(string groupName = "ACCDDD", bool? enabled = null, bool? closed = null, bool? suspended = null, string username = null, bool? isExportExcel = null)
        {
            DataTable result;
            int srecommend;
            int mrecommend;
            int recommend;
            int custId;
            int roleId;
            this.GetDataResult(groupName, enabled, closed, suspended, username, out result, out srecommend, out mrecommend, out recommend, out custId, out roleId);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result == null ? new DataTable() : result);

            return this.JsonBase(new { total = 1, page = 1, records = result != null ? result.Rows.Count : 0, rows = ret, roleId = roleId, srecommend = srecommend, mrecommend = mrecommend, recommend = recommend, custId = custId });
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
        public ActionResult MasterListPost(int? srecommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            this.cacheList = "CacheList" + srecommend + custId + enabled + closed + suspended;
            var result = this.masterService.GetData(srecommend, custId, enabled, closed, suspended);
            this.SetCacheList(result);

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return this.JsonBase(new { total = 1, page = 1, records = result.Rows.Count, rows = ret });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult AgentListPost(int? mrecommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            this.cacheList = "CacheList" + mrecommend + custId + enabled + closed + suspended;
            var result = this.agentService.GetData(mrecommend, custId, enabled, closed, suspended);
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
            try
            {
                var info = IpUtils.LocationFromIP(ip);
                var ipinfo = new IPInfo()
                {
                    IP = ip,
                    City = info.City,
                    Country = info.CountryLong,
                    Flag = info.CountryShort + ".gif",
                    Info = string.Format(ViewLang.IPNote, ip)
                };         
                
                return this.PartialViewBase(ipinfo);
            }
            catch (Exception ex)
            {
                Logs.Error(ex.Message, "ExternalAccount", ex);

                return this.PartialViewBase(new IPInfo
                {
                    City = string.Empty,
                    Country = string.Empty,
                    Flag = "-.gif",
                    Info = string.Empty,
                    IP = ip
                });
            }
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult ConfigDoubleComm(int custid, bool allow)
        {
            var result = this.superService.ConfigDoubleComm(
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

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditInfo(int custId)
        {
            var model = this.customerService.UserInfo(custId);
            model.FirstName = HttpUtility.HtmlDecode(model.FirstName);
            model.LastName = HttpUtility.HtmlDecode(model.LastName);
            model.HomePhone = HttpUtility.HtmlDecode(model.HomePhone);
            model.MobilePhone = HttpUtility.HtmlDecode(model.MobilePhone);
            model.Email = HttpUtility.HtmlDecode(model.Email);
            model.Fax = HttpUtility.HtmlDecode(model.Fax);

            return this.PartialViewBase(model);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult EditInfoPost(int custId, string firstName, string lastName, string homePhone, string mobilePhone, string email, string fax)
        {
            var result = this.customerService.EditUserInfo(custId, firstName, lastName, homePhone, mobilePhone, email, fax, CurrentContext.Ip4Address, CurrentSession.CustId, 0, "E");

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage, IsSuccess = result.IsSuccess });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult ConfigMultipleDoubleComm(int id)
        {
            ViewBag.RoleId = id;
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult TransferCondition()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditTransferCondition(int custId)
        {
            var model = this.masterService.GetTransferOption(custId);

            return this.PartialViewBase(model);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult EditTransferConditionPost(string transferOption, int custId)
        {
            var result = this.masterService.EditTransferOption(transferOption, custId, CurrentSession.CustId, CurrentContext.Ip4Address, 0, "E");

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage, IsSuccess = result.IsSuccess });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult ResetPassword(int custId)
        {
            var user = this.customerService.UserInfo(custId);
            return this.PartialViewBase(user);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult UpdatePassword(int custId, string password)
        {
            var result = this.customerService.ResetPassword(
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

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult GetExportToExcel(int? currentLevel = null, string groupName = "ACCDDD", int? srecommend = null, int? mrecommend = null, int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            string fileName = "CustomerList";
            if (currentLevel == 4)
            {
                fileName = "SuperList";
                this.cacheList = "CacheList" + groupName + custId + enabled + closed + suspended;
                if (this.cache.Get<DataTable>(this.cacheList) == null)
                {
                    this.SetCacheList(this.superService.GetData(null, custId, enabled, closed, suspended, groupName));
                }
            }
            else if (currentLevel == 3)
            {
                fileName = "MasterList";
                this.cacheList = "CacheList" + srecommend + custId + enabled + closed + suspended;
                if (this.cache.Get<DataTable>(this.cacheList) == null)
                {
                    this.SetCacheList(this.masterService.GetData(srecommend, custId, enabled, closed, suspended));
                }
            }
            else if (currentLevel == 2)
            {
                fileName = "AgentList";
                this.cacheList = "CacheList" + mrecommend + custId + enabled + closed + suspended;
                if (this.cache.Get<DataTable>(this.cacheList) == null)
                {
                    this.SetCacheList(this.agentService.GetData(mrecommend, custId, enabled, closed, suspended));
                }
            }
            else if (currentLevel == 1)
            {
                fileName = "MemberList";
                this.cacheList = "CacheList" + recommend + custId + enabled + closed + suspended;
                if (this.cache.Get<DataTable>(this.cacheList) == null)
                {
                    this.SetCacheList(this.memberService.GetData(recommend, custId, enabled, closed, suspended));
                }
            }

            string status = string.Empty;
            if (enabled != null || closed != null || suspended != null)
            {
                status = !Convert.ToBoolean(enabled) ? "Disabled" : Convert.ToBoolean(closed) ? "Closed" : Convert.ToBoolean(suspended) ? "Suspended" : "Open";
            }

            var builder = new Fanex.ExternalAccount.Excel.Customer.CustomerListPage.CustomerListPageBuilder(this.cache.Get<DataTable>(this.cacheList), false, status, Convert.ToInt32(currentLevel));

            var excelStream = builder.Build();
            if (excelStream != null)
            {
                this.ExportToExcel(excelStream, status + fileName + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                excelStream.Dispose();
            }

            return this.ViewBase();
        }

        private void GetDataResult(string groupName, bool? enabled, bool? closed, bool? suspended, string username, out DataTable result, out int srecommend, out int mrecommend, out int recommend, out int custId, out int roleId)
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
                    result = this.superService.GetData(null, custId, enabled, closed, suspended, groupName);
                    this.cacheList = "CacheList" + groupName + custId + enabled + closed + suspended;
                }
                else if (roleId == 3)
                {
                    result = this.masterService.GetData(srecommend, custId, enabled, closed, suspended);
                    this.cacheList = "CacheList" + groupName + srecommend + custId + enabled + closed + suspended;
                }
                else if (roleId == 2)
                {
                    result = this.agentService.GetData(mrecommend, custId, enabled, closed, suspended);
                    this.cacheList = "CacheList" + groupName + mrecommend + custId + enabled + closed + suspended;
                }
                else
                {
                    result = this.memberService.GetData(recommend, custId, enabled, closed, suspended);
                    this.cacheList = "CacheList" + groupName + recommend + custId + enabled + closed + suspended;
                }
            }

            this.SetCacheList(result);
        }
    }
}