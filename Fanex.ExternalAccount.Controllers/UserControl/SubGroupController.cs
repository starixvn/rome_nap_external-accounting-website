namespace Fanex.ExternalAccount.Controllers.UserControl
{
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Controllers.Requests.SubGroup;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Fanex.ExternalAccount.Service.UserControl;


    public class SubGroupController : BaseController
    {
        private readonly ISubGroupService _subGroupService;
        private readonly IGroupService _groupService;

        public SubGroupController() : this(null, null)
        {
        }

        public SubGroupController(ISubGroupService subGroupService, IGroupService groupService)
        {
            _subGroupService = subGroupService ?? new SubGroupService();
            _groupService = groupService ?? new GroupService();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
           Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult SubGroupList() {
            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SubGroup,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult SubGroupPost(int groupId, int pageSize, int pageIndex)
        {
            if (!_groupService.CheckIsMainGroup(groupId))
            {
                return JsonBase(new Result(HttpStatusCode.Unauthorized));
            }

            var groups = _groupService.GetGroupWithSubGroups(groupId);

            if(!groups.Any())
            {
                return JsonBase(new Result(HttpStatusCode.NoContent));
            }

            var model = _subGroupService.GetListCustomerSubGroup(groups, pageSize, pageIndex);

            return JsonBase(model);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SubGroup,
            Policy = AuthenticateAction.AuthenticationType.Host, IsAjaxRequest = false)]
        public ActionResult SubGroupUpdate(SubGroupUpdateRequest updateRequest)
        {
            var result = _subGroupService.UpdateCustomerSubGroup(updateRequest.SubGroupName, updateRequest.CustIds, updateRequest.IsAssigned, CurrentSession.UserId, CurrentContext.Ip4Address);

            return JsonBase(result);
        }
    }
}
