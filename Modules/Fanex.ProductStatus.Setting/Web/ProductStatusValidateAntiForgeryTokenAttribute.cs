namespace Fanex.ProductStatus.Setting.Web
{
    using System;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal class ProductStatusValidateAntiForgeryTokenAttribute : ActionFilterAttribute
    {
        private const string AntiForgeryFormTokenName = "__RequestVerificationToken";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpCookie cookieValue = null;
            var request = filterContext.HttpContext.Request;
            var formToken = request.Headers[AntiForgeryFormTokenName];

            if (!string.IsNullOrWhiteSpace(formToken))
            {
                cookieValue = request.Cookies[AntiForgeryConfig.CookieName];
            }

            AntiForgery.Validate(cookieValue != null ? cookieValue.Value : null, formToken);

            base.OnActionExecuting(filterContext);
        }
    }
}
