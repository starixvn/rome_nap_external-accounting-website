namespace Fanex.ExternalAccount.Controllers.UserControl
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DTO.UserControl;
    using Service.Impl.UserControl;
    using Service.Interface.UserControl;

    public class PermissionController : BaseController
    {
        private readonly IFunctionService functionService;

        public PermissionController()
        {
            functionService = new FunctionService();
        }

        [ExternalAccountAuthenticate(
            Function = eFunctions.ManageFunction,
            Policy = AuthenticateAction.AuthenticationType.Read,
            IsAjaxRequest = false)]
        public ActionResult Index()
        {
            var funcs = functionService.GetAllFunctions();
            var parents = funcs.Where(f => f.ParentID == 0);

            if (!CurrentSession.IsITAdmin)
            {
                if (CurrentSession.IsInternalAdmin)
                {
                    funcs = funcs.Where(f => f.FuncKey != eFunctions.ManageMenu
                                             && f.FuncKey != eFunctions.ManageFunction);

                    parents = parents.Where(f => f.FuncKey != eFunctions.SystemConfiguration);
                }
                else
                {
                    funcs = funcs.Where(f => f.FuncKey != eFunctions.ManageMenu
                                             && f.FuncKey != eFunctions.ManageFunction
                                             && f.FuncKey != eFunctions.ManageGroup
                                             && f.FuncKey != eFunctions.ManageUser);

                    parents = parents.Where(f => f.FuncKey != eFunctions.Administration
                                                 && f.FuncKey != eFunctions.SystemConfiguration
                                                 && f.FuncKey != eFunctions.ExternalGroup);
                }
            }

            IList<Function> funcsOrder = new List<Function>();

            foreach (var item in parents)
            {
                funcsOrder.Add(item);

                foreach (var child in funcs)
                {
                    if (child.ParentID != 0 && item.FuncID == child.ParentID)
                    {
                        funcsOrder.Add(child);
                    }
                }
            }

            return PartialViewBase(funcsOrder);
        }
    }
}
