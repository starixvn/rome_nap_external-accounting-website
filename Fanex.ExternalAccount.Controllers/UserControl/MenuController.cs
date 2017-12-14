namespace Fanex.ExternalAccount.Controllers.UserControl
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using CustomerSetting.Security.Authenticate;
    using DTO.UserControl;
    using Newtonsoft.Json;
    using Service.Impl.UserControl;
    using Service.Interface.UserControl;

    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;

        public MenuController()
        {
            _menuService = _menuService ?? new MenuService();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult MenuList() => View();

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult MenuListPost()
        {
            var result = GetAllMenus();

            return JsonBase(new {total = 1, page = 1, records = result.Count, rows = result},
                new JsonSerializerSettings());
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult AddMenu()
        {
            ViewBag.FunctionList = GetFunctions();
            ViewBag.MenuList = GetMenus();

            return PartialViewBase(new Menu());
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult EditMenu(int id)
        {
            var model = _menuService.GetAllMenus().FirstOrDefault(o => o.MenuId == id);

            if (model != null)
            {
                model.MenuName = HttpUtility.HtmlDecode(model.MenuName);
                model.MenuDesc = HttpUtility.HtmlDecode(model.MenuDesc);
                model.Url = HttpUtility.HtmlDecode(model.Url);

                ViewBag.FunctionList = GetFunctions();
                ViewBag.MenuList = GetMenus();
            }

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult Add(string name, string description, string url, int functionId, int parentId,
            string parentName, int displayOrder)
        {
          
            var result = _menuService.CreateMenu(
                name,
                description,
                url,
                functionId,
                parentId,
                parentName,
                displayOrder,
                true,
                CurrentSession.CustId,
                CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult Edit(int menuId, string name, string description, string url, int functionId, int parentId,
            string parentName, int displayOrder)
        {
            var result = _menuService.UpdateMenu(
                menuId,
                name,
                description,
                url,
                functionId,
                parentId,
                parentName,
                displayOrder,
                CurrentSession.CustId,
                CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateStatus(int menuId, bool isActive)
        {
            var result = _menuService.UpdateMenuStatus(
                menuId,
                isActive,
                CurrentSession.CustId);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult Delete(int menuId)
        {
            var result = _menuService.DeleteMenu(
                menuId,
                CurrentSession.CustId);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        private List<Menu> GetAllMenus()
        {
            var menuList = _menuService.GetAllMenus();
            var parents = menuList.Where(x => x.ParentId == 0);
            var list = new List<Menu>();
            foreach (var parent in parents)
            {
                list.Add(parent);
                var items = menuList.Where(x => x.ParentId != 0 && x.ParentId == parent.MenuId);
                if (items.Any())
                {
                    list.AddRange(items);
                }
            }

            return list;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        private List<Menu> GetMenus()
        {
            var menuList = new CachingService().GetUserCache(CurrentSession.UserId).MenuList;
            var parents = menuList.Where(x => x.ParentId == 0);
            var list = new List<Menu>();
            foreach (var parent in parents)
            {
                list.Add(parent);
            }

            return list;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        private List<Function> GetFunctions()
        {
            var menuList = new CachingService().GetUserCache(CurrentSession.UserId).PolicyList;
            var parents = menuList.Where(x => x.ParentID == 0);
            var list = new List<Function>();
            foreach (var parent in parents)
            {
                list.Add(parent);
                var items = menuList.Where(x => x.ParentID != 0 && x.ParentID == parent.FuncID);
                if (items.Any())
                {
                    list.AddRange(items);
                }
            }

            return list;
        }
    }
}