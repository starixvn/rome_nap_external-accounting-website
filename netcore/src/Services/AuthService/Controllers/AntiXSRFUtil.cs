using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using Fanex.ExternalAccount.Utilities;
using Logging.Extensions.Web;

namespace Fanex.ExternalAccount.Controllers.Authentication
{
    public static class AntiXSRFUtil
    {
        public static bool ValidateAntiXSRF(HttpRequest request, string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return false;
                }

                // Get cookie value from request
                var cookieValue = request.Cookies["__RequestVerificationToken"];
                if (!string.IsNullOrWhiteSpace(cookieValue))
                {
                    // In ASP.NET Core, we use IAntiforgery service
                    // For now, we'll do a simple validation
                    // TODO: Implement proper antiforgery validation using IAntiforgery service
                    return !string.IsNullOrWhiteSpace(token);
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
