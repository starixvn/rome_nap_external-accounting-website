namespace Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Mvc.Filters;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Requests;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Services;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.ViewModels;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;

    [ExternalAccountAuthenticate(Function = eFunctions.SuperWinLossDetailBySubGroup, Policy = AuthenticationType.Read)]
    public class SuperWinLossDetailBySubGroupController : BaseController
    {
        private readonly IGroupService groupService;
        private readonly ISuperWinLossDetailBySubGroupService service;

        public SuperWinLossDetailBySubGroupController()
            : this(new GroupService(), new SuperWinLossDetailBySubGroupService())
        {
        }

        internal SuperWinLossDetailBySubGroupController(
            IGroupService groupService,
            ISuperWinLossDetailBySubGroupService service)
        {
            this.groupService = groupService;
            this.service = service;
        }

        public ActionResult Index()
        {
            int groupId;
            string groupName;
            var parentGroupName = new CurrentSession().ParentGroupName;

            if (CurrentSession.IsITAdmin || CurrentSession.IsInternalAdmin)
            {
                var group = groupService.GetGroupByName(parentGroupName);
                groupId = group.GroupID;
                groupName = group.GroupName;
            }
            else
            {
                groupId = CurrentSession.GroupId;
                groupName = parentGroupName;
            }

            var subGroupNames = groupService
                .GetGroupWithSubGroups(groupId)
                .Where(g => g.ParentGroupId != 0)
                .Select(g => g.GroupName);

            return PartialViewBase(new SuperWinLossDetailBySubGroupViewModel(groupId, groupName, subGroupNames));
        }

        public ActionResult GetData(GetSuperWinLossDetailBySubGroupRequest request)
        {
            request.UserId = CurrentSession.UserId;

            return JsonBase(service.GetSuperWinLossDetailBySubGroupReport(request));
        }

        public void ExportExcel(GetSuperWinLossDetailBySubGroupRequest request)
        {
            request.PageIndex = 0;
            request.PageSize = int.MaxValue - 1;
            request.UserId = CurrentSession.UserId;
            var report = service.GetSuperWinLossDetailBySubGroupReport(request);

            var excelBuilder = new SuperWinLossDetailBySubGroupExcelBuilder(
                service.ConcatProductName(request.ProductIds, request.ProductDetails, request.GroupName),
                report,
                request.FromDate,
                request.ToDate,
                request.SubGroup);

            using (var memoryStream = excelBuilder.BuildMemoryStream())
            {
                ExportToExcel(memoryStream, excelBuilder.GetExcelFileName());
            }
        }

        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);

            if (!CurrentSession.IsITAdmin
                && !CurrentSession.IsInternalAdmin
                && !groupService.GetGroupWithSubGroups(CurrentSession.GroupId).Any())
            {
                filterContext.Result = new ViewResult { ViewName = "~/Views/Shared/NoPermission.cshtml" };
            }
        }
    }
}
