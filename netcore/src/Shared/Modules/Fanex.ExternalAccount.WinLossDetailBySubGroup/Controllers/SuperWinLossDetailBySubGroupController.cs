namespace Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Controllers
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Products;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Requests;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Services;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.ViewModels;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    [ExternalAccountAuthenticate(Function = eFunctions.SuperWinLossDetailBySubGroup, Policy = AuthenticationType.Read)]
    public class SuperWinLossDetailBySubGroupController : BaseController
    {
        private readonly IGroupService groupService;
        private readonly ISuperWinLossDetailBySubGroupService service;
        private readonly ICurrentSession currentSession;
        private readonly IProductDetailService productDetailService;

        internal SuperWinLossDetailBySubGroupController(
            IGroupService groupService,
            ISuperWinLossDetailBySubGroupService service,
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest,
            IConfiguration configuration,
            AuthorizationService authorizationService)
            : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.groupService = groupService;
            this.service = service;
        }

        public IActionResult Index()
        {
            int groupId;
            string groupName;
            var parentGroupName = CurrentSession.ParentGroupName;

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

            ViewBag.NoteSabaOthers = productDetailService.GetProductDetailBelongSabaOtherSports(parentGroupName);

            return PartialViewBase(new SuperWinLossDetailBySubGroupViewModel(groupId, groupName, subGroupNames));
        }

        public IActionResult GetData(GetSuperWinLossDetailBySubGroupRequest request)
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

    }
}
