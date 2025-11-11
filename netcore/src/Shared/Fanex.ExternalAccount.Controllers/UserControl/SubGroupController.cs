namespace Fanex.ExternalAccount.Controllers.UserControl
{
    using System.Linq;
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Controllers.Requests.SubGroup;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.UserControl;
    using IBCUtils;

    public class SubGroupController : BaseController
    {
        private readonly ISubGroupService _subGroupService;
        private readonly IGroupService _groupService;

        public SubGroupController() : this(null!, null!)
        {
        }

        public SubGroupController(ISubGroupService subGroupService, IGroupService groupService)
        {
            _subGroupService = subGroupService ?? new SubGroupService();
            _groupService = groupService ?? new GroupService();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SubGroup,
           Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public IActionResult SubGroupList() {
            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SubGroup,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public IActionResult SubGroupPost(int groupId)
        {
            if (!_groupService.CheckIsMainGroup(groupId))
            {
                return JsonBase(new Result((int)HttpStatusCode.Unauthorized));
            }

            var groups = _groupService.GetGroupWithSubGroups(groupId);

            if(!groups.Any())
            {
                return JsonBase(new Result((int)HttpStatusCode.NoContent));
            }

            return JsonBase(groups.Where(group => group.ParentGroupId != 0));
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SubGroup,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public IActionResult SearchSubGroupCustomer(string groupName, string subGroupName, string username, int pageSize, int pageIndex)
        {
            var model = _subGroupService.GetListCustomerSubGroup(groupName, subGroupName, username, pageSize, pageIndex);

            return JsonBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SubGroup,
            Policy = AuthenticateAction.AuthenticationType.Host, IsAjaxRequest = true)]
        public IActionResult SubGroupUpdate(SubGroupUpdateRequest updateRequest)
        {
            var result = _subGroupService.UpdateCustomerSubGroup(updateRequest.SubGroupName, updateRequest.CustIds, updateRequest.IsAssigned, CurrentSession.UserId, CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SubGroup,
            Policy = AuthenticateAction.AuthenticationType.Host, IsAjaxRequest = false)]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeSubGroupAdminPassword(string groupName)
        {
            var admins = _subGroupService.GetListAdminSubGroup(groupName).Select(admin => new SelectListItem { Text = admin.Username, Value = admin.CustId.ToString() }).ToList();
            ViewBag.Admins = admins;
            ViewBag.GroupName = groupName;
            ViewBag.SCode = Encryptor.RandomCaptcha;

            return PartialViewBase();
        }

        [ValidateAntiForgeryToken]
        [ExternalAccountAuthenticate(Function = eFunctions.SubGroup,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        public IActionResult UpdateAdminPassword(int adminId, string password)
        {
            var rawPassword = Encryptor.Decrypt(password, Encryptor.RandomCaptcha);
            var result = _subGroupService.UpdateSubGroupAdminPassword(adminId, Encryption.CfsEnCode(rawPassword), CurrentSession.UserId, CurrentContext.Ip4Address);

            return JsonBase(result);
        }
    }
}
