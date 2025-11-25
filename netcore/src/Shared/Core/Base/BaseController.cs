namespace Fanex.ExternalAccount.Core.Base
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Authorization;
    using Context;
    using Fanex.ExternalAccount.Core.Criteria;
    using Fanex.ExternalAccount.Core.Model;
    using Fanex.ExternalAccount.Core.Service;
    using Utilities;
    using CurrentContext = Context.CurrentContext;
    using CurrentSession = Context.CurrentSession;
    using Newtonsoft.Json;

    public class BaseController : Controller
    {
        protected const string SiteName = "ExtAcc";
        protected const string UAT = "UAT";
        protected const string PRO = "LIVE";
        private readonly ICurrentContext currentContext;
        private readonly ICurrentSession currentSession;
        private readonly IExtRequest extRequest;
        private readonly IConfiguration configuration;

        protected readonly AuthorizationService AuthorizationService;

        protected BaseController(ICurrentSession currentSession, ICurrentContext currentContext, IExtRequest extRequest, IConfiguration configuration, AuthorizationService authorizationService)
        {
            this.currentContext = currentContext;
            this.currentSession = currentSession;
            this.extRequest = extRequest;
            this.configuration = configuration;
            this.AuthorizationService = authorizationService;
        }

        protected string Environment => configuration?.GetValue<string>("Environment") ?? string.Empty;

        protected bool IncludeAllFunctions => Environment.Equals(UAT, StringComparison.OrdinalIgnoreCase);

        protected virtual ICurrentContext CurrentContext => currentContext ?? new CurrentContext();

        protected virtual ICurrentSession CurrentSession => currentSession ?? new CurrentSession();

        protected virtual IExtRequest ExtRequest => extRequest ?? new ExtRequest();

        protected virtual IActionResult JsonBase(object data, JsonSerializerSettings settings = null)
        {
            return new JsonBase(data, settings ?? new JsonSerializerSettings());
        }

        protected virtual IActionResult JsonBase2(object data) => new JsonBase(data);

        protected virtual IActionResult PartialViewBase(object objModel = null, string viewName = null)
        {
            // For MVC controllers, return the partial view
            // For API controllers that somehow inherit from this base, return JSON
            if (this is Controller controller)
            {
                return viewName != null ? controller.PartialView(viewName, objModel) : controller.PartialView(objModel);
            }

            // Fallback for API contexts - return JSON data
            return new JsonBase(objModel ?? new { });
        }

        protected virtual void ExportToExcel(MemoryStream excelStream, string fileName)
        {
            if (!Response.HasStarted)
            {
                Response.ContentType = "application/vnd.ms-excel";
                Response.Headers.Add("Content-Disposition", $"attachment;filename={fileName}");
            }

            // In ASP.NET Core, we don't need to clear response explicitly
            excelStream.Position = 0;
            excelStream.CopyTo(Response.Body);
        }

        protected IActionResult ActionDenied()
        {
            // Check if it's an AJAX request
            var isAjaxRequest = Request.Headers["X-Requested-With"].ToString() == "XMLHttpRequest";

            if (isAjaxRequest)
            {
                return Ok(new
                {
                    errorCode = 1,
                    errorMessage = ViewLang.DontHavePermission,
                    isSuccess = false
                });
            }

            // ASP.NET Core API controllers don't support views
            return Forbid("Access denied");
        }

        protected CheckCustomerBelongToSpecialSiteResult CheckCustomerBelongToSpecialSite(string username)
        {
            var permissionService = new ServiceBase();
            return permissionService.CheckCustomerBelongToSpecialSite(
                new CheckCustomerIsSpecialSiteCommand { UserName = username });
        }
    }
}
