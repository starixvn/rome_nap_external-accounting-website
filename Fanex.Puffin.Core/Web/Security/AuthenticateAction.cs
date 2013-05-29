using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Fanex.NAP.Core.Model;
using Fanex.Puffin.Core.Models;

namespace Fanex.Puffin.Core.Web.Security
{
    public class AuthenticateAction : ActionFilterAttribute
    {
        private readonly AuthenticationType _authenticationType;

        private readonly string _mainAction;

        public enum AuthenticationType
        {
            Read,
            Host
        }

        public AuthenticateAction(AuthenticationType authenticationType, string mainAction)
        {
            _authenticationType = authenticationType;
            _mainAction = mainAction;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controlName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
         

            var pathActionName = controlName + "/" + _mainAction;

            bool hasPermission = false;

            var actionPrivilege = CheckPrivilege(pathActionName);

            switch (_authenticationType)
            {
                case AuthenticationType.Read:
                    if (actionPrivilege < 1)
                    {
                        ActionDenied(filterContext);
                        return;
                    }
                    break;
                case AuthenticationType.Host:
                    if (actionPrivilege < 2)
                    {
                        var error = new Result();
                        error.ErrorCode = 500;
                        error.ErrorMessage = Resources.Errors._500;
                        filterContext.Result = new JsonResult {Data = error};
                        return;

                    }
                    break;
            }


            base.OnActionExecuting(filterContext);
        }

        private static void ActionDenied(ActionExecutingContext filterContext)
        {
           
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
            {
                area = "",
                controller = "Common",
                action = "AccessDenied"
                
            }));
        }

        



        private int CheckPrivilege(string fullActionPath)
        {
            var privilege = 0;

            if (string.IsNullOrEmpty(fullActionPath))
                return 0;

            var useMenu = AppSession.CurrentUserMenu;

            if (useMenu == null || useMenu.Count == 0)
            {
                return 0;
            }

            var result = useMenu.Where(menuItem => menuItem.Url.Trim().ToLower().Contains(fullActionPath.Trim().ToLower()));


            var s = result as List<MenuItem> ?? result.ToList();
            if (s.Any())
            {
                var firstOrDefault = s.FirstOrDefault();
                if (firstOrDefault != null) privilege = firstOrDefault.Policy;
            }

            return privilege;
        }

        

    }
}
