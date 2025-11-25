namespace Fanex.ProductStatus.Setting.Web
{
    using System;
    using Microsoft.AspNetCore.Antiforgery;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal class ProductStatusValidateAntiForgeryTokenAttribute : ActionFilterAttribute
    {
        private const string AntiForgeryFormTokenName = "__RequestVerificationToken";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var antiforgery = context.HttpContext.RequestServices.GetService(typeof(IAntiforgery)) as IAntiforgery;
            if (antiforgery != null)
            {
                var request = context.HttpContext.Request;
                var formToken = request.Headers[AntiForgeryFormTokenName].ToString();

                if (!string.IsNullOrWhiteSpace(formToken))
                {
                    var cookieValue = request.Cookies["__RequestVerificationToken"];
                    antiforgery.ValidateRequestAsync(context.HttpContext).Wait();
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
