namespace Fanex.ExternalAccount.Core.Base
{
    using Authorization;
    using Context;
    using CustomerSetting.Module.Core.Criteria;
    using CustomerSetting.Module.Core.Models;
    using CustomerSetting.Module.Core.Services.Impl;
    using CustomerSetting.Module.Core.Web;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Configuration;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using Utilities;
    using CurrentContext = Context.CurrentContext;
    using CurrentSession = Context.CurrentSession;

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

        protected CheckCustomerBelongToSpecialSiteResult CheckCustomerBelongToSpecialSite(string username)
        {
            var permissionService = new ServiceBase();
            return permissionService.CheckCustomerBelongToSpecialSite(
                new CheckCustomerIsSpecialSiteCriteria { UserName = username });
        }
       
    }
}