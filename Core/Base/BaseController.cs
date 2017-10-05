// --------------------------------------------------------------------------
// <copyright file="BaseController.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// <summary>
// Base controller for controller to inherit
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.13">Style Cop</change>
// </history>

namespace Fanex.ExternalAccount.Core.Base
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.SessionState;
    using CasinoLibrary.Model.UserException;
    using CustomerSetting.Module.Core.Criteria;
    using CustomerSetting.Module.Core.Services.Impl;
    using CustomerSetting.Module.Core.Services.Interface;
    using Fanex.CustomerSetting.IP.Utils;
    using Fanex.CustomerSetting.Module.Core.Web;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Customer.Casino;
    using Fanex.ExternalAccount.Service.Impl.Login;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.PTEngine.BusinessObjects;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Reports.WinLossDetail;
    using CustomerSetting.Module.Core.Models;

    /// <summary>
    /// Base controller for controller to inherit.
    /// </summary>
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class BaseController : Controller
    {
        #region Constant

        /// <summary>
        /// Constant object.
        /// </summary>
        public const int Super = 4;

        /// <summary>
        /// Constant object.
        /// </summary>
        public const int Master = 3;

        /// <summary>
        /// Constant object.
        /// </summary>
        public const int Agent = 2;

        /// <summary>
        /// Constant object.
        /// </summary>
        public const int Member = 1;

        /// <summary>
        /// Constant object.
        /// </summary>
        protected const string SiteName = "ExtAcc";

        /// <summary>
        /// Constant object.
        /// </summary>
        protected const int FromSystem = 3;

        protected const string UAT = "UAT";
        protected const string PRO = "LIVE";

        #endregion Constant

        /// <summary>
        /// Context of current logging in session.
        /// </summary>
        private readonly ICurrentContext _currentContext;

        /// <summary>
        /// Session of current logging in session.
        /// </summary>
        private readonly ICurrentSession _currentSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        protected BaseController()
        {
            this._currentContext = new Fanex.ExternalAccount.Core.Context.CurrentContext();
            this._currentSession = new Fanex.ExternalAccount.Core.Context.CurrentSession();
        }

        protected string Environment
        {
            get { return Utils.ToString(ConfigurationManager.AppSettings["Environment"]); }
        }

        protected bool IncludeAllFunctions
        {
            get { return this.Environment.Equals(UAT, StringComparison.OrdinalIgnoreCase); }
        }

        /// <summary>
        /// Gets current context of this session.
        /// </summary>
        /// <value>ICurrentContext object.</value>
        protected virtual ICurrentContext CurrentContext
        {
            get
            {
                return this._currentContext ?? new Fanex.ExternalAccount.Core.Context.CurrentContext();
            }
        }

        /// <summary>
        /// Gets current session of this session.
        /// </summary>
        /// <value>ICurrentSession object.</value>
        protected virtual ICurrentSession CurrentSession
        {
            get
            {
                return this._currentSession ?? new Fanex.ExternalAccount.Core.Context.CurrentSession();
            }
        }

        /// <summary>
        /// Returns View associated with model object.
        /// </summary>
        /// <param name="viewName">URL to view.</param>
        /// <param name="model">Model object.</param>
        /// <returns>ActionResult object.</returns>
        protected virtual ActionResult ViewBase(string viewName = null, object model = null)
        {
            if (!Response.IsRequestBeingRedirected)
            {
                Response.AppendHeader("Content-Encoding", "gzip");
            }

            Response.Filter = new System.IO.Compression.GZipStream(Response.Filter, System.IO.Compression.CompressionMode.Compress);

            return string.IsNullOrWhiteSpace(viewName) ? this.View(model) : this.View(viewName, model);
        }

        /// <summary>
        /// JSON base method to render JSON to output stream.
        /// </summary>
        /// <param name="data">Data need to render JSON object.</param>
        /// <param name="settings">Allow Get method by default.</param>
        /// <returns>ActionResult object.</returns>
        protected virtual ActionResult JsonBase(object data, JsonSerializerSettings settings = null)
        {
            if (settings == null)
            {
                settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            return this.JsonExt(data, settings);
        }

        /// <summary>
        /// JSON base method to render JSON to output stream.
        /// </summary>
        /// <param name="data">Data need to render JSON object.</param>
        /// <param name="settings">Allow Get method by default.</param>
        /// <returns>ActionResult object.</returns>
        protected virtual ActionResult JsonBase2(object data, JsonRequestBehavior settings = JsonRequestBehavior.AllowGet)
        {
            return this.Json(data, settings);
        }

        /// <summary>
        /// Returns Partial View associated with model object.
        /// </summary>
        /// <param name="objModel">Model object.</param>
        /// <param name="viewName">URL to view.</param>
        /// <returns>PartialViewResult object.</returns>
        protected virtual ActionResult PartialViewBase(object objModel = null, string viewName = null)
        {
            return string.IsNullOrWhiteSpace(viewName) ? this.PartialView(objModel) : this.PartialView(viewName, objModel);
        }

        /// <summary>
        /// Exports excel function.
        /// </summary>
        /// <param name="excelStream">Excel stream.</param>
        /// <param name="fileName">Name of excel file.</param>
        protected virtual void ExportToExcel(MemoryStream excelStream, string fileName)
        {
            if (!Response.IsRequestBeingRedirected)
            {
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
            }

            Response.Clear();
            Response.BinaryWrite(excelStream.GetBuffer());
            Response.End();
        }

        /// <summary>
        /// Updates Casino statues.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="result">Result object.</param>
        /// <param name="customer">Customer object.</param>
        /// <param name="isMult">Is update multiple or not.</param>
        protected void UpdateCasinoStatus(int custId, Fanex.PTEngine.BusinessObjects.Result result, Customer customer, bool isMult = false)
        {
            try
            {
                var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, 0);
                if (casinoCustomer.CasinoSync && (string.IsNullOrWhiteSpace(result.ErrorCode) || Utils.ToInt32(result.ErrorCode) == 0))
                {
                    // Update Casino status for closed and suspended
                    var cust = new CasinoLibrary.SBAgent.Customer();

                    if (this.IsAuthorized(CurrentSession.UserId, eFunctions.CloseCustomer, AuthenticateAction.AuthenticationType.Host, Request))
                    {
                        cust.UpdateCustClosed(custId, customer.StatusSetting.Closed, CurrentContext.Ip4Address, CurrentSession.UserId, CurrentSession.UserName, FromSystem);
                    }

                    if (isMult)
                    {
                        return;
                    }

                    if (this.IsAuthorized(CurrentSession.UserId, eFunctions.SuspendCustomer, AuthenticateAction.AuthenticationType.Host, Request))
                    {
                        cust.UpdateCustSuspend(custId, customer.StatusSetting.Suspended, CurrentContext.Ip4Address, CurrentSession.UserId, CurrentSession.UserName, FromSystem);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Error(ex);
            }
        }

        /// <summary>
        /// Updates casino statuses.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="closed">Closed status.</param>
        /// <param name="suspended">Suspended status.</param>
        protected void UpdateCasinoStatus2(int custId, bool closed, bool suspended)
        {
            var cust = new CasinoLibrary.SBAgent.Customer();
            try
            {
                if (this.IsAuthorized(CurrentSession.UserId, eFunctions.CloseCustomer, AuthenticateAction.AuthenticationType.Host, Request))
                {
                    cust.UpdateCustClosed(custId, closed, CurrentContext.Ip4Address, CurrentSession.UserId, CurrentSession.UserName, FromSystem);
                }

                if (this.IsAuthorized(CurrentSession.UserId, eFunctions.SuspendCustomer, AuthenticateAction.AuthenticationType.Host, Request))
                {
                    cust.UpdateCustSuspend(custId, suspended, CurrentContext.Ip4Address, CurrentSession.UserId, CurrentSession.UserName, FromSystem);
                }
            }
            catch (Exception ex)
            {
                Logs.Error(ex);
            }
        }

        /// <summary>
        /// Updates casino statuses.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="closed">Closed status.</param>
        protected void UpdateCasinoClosed(int custId, bool closed)
        {
            var cust = new CasinoLibrary.SBAgent.Customer();
            try
            {
                if (this.IsAuthorized(CurrentSession.UserId, eFunctions.CloseCustomer, AuthenticateAction.AuthenticationType.Host, Request))
                {
                    cust.UpdateCustClosed(custId, closed, CurrentContext.Ip4Address, CurrentSession.UserId, CurrentSession.UserName, FromSystem);
                }
            }
            catch (ErrorException casinoEx)
            {
                Logs.CasinoError(casinoEx);
            }
            catch (Exception ex)
            {
                Logs.Error(ex);
            }
        }

        /// <summary>
        /// Updates casino statuses.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="suspended">Suspended status.</param>
        protected void UpdateCasinoSuspended(int custId, bool suspended)
        {
            var cust = new CasinoLibrary.SBAgent.Customer();
            try
            {
                if (this.IsAuthorized(CurrentSession.UserId, eFunctions.SuspendCustomer, AuthenticateAction.AuthenticationType.Host, Request))
                {
                    cust.UpdateCustSuspend(custId, suspended, CurrentContext.Ip4Address, CurrentSession.UserId, CurrentSession.UserName, FromSystem);
                }
            }
            catch (ErrorException casinoEx)
            {
                Logs.CasinoError(casinoEx);
            }
            catch (Exception ex)
            {
                Logs.Error(ex);
            }
        }

        /// <summary>
        /// Checks belonging.
        /// </summary>
        /// <param name="groupname">Group name.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="userName">Username of admin.</param>
        /// <param name="isGetUplineInfo">Gets up-line info or not.</param>
        /// <returns>Boolean object.</returns>
        protected bool IsBelong(string groupname, int custId, string userName, bool isGetUplineInfo)
        {
            var loginService = new LoginService();
            var userInfo = new UserService().GetUserById(CurrentSession.UserId);

            if (CurrentSession.GroupId != userInfo.GroupID)
            {
                CurrentSession.IsInternalAdmin = loginService.CheckExternalGroup(CurrentSession.UserId, 11, "ExternalGroup");
                CurrentSession.GroupId = userInfo.GroupID;
                CurrentSession.CurrencyGroup = userInfo.GroupName;
                CurrentSession.IsITAdmin = CurrentSession.GroupId == 1;
            }

            var groups = loginService.GroupList();
            var userGroup = groups.Where(o => o.GroupID == CurrentSession.GroupId);
            var result = new ReportService().GetCustomerInfo(groupname, custId, userName, isGetUplineInfo);
            var hasGroup = CurrentSession.IsInternalAdmin ? groups.Any(o => o.GroupName.Equals(groupname, StringComparison.OrdinalIgnoreCase)) :
                userGroup.Any() ? userGroup.FirstOrDefault().GroupName.Equals(groupname, StringComparison.OrdinalIgnoreCase) : false;

            return (result != null && result.Rows.Count > 0 && hasGroup) || (custId == 0 && hasGroup);
        }

        /// <summary>
        /// Returns denied action.
        /// </summary>
        /// <param name="request">Request context.</param>
        /// <returns>ActionResult object.</returns>
        protected ActionResult ActionDenied(HttpRequestBase request)
        {
            if (request.IsAjaxRequest())
            {
                return new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        errorCode = 1,
                        errorMessage = ViewLang.DontHavePermission,
                        isSuccess = false
                    }
                };
            }
            else
            {
                return new ViewResult
                {
                    ViewName = "~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml"
                };
            }
        }

        /// <summary>
        /// Checks authorize on function of an admin.
        /// </summary>
        /// <param name="userId">Admin user id.</param>
        /// <param name="functionKey">Function key.</param>
        /// <param name="policy">Policy permission.</param>
        /// <param name="request">HttpRequestBase object.</param>
        /// <returns>Boolean object.</returns>
        protected bool IsAuthorized(int userId, string functionKey, AuthenticateAction.AuthenticationType policy, HttpRequestBase request)
        {
            var userCache = new CachingService().GetUserCache(userId);
            var menuList = userCache.MenuList;
            var foundItems = userCache.PolicyList.Where(x => x.FuncKey == functionKey);
            var hasPerm = true;

            if (!request.RawUrl.Equals("/") && !request.RawUrl.ToLower().Contains("home/index") &&
                !request.RawUrl.ToLower().Contains("exporttoexcel") && !request.IsAjaxRequest())
            {
                hasPerm = menuList.Any(o => !string.IsNullOrWhiteSpace(o.Url) &&
                    request.RawUrl.ToLower().Contains(o.Url.ToLower().Replace("~", string.Empty)));
            }

            if (!hasPerm && (foundItems == null || foundItems.Any()))
            {
                return false;
            }

            if (foundItems.Any())
            {
                var firstItem = foundItems.First();
                switch (policy)
                {
                    case AuthenticateAction.AuthenticationType.Read:
                        if (firstItem.Policy == EPolicy.Read || firstItem.Policy == EPolicy.Update || firstItem.Policy == EPolicy.Delete ||
                            firstItem.Policy == EPolicy.Create)
                        {
                            return true;
                        }

                        break;
                    case AuthenticateAction.AuthenticationType.Host:
                        if (firstItem.Policy == EPolicy.Read)
                        {
                            return false;
                        }

                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks and remove IPVN if admin does not have ViewVNIP permission.
        /// </summary>
        /// <param name="table">DataTable customer.</param>
        /// <param name="ipFieldName">Field name contains IP address.</param>
        /// <returns>DataTable object.</returns>
        protected DataTable CheckVNIP(DataTable table, string ipFieldName = null)
        {
            string fieldName = string.IsNullOrWhiteSpace(ipFieldName) ? "lastloginip" : ipFieldName;
            if (object.ReferenceEquals(null, table) || table.Rows.Count == 0 || !table.Columns.Contains(fieldName))
            {
                return table;
            }

            foreach (DataRow row in table.Rows)
            {
                string ip = row.ToString(fieldName);
                if (string.IsNullOrWhiteSpace(ip) || !IpUtils.IsVnIp(ip))
                {
                    continue;
                }

                if (!this.IsAuthorized(CurrentSession.UserId, eFunctions.ViewVNIP, AuthenticateAction.AuthenticationType.Read, Request))
                {
                    row[fieldName] = string.Empty;
                }
            }

            return table;
        }

        /// <summary>
        /// Gets level name of request.
        /// </summary>
        /// <param name="viewlevel">Level id of user.</param>
        /// <returns>Level name of user.</returns>
        protected virtual string LevelName(int viewlevel)
        {
            switch (viewlevel)
            {
                case 1:
                    return "Member";
                case 2:
                    return "Agent";
                case 3:
                    return "Master";
                default:
                    return "Super";
            }
        }

        protected bool IsAdminHasPermissionOnCustomer(string username, bool isSpecialSiteCustomer)
        {
            var permisisonService = new ServiceBase();
            if (isSpecialSiteCustomer)
            {
                var checkCustomerIsInternalAdminResult = permisisonService.CheckCustomerIsInternalAdmin(new CheckCustomerIsInternalAdminCriteria() { CustID = CurrentSession.CustId });
                if (checkCustomerIsInternalAdminResult.IsInternalAdmin)
                {
                    if (!checkCustomerIsInternalAdminResult.IsAllowAccess)
                    {
                        return false;
                    }
                    
                    bool isHasAccessSpecialSitePermission = permisisonService.CheckCustomerHasSpecialSitePermission(new CheckCustomerHasSpecialSiteFunctionPermissionCriteria() { CustID = CurrentSession.CustId });
                    if (!isHasAccessSpecialSitePermission)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected CheckCustomerBelongToSpecialSiteResult CheckCustomerBelongToSpecialSite(string username)
        {
            var permissionService = new ServiceBase();
            return permissionService.CheckCustomerBelongToSpecialSite(new CheckCustomerIsSpecialSiteCriteria() { UserName = username });
        }

        protected List<KeyValuePair<string, string>> GetProductList()
        {
            var list = new List<KeyValuePair<string, string>>();
            var products = ProductsConfiguration.Instance.ProductCollection;
            IServiceBase permissionService = new ServiceBase();
            var checkCustomerIsInternalAdminResult = permissionService.CheckCustomerIsInternalAdmin(new CheckCustomerIsInternalAdminCriteria() { CustID = CurrentSession.CustId });
            bool isAuthorized = permissionService.CheckCustomerHasSpecialSitePermission(new CheckCustomerHasSpecialSiteFunctionPermissionCriteria() { CustID = CurrentSession.CustId });
            foreach (ProductElementConfiguration product in products)
            {
                if (product.IsSpecial)
                {
#pragma warning disable S2589 // Boolean expressions should not be gratuitous
                    if (!IsHiddenProduct(product) && 
                        (!checkCustomerIsInternalAdminResult.IsInternalAdmin || (checkCustomerIsInternalAdminResult.IsInternalAdmin && checkCustomerIsInternalAdminResult.IsAllowAccess && isAuthorized)))
#pragma warning restore S2589 // Boolean expressions should not be gratuitous
                    {
                        list.Add(new KeyValuePair<string, string>(
                            product.Id,
                            product.ProductId == 0 ? product.Name : this.GetProductNameFromRemotingRef(product.ProductId)));
                    }
                }
                else
                {
                    if (!this.IsHiddenProduct(product))
                    {
                        list.Add(new KeyValuePair<string, string>(
                            product.Id,
                            product.ProductId == 0 ? product.Name : this.GetProductNameFromRemotingRef(product.ProductId)));
                    }
                }
            }

            return list;
        }

        protected bool IsHiddenProduct(ProductElementConfiguration product)
        {
            if (product.Hide)
            {
                return true;
            }

            if (product.ShowForGroups.Equals("*", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var groups = product.ShowForGroups.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var requestURL = Request != null ? Request.UrlReferrer.AbsolutePath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries) : new string[0];
            var groupName = requestURL.Length == 0 || !CurrentSession.IsInternalAdmin ? CurrentSession.CurrencyGroup : requestURL[requestURL.Length - 1];

            foreach (var group in groups)
            {
                if (groupName.Equals(group, StringComparison.OrdinalIgnoreCase) ||
                    groupName.Equals("Finance", StringComparison.OrdinalIgnoreCase) ||
                    groupName.Equals("Internal", StringComparison.OrdinalIgnoreCase) ||
                    groupName.Equals("IT", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        protected string GetProductNameFromRemotingRef(int productId)
        {
            return RemotingRef.RefData.GetResourceDataByRefIdAndResourceId("Product", productId.ToString(), "en", null);
        }
    }
}