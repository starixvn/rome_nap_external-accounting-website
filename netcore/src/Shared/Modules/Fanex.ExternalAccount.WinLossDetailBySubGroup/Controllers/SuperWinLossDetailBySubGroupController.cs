namespace Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Mvc.Filters;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Products;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Requests;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Services;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.ViewModels;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;

    [ExternalAccountAuthenticate(Function = eFunctions.SuperWinLossDetailBySubGroup, Policy = AuthenticationType.Read)]
    public class SuperWinLossDetailBySubGroupController : BaseController
    {
        private readonly IGroupService groupService;
        private readonly ISuperWinLossDetailBySubGroupService service;
        private readonly CurrentSession currentSession;
        private readonly ProductDetailService productDetailService;

        public SuperWinLossDetailBySubGroupController()
            : this(new GroupService(), new SuperWinLossDetailBySubGroupService(), new CurrentSession(), new ProductDetailService())
        {
        }

        internal SuperWinLossDetailBySubGroupController(
            IGroupService groupService,
            ISuperWinLossDetailBySubGroupService service,
            CurrentSession currentSession,
            ProductDetailService productDetailService)
        {
            this.groupService = groupService;
            this.service = service;
            this.currentSession = currentSession;
            this.productDetailService = productDetailService ?? new ProductDetailService();
        }

        public ActionResult Index()
        {
            int groupId;
            string groupName;
            var parentGroupName = currentSession.ParentGroupName;

            if (currentSession.IsITAdmin || currentSession.IsInternalAdmin)
            {
                var group = groupService.GetGroupByName(parentGroupName);
                groupId = group.GroupID;
                groupName = group.GroupName;
            }
            else
            {
                groupId = currentSession.GroupId;
                groupName = parentGroupName;
            }

            var subGroupNames = groupService
                .GetGroupWithSubGroups(groupId)
                .Where(g => g.ParentGroupId != 0)
                .Select(g => g.GroupName);

            ViewBag.NoteSabaOthers = productDetailService.GetProductDetailBelongSabaOtherSports(parentGroupName);

            return PartialViewBase(new SuperWinLossDetailBySubGroupViewModel(groupId, groupName, subGroupNames));
        }

        public ActionResult GetData(GetSuperWinLossDetailBySubGroupRequest request)
        {
            return JsonBase(service.GetSuperWinLossDetailBySubGroupReport(request));
        }

        public void ExportExcel(GetSuperWinLossDetailBySubGroupRequest request)
        {
            request.PageIndex = 0;
            request.PageSize = int.MaxValue - 1;
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

            if (!currentSession.IsITAdmin
                && !currentSession.IsInternalAdmin
                && !groupService.GetGroupWithSubGroups(currentSession.GroupId).Any())
            {
                filterContext.Result = new ViewResult { ViewName = "~/Views/Shared/NoPermission.cshtml" };
            }

            // Logged in user is selecting sub-group
            if (!currentSession.CurrencyGroup.Equals(currentSession.ParentGroupName, StringComparison.OrdinalIgnoreCase))
            {
                filterContext.Result = new ViewResult { ViewName = "~/Views/SuperWinLossDetailBySubGroup/_NoPermission.cshtml" };
            }
        }
    }
}
