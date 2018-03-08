namespace Fanex.ExternalAccount.Controllers.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DTO.UserControl;
    using Newtonsoft.Json;
    using RSACryptoPad;
    using Service.Impl.Login;
    using Service.Impl.UserControl;
    using Service.Interface.Login;
    using Service.Interface.UserControl;
    using Utilities;

    public class UserController : BaseController
    {
        private readonly CachingService _cache = new CachingService();
        private readonly IUserService _userService = new UserService();
        private readonly IGroupService groupService = new GroupService();
        private readonly ILoginService loginService = new LoginService();
        private readonly IFunctionService functionService = new FunctionService();

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult UserList()
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
        [AntiXSRFFilter]
        public ActionResult UserListPost(string userName = null, int? groupId = null)
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
        [AntiXSRFFilter]
        public ActionResult EditUser(int id)
        {
            var model = _userService.GetUserById(id);

            model.FirstName = HttpUtility.HtmlDecode(model.FirstName);
            model.LastName = HttpUtility.HtmlDecode(model.LastName);

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
                ViewBag.GroupList = loginService.GroupList();
            }

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult Edit(int custId, string firstName, string lastName, int groupId)
        {
            var result = _userService.EditUser(CurrentSession.UserId, custId, firstName, lastName,
                CurrentContext.Ip4Address, groupId);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult AddUser()
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

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult AddExistUser()
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

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult Add(string username, string pwd, string firstName, string lastName, int groupId)
        {
            var result = _userService.CreateUser(CurrentSession.UserId, username, pwd, firstName, lastName, groupId,
                CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult AddExist(string username, int groupId)
        {
            var result =
                _userService.CreateExistUser(CurrentSession.UserId, username, groupId, CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateStatus(int userId, bool isActive)
        {
            var result = _userService.UpdateStatus(CurrentSession.UserId, userId, isActive, CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult ResetPwd(int userId, string pwd)
        {
            var result = _userService.ResetPassword(CurrentSession.UserId, userId, pwd, CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult ResetPassword(int id)
        {
            var model = _userService.GetUserById(id);

            ViewBag.IsInternalAdmin = loginService.CheckExternalGroup(id, null, eFunctions.ExternalGroup);

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult GrantPermission(int userId, string username, string groupName)
        {
            var funcs = functionService.GetAllFunctions().ToList();
            funcs = funcs.Where(f => f.IsTest).ToList();
            var groups = groupService.GetListGroup();
            var externalGroup = loginService.GroupList();
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
                .FirstOrDefault(o => o.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase))?.GroupID;

            ViewBag.UserID = userId;
            ViewBag.Username = username;

            var isExternalGroup =
                externalGroup.Any(i => i.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase));

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
        [AntiXSRFFilter]
        public ActionResult GrantPermissionPost(int userId, string oldPolicy, string policy)
        {
            var result = _userService.UpdateUserPolicy(CurrentSession.CustId, userId, oldPolicy, policy,
                CurrentContext.Ip4Address);
            if (result.IsSuccess)
            {
                _cache.RemoveUserCache(CurrentSession.UserId);
            }

            return JsonBase(result);
        }

        public ActionResult InternalLogin(string username)
        {
            string url;
            var checkCustomerBelongToSpecialSiteResult = CheckCustomerBelongToSpecialSite(username);
            if (IsAdminHasPermissionOnCustomer(checkCustomerBelongToSpecialSiteResult.IsSpecialSite))
            {
                var encryption = new EncryptionThread();
                var param = string.Join("&", username, CurrentSession.UserId, loginService.GetDbTime().Ticks,
                    "NAPEXT");
                param = encryption.EncryptString(param, 1024, Request.PhysicalApplicationPath + "/IbcKeys/ibcpkey.snk");

                url =
                    $"{GetInternalLoginUrl((AlphaSiteCode)checkCustomerBelongToSpecialSiteResult.SiteCode)}" +
                    $"?{(Session[Config.InternalAgentUMToken] == null ? string.Empty : $"UMToken={Session[Config.InternalAgentUMToken]}&")}params={param}";
            }
            else
            {
                url = Url.Content("~") + "Shared/NoPermission";
            }

            return Redirect(url);
        }

        private string GetInternalLoginUrl(AlphaSiteCode siteCode)
        {
            var domain = CurrentContext.DomainName
                .Replace("www.", string.Empty)
                .Replace("staging.", string.Empty);
            var domainSpliters = domain.Split('.', ':');
            var ip = 0;
            var isDomainRequest = domainSpliters.Any(i => !int.TryParse(i, out ip));
            domain = isDomainRequest ? domain : "focapts.com";
            switch (siteCode)
            {
                case AlphaSiteCode.I7:
                    return string.Format(Config.InternalLoginUrl7, domain);
                case AlphaSiteCode.I9:
                    return string.Format(Config.InternalLoginUrl9, domain);
                default:
                    return string.Format(Config.InternalLoginUrl, domain);
            }
        }

        [AntiXSRFFilter]
        public ActionResult ResetPwdOwner(int userId, string pwd)
        {
            if (userId != CurrentSession.UserId)
            {
                return ActionDenied(Request);
            }

            var result = _userService.ResetPassword(CurrentSession.UserId, userId, pwd, CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        public ActionResult ResetPasswordOwner(int id)
        {
            if (id != CurrentSession.UserId)
            {
                return ActionDenied(Request);
            }

            var model = _userService.GetUserById(id);
            ViewBag.IsInternalAdmin = loginService.CheckExternalGroup(id, null, eFunctions.ExternalGroup);
            return PartialViewBase(model);
        }

        private enum AlphaSiteCode
        {
            I1 = 1,
            I7 = 7,
            I9 = 9
        }
    }
}