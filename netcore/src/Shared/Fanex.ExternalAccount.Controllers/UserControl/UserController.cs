namespace Fanex.ExternalAccount.Controllers.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Core.Authorization;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DTO.UserControl;
    using Fanex.Cryptography.Aes;
    using Fanex.ExternalAccount.Controllers.ViewModels;
    using Newtonsoft.Json;
    using Service.Impl.Login;
    using Service.Impl.UserControl;
    using Service.Interface.Login;
    using Tarus.Infrastructure.Configurations.InternalLogin;
    using Utilities;
    using System.Net;
    using Microsoft.AspNetCore.Http;

    public class UserController : BaseController
    {
        private readonly CachingService _cache = new CachingService();
        private readonly IUserService _userService = new UserService();
        private readonly IGroupService groupService = new GroupService();
        private readonly ILoginService loginService = new LoginService();
        private readonly IFunctionService functionService = new FunctionService();
        private readonly AuthorizationService _authorization = new AuthorizationServiceImpl();
        private readonly InternalLoginManager _internalLoginManager;

        public UserController(InternalLoginManager internalLoginManager)
        {
            _internalLoginManager = internalLoginManager;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public IActionResult UserList()
        {
            var groups = groupService.GetListGroup();
            if (CurrentSession.IsITAdmin)
            {
                ViewBag.GroupList = groups;
            }
            else if (CurrentSession.IsInternalAdmin)
            {
                ViewBag.GroupList = groups.Where(o => !Group.IsITGroup(o.GroupID));
            }
            else
            {
                ViewBag.GroupList = groups.Where(o => o.GroupName == CurrentSession.CurrencyGroup);
            }

            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public IActionResult UserListPost(string userName = null, int? groupId = null)
        {
            var result = _userService.GetListUsers(userName, groupId);

            if (!CurrentSession.IsITAdmin && CurrentSession.IsInternalAdmin)
            {
                result = result.Where(o => !Group.IsITGroup(o.GroupID));
            }
            else
            {
                if (!CurrentSession.IsITAdmin && !CurrentSession.IsInternalAdmin)
                {
                    result = result.Where(o => o.GroupName == CurrentSession.CurrencyGroup);
                }
            }

            return JsonBase(new { total = 1, page = 1, records = result.Count(), rows = result },
                new JsonSerializerSettings());
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(int id)
        {
            var model = _userService.GetUserById(id);

            model.FirstName = WebUtility.HtmlDecode(model.FirstName);
            model.LastName = WebUtility.HtmlDecode(model.LastName);

            var groups = groupService.GetListGroup();
            if (CurrentSession.IsITAdmin)
            {
                ViewBag.GroupList = groups;
            }
            else if (CurrentSession.IsInternalAdmin)
            {
                ViewBag.GroupList = groups.Where(o => !Group.IsITGroup(o.GroupID));
            }
            else
            {
                ViewBag.GroupList = groupService.GetExternalGroup();
            }

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int custId, string firstName, string lastName, int groupId)
        {
            var result = _userService.EditUser(CurrentSession.UserId, custId, firstName, lastName,
                CurrentContext.Ip4Address, groupId);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser()
        {
            return PartialViewBase(GetUserModel());
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public IActionResult AddExistUser()
        {
            return PartialViewBase(GetUserModel());
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult Add(string username, string pwd, string firstName, string lastName, int groupId)
        {
            var result = _userService.CreateUser(CurrentSession.UserId, username, pwd, firstName, lastName, groupId,
                CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult AddExist(string username, int groupId)
        {
            var result =
                _userService.CreateExistUser(CurrentSession.UserId, username, groupId, CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int userId, bool isActive)
        {
            var result = _userService.UpdateStatus(CurrentSession.UserId, userId, isActive, CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPwd(int userId, string pwd)
        {
            var result = _userService.ResetPassword(CurrentSession.UserId, userId, pwd, CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(int id)
        {
            var model = _userService.GetUserById(id);

            ViewBag.IsInternalAdmin = loginService.CheckExternalGroup(id, null, eFunctions.ExternalGroup);

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public IActionResult GrantPermission(int userId, string username, string groupName)
        {
            var funcs = functionService.GetAllFunctions().ToList();
            funcs = funcs.Where(f => f.IsTest).ToList();
            var groups = groupService.GetListGroup();
            var externalGroup = groupService.GetExternalGroup();
            var functionList = _userService.GetUserFunction(userId);
            var allowFunctions = new List<Function>();

            if (!IncludeAllFunctions)
            {
                foreach (var function in functionList)
                {
                    if (funcs.All(f => f.FuncID != function.FuncID))
                    {
                        allowFunctions.Add(function);
                    }
                }

                functionList = allowFunctions;
            }

            var groupId = groups
                .FirstOrDefault(o => o.GroupName.Equals(ExtRequest.GetGroupName(groupName), StringComparison.OrdinalIgnoreCase))?.GroupID;

            ViewBag.UserID = userId;
            ViewBag.Username = username;

            var isExternalGroup =
                externalGroup.Any(i => i.GroupName.Equals(ExtRequest.GetGroupName(groupName), StringComparison.OrdinalIgnoreCase));

            if (isExternalGroup)
            {
                ViewBag.UserFunc = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ManageFunction
                                                           && x.FuncKey != eFunctions.ManageGroup
                                                           && x.FuncKey != eFunctions.ManageMenu
                                                           && x.FuncKey != eFunctions.ManageUser
                                                           && x.FuncKey != eFunctions.Administration
                                                           && x.FuncKey != eFunctions.SystemConfiguration
                                                           && x.FuncKey != eFunctions.ExternalGroup);

                ViewBag.UserSubFunc = functionList.Where(x =>
                    x.ParentID != 0 && x.FuncKey != eFunctions.ManageFunction
                    && x.FuncKey != eFunctions.ManageGroup && x.FuncKey != eFunctions.ManageMenu
                    && x.FuncKey != eFunctions.ManageUser
                    && x.FuncKey != eFunctions.Administration && x.FuncKey != eFunctions.SystemConfiguration
                    && x.FuncKey != eFunctions.ExternalGroup);
            }
            else if (Group.IsITGroup(groupId ?? 0))
            {
                ViewBag.UserFunc = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ExternalGroup);
                ViewBag.UserSubFunc = functionList.Where(x => x.ParentID != 0 && x.FuncKey != eFunctions.ExternalGroup);
            }
            else
            {
                ViewBag.UserFunc = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ManageFunction
                                                           && x.FuncKey != eFunctions.ManageMenu
                                                           && x.FuncKey != eFunctions.SystemConfiguration
                                                           && x.FuncKey != eFunctions.ExternalGroup);

                ViewBag.UserSubFunc = functionList.Where(x =>
                    x.ParentID != 0 && x.FuncKey != eFunctions.ManageFunction
                    && x.FuncKey != eFunctions.ManageMenu && x.FuncKey != eFunctions.SystemConfiguration
                    && x.FuncKey != eFunctions.ExternalGroup);
            }

            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult GrantPermissionPost(int userId, string oldPolicy, string policy)
        {
            var result = _userService.UpdateUserPolicy(CurrentSession.CustId, userId, oldPolicy, policy,
                CurrentContext.Ip4Address);
            if (result.IsSuccess)
            {
                _cache.RemoveUserCache(CurrentSession.UserId);
            }

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.InternalLogin,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public IActionResult InternalLogin(string username, string groupName)
        {
            var url = Url.Content("~") + "Shared/NoPermission";
            var checkCustomerBelongToSpecialSiteResult = CheckCustomerBelongToSpecialSite(username);
            var isBelonging = _authorization.IsBelong(ExtRequest.GetGroupName(groupName), 0, username);

            if (isBelonging)
            {
                var param = string.Join("&", username, CurrentSession.UserId, loginService.GetDbTime().Ticks, "ExtNAP");
                param = AesExtension.Encrypt(param, "!@#$");

                var CustomerSite = checkCustomerBelongToSpecialSiteResult.Site;
                var internalAgentUrl = _internalLoginManager.GetInternalLoginUrl(CustomerSite);
                var internalPaths = internalAgentUrl.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                var internalDomains = internalPaths[0].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var internalLoginAction = internalPaths[1];
                var configDomains = new List<InternLoginDomain>();
                string randomDomain = null;
                string internalLoginSessionKey = $"InternalLoginDomains_{CustomerSite}";

                if (HttpContext.Session.GetString(internalLoginSessionKey) != null)
                {
                    configDomains = JsonConvert.DeserializeObject<List<InternLoginDomain>>(HttpContext.Session.GetString(internalLoginSessionKey));
                    randomDomain = configDomains.FirstOrDefault(domain => !domain.IsInUsed)?.Domain;
                    if (randomDomain == null)
                    {
                        randomDomain = internalDomains[new Random().Next(internalDomains.Length)];
                    }
                }
                else
                {
                    foreach (var domain in internalDomains)
                    {
                        configDomains.Add(new InternLoginDomain { Domain = domain });
                    }
                }

                var foundDomain = configDomains.Find(item => item.Domain == randomDomain);
                if (foundDomain != null)
                {
                    foundDomain.IsInUsed = true;
                }

                HttpContext.Session.SetString(internalLoginSessionKey, JsonConvert.SerializeObject(configDomains));

                randomDomain = FormatInternalLoginDomain(randomDomain);
                var internalDomain = randomDomain.StartsWith("http") ? $"{randomDomain}{internalLoginAction}" : $"https://{randomDomain}{internalLoginAction}";

                url = $"{internalDomain}?{(HttpContext.Session.GetString(Config.InternalAgentUMToken) == null ? string.Empty : $"UMToken={HttpContext.Session.GetString(Config.InternalAgentUMToken)}&")}parameter={param}";
            }

            return Redirect(url);
        }
     
        public IActionResult ResetPwdOwner(int userId, string pwd)
        {
            if (userId != CurrentSession.UserId)
            {
                return ActionDenied();
            }

            var result = _userService.ResetPassword(CurrentSession.UserId, userId, pwd, CurrentContext.Ip4Address);

            return JsonBase(result);
        }
    
        public IActionResult ResetPasswordOwner(int id)
        {
            if (id != CurrentSession.UserId)
            {
                return ActionDenied();
            }

            var model = _userService.GetUserById(id);
            ViewBag.IsInternalAdmin = loginService.CheckExternalGroup(id, null, eFunctions.ExternalGroup);
            return PartialViewBase(model);
        }

        private User GetUserModel()
        {
            var model = new User();
            var groups = groupService.GetListGroup();
            if (CurrentSession.IsITAdmin)
            {
                ViewBag.GroupList = groups;
            }
            else if (CurrentSession.IsInternalAdmin)
            {
                ViewBag.GroupList = groups.Where(o => !Group.IsITGroup(o.GroupID));
            }
            else
            {
                ViewBag.GroupList = groups.Where(o => o.GroupName == CurrentSession.CurrencyGroup);
            }

            return model;
        }

        private string FormatInternalLoginDomain(string configDomain)
        {
            return Request.Headers["Host"].Contains("oneops.net") ?
                string.Format(configDomain, "gatonic.com") :
                string.Format(configDomain, HttpContext.Request.Headers["Host"]
                                                    .ToString()
                                                    .Replace("www.", string.Empty).ToString()
                                                    .Replace("staging.", string.Empty).ToString()
                                                    .Replace("h1.", string.Empty).ToString()
                                                    .Replace("h2.", string.Empty).ToString()
                                                    .Replace("h3.", string.Empty).ToString());
        }
    }
}