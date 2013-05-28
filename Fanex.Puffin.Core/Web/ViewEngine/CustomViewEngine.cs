using System.Web.Mvc;

namespace Fanex.Puffin.Core.Web.ViewEngine
{

    public class CustomViewEngine : RazorViewEngine
    {
        public CustomViewEngine()
            : base()
        {

            PartialViewLocationFormats = new string[]
                {
                    "~/Areas/{0}.cshtml"
                    //Other areas
                };
        }



        /*public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {


            ViewEngineResult areaResult;

            if (controllerContext.RouteData.DataTokens.ContainsKey("area"))
            {
                string areaPartialName = FormatViewName(controllerContext, partialViewName); 

                List<string> views = new List<string>();
                views.Add(areaPartialName);

                return  new ViewEngineResult(views);
                areaResult = base.FindPartialView(controllerContext, areaPartialName, false);
                if (areaResult != null && areaResult.View != null)
                {
                    return areaResult;
                }
                string sharedAreaPartialName = FormatSharedViewName(controllerContext, partialViewName) ;
                areaResult = base.FindPartialView(controllerContext, sharedAreaPartialName, false);
                if (areaResult != null && areaResult.View != null)
                {
                    return areaResult;
                }
            }

            return base.FindPartialView(controllerContext, partialViewName, useCache);
            
        }

        private static string FormatViewName(ControllerContext controllerContext, string viewName)
        {
            string controllerName = controllerContext.RouteData.GetRequiredString("controller");

            string area = controllerContext.RouteData.DataTokens["area"].ToString();
            // Compared to the original sources (http://haacked.com/archive/2008/11/04/areas-in-aspnetmvc.aspx),
            // we removed the Areas prefix from the path 
            return "~/" + AreasLocation + "/" +  area + "/Views/" + controllerName + "/" + viewName + ".cshtml";
        }

        private static string FormatSharedViewName(ControllerContext controllerContext, string viewName)
        {
            string area = controllerContext.RouteData.DataTokens["area"].ToString();
            // Compared to the original sources (http://haacked.com/archive/2008/11/04/areas-in-aspnetmvc.aspx),
            // we removed the Areas prefix from the path 
            return "~/" + AreasLocation + "/" + area + "/Views/Shared/" + viewName + ".cshtml";
        }*/
    }
}
