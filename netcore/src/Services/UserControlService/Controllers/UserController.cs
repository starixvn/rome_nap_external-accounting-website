using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Fanex.ExternalAccount.Core.Context;

namespace Fanex.ExternalAccount.Controllers.UserControl
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (ManageUser)
    public class UserController : ControllerBase
    {
        private readonly ICachingService _cache;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly ILoginService _loginService;
        private readonly IFunctionService _functionService;
        private readonly IAuthorizationService _authorization;
        private readonly ICurrentSession _currentSession;
        private readonly ICurrentContext _currentContext;

        public UserController(
            ICachingService cache,
            IUserService userService,
            IGroupService groupService,
            ILoginService loginService,
            IFunctionService functionService,
            IAuthorizationService authorization,
            ICurrentSession currentSession,
            ICurrentContext currentContext)
        {
            _cache = cache;
            _userService = userService;
            _groupService = groupService;
            _loginService = loginService;
            _functionService = functionService;
            _authorization = authorization;
            _currentSession = currentSession;
            _currentContext = currentContext;
        }

        [HttpGet]
        [Route("UserList")]
        public IActionResult UserList()
        {
            var groups = _groupService.GetListGroup();
            object groupList = null!;

            if (_currentSession.IsITAdmin)
            {
                groupList = groups;
            }
            else if (_currentSession.IsInternalAdmin)
            {
                groupList = groups.Where(o => !Group.IsITGroup(o.GroupID));
            }
            else
            {
                groupList = groups.Where(o => o.GroupName == _currentSession.CurrencyGroup);
            }

            return Ok(new { groupList });
        }

        [HttpPost]
        [Route("UserListPost")]
        public IActionResult UserListPost([FromQuery] string userName = null, [FromQuery] int? groupId = null)
        {
            var result = _userService.GetListUsers(userName, groupId);

            if (!_currentSession.IsITAdmin && _currentSession.IsInternalAdmin)
            {
                result = result.Where(o => !Group.IsITGroup(o.GroupID));
            }
            else
            {
                if (!_currentSession.IsITAdmin && !_currentSession.IsInternalAdmin)
                {
                    result = result.Where(o => o.GroupName == _currentSession.CurrencyGroup);
                }
            }

            return Ok(new { total = 1, page = 1, records = result.Count(), rows = result });
        }

        [HttpGet]
        [Route("EditUser/{id}")]
        public IActionResult EditUser(int id)
        {
            var model = _userService.GetUserById(id);

            model.FirstName = System.Net.WebUtility.HtmlDecode(model.FirstName);
            model.LastName = System.Net.WebUtility.HtmlDecode(model.LastName);

            var groups = _groupService.GetListGroup();
            object groupList = null!;

            if (_currentSession.IsITAdmin)
            {
                groupList = groups;
            }
            else if (_currentSession.IsInternalAdmin)
            {
                groupList = groups.Where(o => !Group.IsITGroup(o.GroupID));
            }
            else
            {
                groupList = _groupService.GetExternalGroup();
            }

            return Ok(new { model, groupList });
        }

        [HttpPost]
        [Route("Edit")]
        public IActionResult Edit([FromBody] EditUserRequest request)
        {
            var result = _userService.EditUser(_currentSession.UserId, request.CustId, request.FirstName, request.LastName,
                _currentContext.Ip4Address, request.GroupId);

            return Ok(result);
        }

        [HttpGet]
        [Route("AddUser")]
        public IActionResult AddUser()
        {
            var model = GetUserModel();
            return Ok(new { model });
        }

        [HttpGet]
        [Route("AddExistUser")]
        public IActionResult AddExistUser()
        {
            var model = GetUserModel();
            return Ok(new { model });
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add([FromBody] AddUserRequest request)
        {
            var result = _userService.CreateUser(_currentSession.UserId, request.Username, request.Pwd, request.FirstName, request.LastName, request.GroupId,
                _currentContext.Ip4Address);

            return Ok(result);
        }

        [HttpPost]
        [Route("AddExist")]
        public IActionResult AddExist([FromBody] AddExistUserRequest request)
        {
            var result =
                _userService.CreateExistUser(_currentSession.UserId, request.Username, request.GroupId, _currentContext.Ip4Address);

            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateStatus")]
        public IActionResult UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            var result = _userService.UpdateStatus(_currentSession.UserId, request.UserId, request.IsActive, _currentContext.Ip4Address);

            return Ok(result);
        }

        [HttpPost]
        [Route("ResetPwd")]
        public IActionResult ResetPwd([FromBody] ResetPwdRequest request)
        {
            var result = _userService.ResetPassword(_currentSession.UserId, request.UserId, request.Pwd, _currentContext.Ip4Address);

            return Ok(result);
        }

        [HttpGet]
        [Route("ResetPassword/{id}")]
        public IActionResult ResetPassword(int id)
        {
            var model = _userService.GetUserById(id);
            var isInternalAdmin = _loginService.CheckExternalGroup(id, null, eFunctions.ExternalGroup);

            return Ok(new { model, isInternalAdmin });
        }

        [HttpGet]
        [Route("GrantPermission")]
        public IActionResult GrantPermission([FromQuery] int userId, [FromQuery] string username, [FromQuery] string groupName)
        {
            var funcs = _functionService.GetAllFunctions().ToList();
            funcs = funcs.Where(f => f.IsTest).ToList();
            var groups = _groupService.GetListGroup();
            var externalGroup = _groupService.GetExternalGroup();
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

            var isExternalGroup =
                externalGroup.Any(i => i.GroupName.Equals(ExtRequest.GetGroupName(groupName), StringComparison.OrdinalIgnoreCase));

            object userFunc = null!;
            object userSubFunc = null!;

            if (isExternalGroup)
            {
                userFunc = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ManageFunction
                                                           && x.FuncKey != eFunctions.ManageGroup
                                                           && x.FuncKey != eFunctions.ManageMenu
                                                           && x.FuncKey != eFunctions.ManageUser
                                                           && x.FuncKey != eFunctions.Administration
                                                           && x.FuncKey != eFunctions.SystemConfiguration
                                                           && x.FuncKey != eFunctions.ExternalGroup);

                userSubFunc = functionList.Where(x =>
                    x.ParentID != 0 && x.FuncKey != eFunctions.ManageFunction
                    && x.FuncKey != eFunctions.ManageGroup && x.FuncKey != eFunctions.ManageMenu
                    && x.FuncKey != eFunctions.ManageUser
                    && x.FuncKey != eFunctions.Administration && x.FuncKey != eFunctions.SystemConfiguration
                    && x.FuncKey != eFunctions.ExternalGroup);
            }
            else if (Group.IsITGroup(groupId ?? 0))
            {
                userFunc = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ExternalGroup);
                userSubFunc = functionList.Where(x => x.ParentID != 0 && x.FuncKey != eFunctions.ExternalGroup);
            }
            else
            {
                userFunc = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ManageFunction
                                                           && x.FuncKey != eFunctions.ManageMenu
                                                           && x.FuncKey != eFunctions.SystemConfiguration
                                                           && x.FuncKey != eFunctions.ExternalGroup);

                userSubFunc = functionList.Where(x =>
                    x.ParentID != 0 && x.FuncKey != eFunctions.ManageFunction
                    && x.FuncKey != eFunctions.ManageMenu && x.FuncKey != eFunctions.SystemConfiguration
                    && x.FuncKey != eFunctions.ExternalGroup);
            }

            return Ok(new
            {
                userId,
                username,
                userFunc,
                userSubFunc
            });
        }

        [HttpPost]
        [Route("GrantPermissionPost")]
        public IActionResult GrantPermissionPost([FromBody] GrantPermissionRequest request)
        {
            var result = _userService.UpdateUserPolicy(_currentSession.UserId, request.UserId, request.OldPolicy, request.Policy,
                _currentContext.Ip4Address);
            if (result.IsSuccess)
            {
                _cache.RemoveUserCache(_currentSession.UserId);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("InternalLogin")]
        public IActionResult InternalLogin([FromQuery] string username, [FromQuery] string groupName)
        {
            // TODO: Implement InternalLogin logic - this is complex and needs proper session handling
            // For now, return a placeholder
            return Ok(new { message = "Internal login not yet implemented", username, groupName });
        }

        [HttpPost]
        [Route("ResetPwdOwner")]
        public IActionResult ResetPwdOwner([FromBody] ResetPwdOwnerRequest request)
        {
            if (request.UserId != _currentSession.UserId)
            {
                return Forbid(); // Action denied equivalent
            }

            var result = _userService.ResetPassword(_currentSession.UserId, request.UserId, request.Pwd, _currentContext.Ip4Address);

            return Ok(result);
        }

        [HttpGet]
        [Route("ResetPasswordOwner/{id}")]
        public IActionResult ResetPasswordOwner(int id)
        {
            if (id != _currentSession.UserId)
            {
                return Forbid(); // Action denied equivalent
            }

            var model = _userService.GetUserById(id);
            var isInternalAdmin = _loginService.CheckExternalGroup(id, null, eFunctions.ExternalGroup);
            return Ok(new { model, isInternalAdmin });
        }

        private User GetUserModel()
        {
            var model = new User();
            var groups = _groupService.GetListGroup();
            object groupList = null!;

            if (_currentSession.IsITAdmin)
            {
                groupList = groups;
            }
            else if (_currentSession.IsInternalAdmin)
            {
                groupList = groups.Where(o => !Group.IsITGroup(o.GroupID));
            }
            else
            {
                groupList = groups.Where(o => o.GroupName == _currentSession.CurrencyGroup);
            }

            // Note: In ASP.NET Core, we can't set ViewBag here, so we return it in the response
            return model;
        }
    }

    // TODO: Define proper interfaces and services
    public interface IUserService
    {
        object GetListUsers(string userName, int? groupId);
        User GetUserById(int id);
        object EditUser(int currentUserId, int custId, string firstName, string lastName, string ipAddress, int groupId);
        object CreateUser(int currentUserId, string username, string pwd, string firstName, string lastName, int groupId, string ipAddress);
        object CreateExistUser(int currentUserId, string username, int groupId, string ipAddress);
        object UpdateStatus(int currentUserId, int userId, bool isActive, string ipAddress);
        object ResetPassword(int currentUserId, int userId, string pwd, string ipAddress);
        object GetUserFunction(int userId);
        object UpdateUserPolicy(int custId, int userId, string oldPolicy, string policy, string ipAddress);
    }

    public class UserService : IUserService
    {
        public object GetListUsers(string userName, int? groupId)
        {
            // TODO: Implement actual service logic
            return new List<User>();
        }

        public User GetUserById(int id)
        {
            // TODO: Implement actual service logic
            return new User { UserId = id, FirstName = "John", LastName = "Doe" };
        }

        public object EditUser(int currentUserId, int custId, string firstName, string lastName, string ipAddress, int groupId)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object CreateUser(int currentUserId, string username, string pwd, string firstName, string lastName, int groupId, string ipAddress)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object CreateExistUser(int currentUserId, string username, int groupId, string ipAddress)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object UpdateStatus(int currentUserId, int userId, bool isActive, string ipAddress)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object ResetPassword(int currentUserId, int userId, string pwd, string ipAddress)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object GetUserFunction(int userId)
        {
            // TODO: Implement actual service logic
            return new List<Function>();
        }

        public object UpdateUserPolicy(int custId, int userId, string oldPolicy, string policy, string ipAddress)
        {
            // TODO: Implement actual service logic
            return new { IsSuccess = true };
        }
    }

    public interface ILoginService
    {
        object CheckExternalGroup(int id, object param, string function);
        // Add other methods as needed
    }

    public class LoginService : ILoginService
    {
        public object CheckExternalGroup(int id, object param, string function)
        {
            // TODO: Implement actual service logic
            return false;
        }
    }

    // TODO: Define proper request/response models
    public class EditUserRequest
    {
        public int CustId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int GroupId { get; set; }
    }

    public class AddUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Pwd { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int GroupId { get; set; }
    }

    public class AddExistUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public int GroupId { get; set; }
    }

    public class ResetPwdRequest
    {
        public int UserId { get; set; }
        public string Pwd { get; set; } = string.Empty;
    }

    public class ResetPwdOwnerRequest
    {
        public int UserId { get; set; }
        public string Pwd { get; set; } = string.Empty;
    }

    // TODO: Define proper data models
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public int GroupID { get; set; }
    }
}