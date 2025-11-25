namespace Fanex.ExternalAccount.Controllers.UserControl
{
    using Microsoft.AspNetCore.Mvc;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DataAccess.Criteria.GroupConfiguration;
    using Service.Impl.UserControl;
    using Service.Products;
    using Service.UserControl;
    using ViewModels;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    public class GroupConfigurationController : BaseController
    {
        private readonly IGroupConfigurationService groupConfigurationService;
        private readonly IGroupService groupService;
        private readonly IProductService productService;

        public GroupConfigurationController(
            IGroupService groupService,
            IProductService productService,
            IGroupConfigurationService groupConfigurationService,
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest,
            IConfiguration configuration,
            AuthorizationService authorizationService) : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.groupService = groupService;
            this.groupConfigurationService = groupConfigurationService;
            this.productService = productService;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageProduct,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public IActionResult Index()
        {
            var products = productService.GetAllProducts();
            var groups = groupService.GetExternalGroup();

            return View(new GroupConfigurationViewModel { Groups = groups, Products = products });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageProduct, Policy = AuthenticateAction.AuthenticationType.Read)]
        public IActionResult Get(GetGroupConfigurationCriteria criteria)
        {
            var result = groupConfigurationService.Get(criteria);

            return Ok(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageProduct, Policy = AuthenticateAction.AuthenticationType.Host)]
        public IActionResult Update(UpdateGroupConfigurationCriteria criteria)
        {
            criteria.UserId = CurrentSession.UserId;
            var result = groupConfigurationService.Update(criteria);

            return Ok(result);
        }
    }
}