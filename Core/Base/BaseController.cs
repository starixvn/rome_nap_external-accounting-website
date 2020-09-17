namespace Fanex.ExternalAccount.Core.Base
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.SessionState;
    using Authorization;
    using CasinoLibrary.Model.UserException;
    using Context;
    using CustomerSetting.Module.Core.Criteria;
    using CustomerSetting.Module.Core.Models;
    using CustomerSetting.Module.Core.Services.Impl;
    using CustomerSetting.Module.Core.Web;
    using CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.DTO.Products;
    using Logging.Extensions.Web;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using PTEngine.BusinessObjects;
    using Service.Impl.Customer.Casino;
    using Shared;
    using Utilities;
    using CurrentContext = Context.CurrentContext;
    using CurrentSession = Context.CurrentSession;
    using Result = PTEngine.BusinessObjects.Result;

    [SessionState(SessionStateBehavior.ReadOnly)]
    public class BaseController : Controller
    {
        protected const string SiteName = "ExtAcc";
        protected const string UAT = "UAT";
        protected const string PRO = "LIVE";
        private readonly ICurrentContext currentContext;
        private readonly ICurrentSession currentSession;
        private readonly IExtRequest extRequest;

        protected readonly AuthorizationService AuthorizationService = new AuthorizationServiceImpl();

        protected BaseController() : this(new CurrentSession(), new CurrentContext(), new ExtRequest())
        {
        }

        protected BaseController(ICurrentSession currentSession, ICurrentContext currentContext, IExtRequest extRequest)
        {
            this.currentContext = currentContext;
            this.currentSession = currentSession;
            this.extRequest = extRequest;
        }

        protected static string Environment => Utils.ToString(ConfigurationManager.AppSettings["Environment"]);

        protected bool IncludeAllFunctions => Environment.Equals(UAT, StringComparison.OrdinalIgnoreCase);

        protected virtual ICurrentContext CurrentContext => currentContext ?? new CurrentContext();

        protected virtual ICurrentSession CurrentSession => currentSession ?? new CurrentSession();

        protected virtual IExtRequest ExtRequest => extRequest ?? new ExtRequest();

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
                        CurrentSession.UserId, CurrentSession.UserName, CurrentSession.FromSystem);
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
                        CurrentSession.UserId, CurrentSession.UserName, CurrentSession.FromSystem);
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
                        CurrentSession.UserName, CurrentSession.FromSystem);
                }

                if (AuthorizationService.IsAuthorized(
                    CurrentSession.UserId,
                    eFunctions.SuspendCustomer,
                    AuthenticateAction.AuthenticationType.Host))
                {
                    cust.UpdateCustSuspend(custId, suspended, CurrentContext.Ip4Address, CurrentSession.UserId,
                        CurrentSession.UserName, CurrentSession.FromSystem);
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
                        CurrentSession.UserName, CurrentSession.FromSystem);
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
                        CurrentSession.UserName, CurrentSession.FromSystem);
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

        protected static List<KeyValuePair<string, string>> GetProductList()
        {
            return new ProductListAuthorization()
                .GetProductList()
                .Select(product => new KeyValuePair<string, string>(product.Id, product.ProductText))
                .ToList();
        }

        protected static IEnumerable<ProductDetailDto> GetProductDetails(string groupName)
            => new ProductListAuthorization().GetProductDetails(groupName);

        protected int GetDefaultMinBetValue()
            => CurrentSession.IsBelongToSpecialGroups ? 1 : 0;
    }
}