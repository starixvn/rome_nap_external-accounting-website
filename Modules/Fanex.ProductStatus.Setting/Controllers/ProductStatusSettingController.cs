namespace Fanex.ProductStatus.Setting.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using AppResources;
    using Configuration;
    using Models;
    using Services;
    using ViewModels;
    using Web;

    public class ProductStatusSettingController : Controller
    {
        private const string SingleViewPath = "~/Areas/ProductStatusSetting/Views/Single/Index.cshtml";
        private const string MultipleViewPath = "~/Areas/ProductStatusSetting/Views/Multiple/Index.cshtml";
        private const string SimpleViewPath = "~/Areas/ProductStatusSetting/Views/Simple/Index.cshtml";

        private readonly ProductStatusService productStatusService;

        public ProductStatusSettingController()
            : this(null)
        {
        }

        public ProductStatusSettingController(ProductStatusService productStatusService)
        {
            this.productStatusService = productStatusService ?? new ProductStatusServiceImpl();
        }

        [ProductStatusAuthorize(IsHost = false)]
        public ActionResult Index(QueryViewModel viewModel)
        {
            PopulateQueryViewModel(viewModel);

            return View(SingleViewPath, viewModel);
        }

        [HttpPost, ProductStatusAuthorize(IsHost = true), ProductStatusValidateAntiForgeryToken]
        public ActionResult Update(UpdateViewModel viewModel)
        {
            if (!viewModel.IsValid())
            {
                return JsonBadRequest();
            }

            var result = productStatusService.UpdateProductStatus(
                    viewModel.IsGroup,
                    viewModel.ProductId,
                    viewModel.CustId,
                    viewModel.CustRole,
                    viewModel.ActorId,
                    viewModel.ActorName,
                    viewModel.IP,
                    viewModel.SettingStatus,
                    viewModel.ProductStatus.Select(product => product.CorrectProperties(viewModel.CustRole)).ToList(),
                    viewModel.SystemId);

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #region Multiple Update

        [HttpPost]
        public ActionResult MultipleView(MultipleQueryViewModel viewModel)
        {
            if (viewModel.ProductId != 0 && CanView())
            {
                PopulateQueryViewModel(viewModel);
            }

            return View(MultipleViewPath, viewModel);
        }

        [HttpPost, ProductStatusAuthorize(IsHost = true), ProductStatusValidateAntiForgeryToken]
        public ActionResult MultipleUpdate(MultipleUpdateViewModel viewModel)
        {
            if (!viewModel.IsValid())
            {
                return JsonBadRequest();
            }

            var results = productStatusService.MultipleUpdateProductStatus(
                viewModel.IsGroup,
                viewModel.ProductId,
                viewModel.CustRole,
                viewModel.CustomerIds,
                viewModel.CustomerNames,
                viewModel.ActorId,
                viewModel.ActorName,
                viewModel.IP,
                viewModel.SettingStatus,
                viewModel.ProductStatus,
                viewModel.SystemId);

            return Json(results, JsonRequestBehavior.DenyGet);
        }

        #endregion Multiple Update

        #region Old Behavior

        [ProductStatusAuthorize(IsHost = false)]
        public ActionResult Simple(SimpleQueryViewModel viewModel)
        {
            if (viewModel.IsValid())
            {
                viewModel.SetSimpleProductStatusViewModel(
                    productStatusService.GetProductStatus(viewModel.ProductId, viewModel.CustId, viewModel.SportTypeId, viewModel.SystemId, ProductStatusConfigurationHandler.Actor.SettingStatus));
            }

            return View(SimpleViewPath, viewModel);
        }

        [HttpPost, ProductStatusAuthorize(IsHost = true), ProductStatusValidateAntiForgeryToken]
        public ActionResult SimpleUpdate(SimpleUpdateViewModel viewModel)
        {
            if (!viewModel.IsValid())
            {
                return JsonBadRequest();
            }

            var result = productStatusService.UpdateProductStatus(
                viewModel.IsGroup,
                viewModel.ProductId,
                viewModel.CustId,
                viewModel.CustRole,
                viewModel.ActorId,
                viewModel.ActorName,
                viewModel.IP,
                viewModel.SettingStatus,
                viewModel.ProductStatus.Select(product => product.ToProductStatusCommand(viewModel.CustRole)).ToList(),
                viewModel.SystemId);

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #endregion Old Behavior

        private void PopulateQueryViewModel(BaseQueryViewModel viewModel)
        {
            if (viewModel.IsValid())
            {
                viewModel.SetProductStatusViewModel(
                    viewModel.IsGroup
                    ? productStatusService.GetProductStatusByGroup(viewModel.ProductId, viewModel.CustId, viewModel.SystemId, ProductStatusConfigurationHandler.Actor.SettingStatus)
                    : productStatusService.GetProductStatus(viewModel.ProductId, viewModel.CustId, viewModel.SportTypeId, viewModel.SystemId, ProductStatusConfigurationHandler.Actor.SettingStatus));
            }
        }

        private ActionResult JsonBadRequest()
            => Json(new Result((int)HttpStatusCode.BadRequest, Resources.BadRequest), JsonRequestBehavior.DenyGet);

        private bool CanView()
        {
            var controllerDescriptor = new ReflectedControllerDescriptor(GetType());
            var actionDescriptor = controllerDescriptor.FindAction(ControllerContext, nameof(MultipleView));
            var filterContext = new AuthorizationContext(ControllerContext, actionDescriptor);

            var authorizationService = ProductStatusConfigurationHandler.AuthorizeService;

            return authorizationService.CanView(filterContext);
        }
    }
}