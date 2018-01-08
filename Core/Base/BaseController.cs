namespace Fanex.ExternalAccount.Core.Base
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.SessionState;
    using Authorization;
    using CasinoLibrary.Model.UserException;
    using CustomerSetting.IP.Utils;
    using CustomerSetting.Module.Core.Criteria;
    using CustomerSetting.Module.Core.Models;
    using CustomerSetting.Module.Core.Services.Impl;
    using CustomerSetting.Module.Core.Services.Interface;
    using CustomerSetting.Module.Core.Web;
    using CustomerSetting.Security.Authenticate;
    using DTO.UserControl;
    using Logging.Extensions.Web;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using PTEngine.BusinessObjects;
    using RemotingRef;
    using Reports.WinLossDetail;
    using Service.Impl.Customer.Casino;
    using Utilities;
    using CurrentContext = Context.CurrentContext;
    using CurrentSession = Context.CurrentSession;
    using Result = PTEngine.BusinessObjects.Result;

    [SessionState(SessionStateBehavior.ReadOnly)]
    public class BaseController : Controller
    {
        protected const string SiteName = "ExtAcc";

        protected const int FromSystem = 3;

        protected const string UAT = "UAT";
        protected const string PRO = "LIVE";
        private readonly ICurrentContext _currentContext;

        private readonly ICurrentSession _currentSession;
        protected readonly AuthorizationService AuthorizationService = new AuthorizationServiceImpl();

        protected BaseController()
        {
            _currentContext = new CurrentContext();
            _currentSession = new CurrentSession();
        }

        protected static string Environment => Utils.ToString(ConfigurationManager.AppSettings["Environment"]);

        protected bool IncludeAllFunctions => Environment.Equals(UAT, StringComparison.OrdinalIgnoreCase);

        protected virtual ICurrentContext CurrentContext => _currentContext ?? new CurrentContext();

        protected virtual ICurrentSession CurrentSession => _currentSession ?? new CurrentSession();

        protected virtual ActionResult JsonBase(object data, JsonSerializerSettings settings = null)
            => this.JsonExt(data, settings ?? new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

        protected virtual ActionResult JsonBase2(object data,
            JsonRequestBehavior settings = JsonRequestBehavior.AllowGet) => Json(data, settings);

        protected virtual ActionResult PartialViewBase(object objModel = null, string viewName = null) =>
            string.IsNullOrWhiteSpace(viewName) ? PartialView(objModel) : PartialView(viewName, objModel);

        protected virtual void ExportToExcel(MemoryStream excelStream, string fileName)
        {
            if (!Response.IsRequestBeingRedirected)
            {
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", $"attachment;filename={fileName}");
            }

            Response.Clear();
            Response.BinaryWrite(excelStream.GetBuffer());
            Response.End();
        }

        protected void UpdateCasinoStatus(int custId, Result result, Customer customer, bool isMult = false)
        {
            try
            {
                var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, 0);
                if (!casinoCustomer.CasinoSync
                    || !string.IsNullOrWhiteSpace(result.ErrorCode)
                    && Utils.ToInt32(result.ErrorCode) != 0)
                {
                    return;
                }

                var cust = new CasinoLibrary.SBAgent.Customer();

                if (AuthorizationService.IsAuthorized(
                    CurrentSession.UserId,
                    eFunctions.CloseCustomer,
                    AuthenticateAction.AuthenticationType.Host))
                {
                    cust.UpdateCustClosed(custId, customer.StatusSetting.Closed, CurrentContext.Ip4Address,
                        CurrentSession.UserId, CurrentSession.UserName, FromSystem);
                }

                if (isMult)
                {
                    return;
                }

                if (AuthorizationService.IsAuthorized(
                    CurrentSession.UserId,
                    eFunctions.SuspendCustomer,
                    AuthenticateAction.AuthenticationType.Host))
                {
                    cust.UpdateCustSuspend(custId, customer.StatusSetting.Suspended, CurrentContext.Ip4Address,
                        CurrentSession.UserId, CurrentSession.UserName, FromSystem);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        protected void UpdateCasinoStatus2(int custId, bool closed, bool suspended)
        {
            var cust = new CasinoLibrary.SBAgent.Customer();
            try
            {
                if (AuthorizationService.IsAuthorized(
                    CurrentSession.UserId,
                    eFunctions.CloseCustomer,
                    AuthenticateAction.AuthenticationType.Host))
                {
                    cust.UpdateCustClosed(custId, closed, CurrentContext.Ip4Address, CurrentSession.UserId,
                        CurrentSession.UserName, FromSystem);
                }

                if (AuthorizationService.IsAuthorized(
                    CurrentSession.UserId,
                    eFunctions.SuspendCustomer,
                    AuthenticateAction.AuthenticationType.Host))
                {
                    cust.UpdateCustSuspend(custId, suspended, CurrentContext.Ip4Address, CurrentSession.UserId,
                        CurrentSession.UserName, FromSystem);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        protected void UpdateCasinoClosed(int custId, bool closed)
        {
            var cust = new CasinoLibrary.SBAgent.Customer();
            try
            {
                if (AuthorizationService.IsAuthorized(
                    CurrentSession.UserId,
                    eFunctions.CloseCustomer,
                    AuthenticateAction.AuthenticationType.Host))
                {
                    cust.UpdateCustClosed(custId, closed, CurrentContext.Ip4Address, CurrentSession.UserId,
                        CurrentSession.UserName, FromSystem);
                }
            }
            catch (ErrorException casinoEx)
            {
                ExceptionHandler.Handle(new Exception(casinoEx.Message, casinoEx), TraceEventType.Information);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        protected void UpdateCasinoSuspended(int custId, bool suspended)
        {
            var cust = new CasinoLibrary.SBAgent.Customer();
            try
            {
                if (AuthorizationService.IsAuthorized(
                    CurrentSession.UserId,
                    eFunctions.SuspendCustomer,
                    AuthenticateAction.AuthenticationType.Host))
                {
                    cust.UpdateCustSuspend(custId, suspended, CurrentContext.Ip4Address, CurrentSession.UserId,
                        CurrentSession.UserName, FromSystem);
                }
            }
            catch (ErrorException casinoEx)
            {
                ExceptionHandler.Handle(new Exception(casinoEx.Message, casinoEx), TraceEventType.Information);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

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

            return new ViewResult
            {
                ViewName = "~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml"
            };
        }

        protected DataTable CheckVNIP(DataTable table, string ipFieldName = null)
        {
            var fieldName = string.IsNullOrWhiteSpace(ipFieldName) ? "lastloginip" : ipFieldName;
            if (ReferenceEquals(null, table) || table.Rows.Count == 0 || !table.Columns.Contains(fieldName))
            {
                return table;
            }

            foreach (DataRow row in table.Rows)
            {
                var ip = row.ToString(fieldName);
                if (string.IsNullOrWhiteSpace(ip) || !IpUtils.IsVnIp(ip))
                {
                    continue;
                }

                if (!AuthorizationService.IsAuthorized(
                    CurrentSession.UserId,
                    eFunctions.ViewVNIP))
                {
                    row[fieldName] = string.Empty;
                }
            }

            return table;
        }

        protected bool IsAdminHasPermissionOnCustomer(bool isSpecialSiteCustomer)
        {
            var permisisonService = new ServiceBase();

            if (!isSpecialSiteCustomer)
            {
                return true;
            }

            var isInternalAdminResult =
                permisisonService.CheckCustomerIsInternalAdmin(
                    new CheckCustomerIsInternalAdminCriteria { CustID = CurrentSession.CustId });

            if (!isInternalAdminResult.IsInternalAdmin)
            {
                return true;
            }

            if (!isInternalAdminResult.IsAllowAccess)
            {
                return false;
            }

            return permisisonService.CheckCustomerHasSpecialSitePermission(
                new CheckCustomerHasSpecialSiteFunctionPermissionCriteria { CustID = CurrentSession.CustId });
        }

        protected CheckCustomerBelongToSpecialSiteResult CheckCustomerBelongToSpecialSite(string username)
        {
            var permissionService = new ServiceBase();
            return permissionService.CheckCustomerBelongToSpecialSite(
                new CheckCustomerIsSpecialSiteCriteria { UserName = username });
        }

        protected List<KeyValuePair<string, string>> GetProductList()
        {
            var list = new List<KeyValuePair<string, string>>();
            var products = ProductsConfiguration.Instance.ProductCollection;
            IServiceBase permissionService = new ServiceBase();

            var checkIsInternalAdminResult =
                permissionService.CheckCustomerIsInternalAdmin(
                    new CheckCustomerIsInternalAdminCriteria { CustID = CurrentSession.CustId });

            var isAuthorized = permissionService.CheckCustomerHasSpecialSitePermission(
                new CheckCustomerHasSpecialSiteFunctionPermissionCriteria { CustID = CurrentSession.CustId });

            foreach (ProductElementConfiguration product in products)
            {
                if (!IsHiddenProduct(product))
                {
                    if (product.IsSpecial &&
                        AdminDoesNotHavePermissionOnSpecialProduct(checkIsInternalAdminResult, isAuthorized))
                    {
                        continue;
                    }

                    list.Add(new KeyValuePair<string, string>(
                        product.Id,
                        product.ProductId == 0 ? product.Name : GetProductNameFromRemotingRef(product.ProductId)));
                }
            }

            return list;
        }

        private static bool AdminDoesNotHavePermissionOnSpecialProduct(
            CheckCustomerIsInternalAdminResult checkIsInternalAdminResult,
            bool isAuthorized)
            => checkIsInternalAdminResult.IsInternalAdmin
               && (!checkIsInternalAdminResult.IsAllowAccess || !isAuthorized);

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

            var groups = product.ShowForGroups.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var requestUrl = Request != null && Request.UrlReferrer != null
                ? Request.UrlReferrer.AbsolutePath.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)
                : new string[0];
            var groupName = requestUrl.Length == 0 || !CurrentSession.IsInternalAdmin
                ? CurrentSession.CurrencyGroup
                : requestUrl[requestUrl.Length - 1];

            foreach (var group in groups)
            {
                if (groupName.Equals(group, StringComparison.OrdinalIgnoreCase)
                    || Group.IsOfficeGroup(groupName))
                {
                    return false;
                }
            }

            return true;
        }

        protected static string GetProductNameFromRemotingRef(int productId) =>
            RefData.GetResourceDataByRefIdAndResourceId("Product", productId.ToString(), "en", null);
    }
}