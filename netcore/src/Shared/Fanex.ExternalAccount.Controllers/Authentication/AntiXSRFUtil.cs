namespace Fanex.ExternalAccount.Controllers.Authentication
{
    using System;
    using System.Diagnostics;
    using System.Web;
    using System.Web.Helpers;
    using Logging.Extensions.Web;
    using Utilities;

    public static class AntiXSRFUtil
    {
        public static bool ValidateAntiXSRF(HttpRequestBase Request, string token)
        {
            try
            {
                var request = Request;
                var formToken = token;

                if (string.IsNullOrWhiteSpace(formToken))
                {
                    return false;
                }

                var cookieValue = request.Cookies[AntiForgeryConfig.CookieName];
                if (!string.IsNullOrWhiteSpace(cookieValue?.Value))
                {
                    AntiForgery.Validate(cookieValue.Value, formToken);
                }
                else
                {
                    var exceptionLog = new Exception(ViewLang.CannotReadRequestValidationCookie);
                    ExceptionHandler.Handle(exceptionLog, TraceEventType.Warning);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, TraceEventType.Warning);

                return false;
            }

            return true;
        }

    }
}