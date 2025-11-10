using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Fanex.ExternalAccount.Core.Context;

namespace Fanex.ExternalAccount.Controllers.UserControl
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (ManageMenu)
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly ICachingService _cachingService;
        private readonly ICurrentSession _currentSession;
        private readonly ICurrentContext _currentContext;

        public MenuController(
            IMenuService menuService,
            ICachingService cachingService,
            ICurrentSession currentSession,
            ICurrentContext currentContext)
        {
            _menuService = menuService;
            _cachingService = cachingService;
            _currentSession = currentSession;
            _currentContext = currentContext;
        }

        [HttpGet]
        [Route("MenuList")]
        public IActionResult MenuList()
        {
            return Ok(new { message = "Menu list view" });
        }

        [HttpPost]
        [Route("MenuListPost")]
        public IActionResult MenuListPost()
        {
            var result = GetAllMenus();

            return Ok(new { total = 1, page = 1, records = result.Count, rows = result });
        }

        [HttpGet]
        [Route("AddMenu")]
        public IActionResult AddMenu()
        {
            var functionList = GetFunctions();
            var menuList = GetMenus();

            return Ok(new { functionList, menuList, model = new Menu() });
        }

        [HttpGet]
        [Route("EditMenu/{id}")]
        public IActionResult EditMenu(int id)
        {
            var model = _menuService.GetAllMenus().FirstOrDefault(o => o.MenuId == id);

            if (model != null)
            {
                model.MenuName = System.Net.WebUtility.HtmlDecode(model.MenuName);
                model.MenuDesc = System.Net.WebUtility.HtmlDecode(model.MenuDesc);
                model.Url = System.Net.WebUtility.HtmlDecode(model.Url);
            }

            var functionList = GetFunctions();
            var menuList = GetMenus();

            return Ok(new { model, functionList, menuList });
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add([FromBody] Menu menu)
        {
            if (menu != null)
            {
                menu.IsActive = true;
            }
            var result = _menuService.CreateMenu(menu, _currentSession.UserId, _currentContext.Ip4Address);

            return Ok(result);
        }

        [HttpPost]
        [Route("Edit")]
        public IActionResult Edit([FromBody] Menu menu)
        {
            var result = _menuService.UpdateMenu(menu, _currentSession.UserId, _currentContext.Ip4Address);

            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateStatus")]
        public IActionResult UpdateStatus([FromBody] UpdateMenuStatusRequest request)
        {
            var result = _menuService.UpdateMenuStatus(
                request.MenuId,
                request.IsActive,
                _currentSession.UserId);

            return Ok(result);
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete([FromBody] DeleteMenuRequest request)
        {
            var result = _menuService.DeleteMenu(
                request.MenuId,
                _currentSession.UserId);

            return Ok(result);
        }

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

        private List<Menu> GetMenus()
        {
            // TODO: var menuList = _cachingService.GetUserCache(_currentSession.UserId, ExtRequest.GetGroupName()).MenuList;
            var menuList = new List<Menu>(); // Placeholder
            var parents = menuList.Where(x => x.ParentId == 0);
            var list = new List<Menu>();
            foreach (var parent in parents)
            {
                list.Add(parent);
            }

            return list;
        }

        private List<Function> GetFunctions()
        {
            // TODO: var menuList = _cachingService.GetUserCache(_currentSession.UserId, ExtRequest.GetGroupName()).PolicyList;
            var menuList = new List<Function>(); // Placeholder
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

    // TODO: Define proper interfaces and services
    public interface IMenuService
    {
        List<Menu> GetAllMenus();
        object CreateMenu(Menu menu, int userId, string ipAddress);
        object UpdateMenu(Menu menu, int userId, string ipAddress);
        object UpdateMenuStatus(int menuId, bool isActive, int userId);
        object DeleteMenu(int menuId, int userId);
    }

    public class MenuService : IMenuService
    {
        public List<Menu> GetAllMenus()
        {
            // TODO: Implement actual service logic
            return new List<Menu>();
        }

        public object CreateMenu(Menu menu, int userId, string ipAddress)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object UpdateMenu(Menu menu, int userId, string ipAddress)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object UpdateMenuStatus(int menuId, bool isActive, int userId)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object DeleteMenu(int menuId, int userId)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }
    }

    // TODO: Define proper request models
    public class UpdateMenuStatusRequest
    {
        public int MenuId { get; set; }
        public bool IsActive { get; set; }
    }

    public class DeleteMenuRequest
    {
        public int MenuId { get; set; }
    }

    // TODO: Define proper data models
    public class Menu
    {
        public int MenuId { get; set; }
        public int ParentId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string MenuDesc { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}