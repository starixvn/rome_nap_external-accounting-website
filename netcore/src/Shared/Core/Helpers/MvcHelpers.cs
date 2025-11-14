namespace Fanex.ExternalAccount.Core.Helpers
{
    using System.IO;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Mvc.Razor;
    using ITempDataProvider = Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider;

    public static class MvcHelpers
    {
        public static string RenderRazorViewToString(this Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;

            var serviceProvider = controller.HttpContext.RequestServices;
            var razorViewEngine = serviceProvider.GetService<IRazorViewEngine>();
            var tempDataProvider = serviceProvider.GetService<ITempDataProvider>();

            using (var sw = new StringWriter())
            {
                var actionContext = new ActionContext(controller.HttpContext, controller.RouteData, controller.ControllerContext.ActionDescriptor);
                var viewResult = razorViewEngine.FindView(actionContext, viewName, false);

                if (!viewResult.Success)
                {
                    throw new FileNotFoundException($"View '{viewName}' not found. Locations searched: {string.Join(", ", viewResult.SearchedLocations ?? new string[0])}");
                }

                var viewDictionary = new ViewDataDictionary(controller.ViewData)
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();

                return sw.ToString();
            }
        }
    }
}
