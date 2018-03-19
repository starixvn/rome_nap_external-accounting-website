namespace Fanex.ExternalAccount.Controllers.Customer.Sportsbook
{
    using System.Web.Mvc;
    using Core.Base;

    public class ThirdPartyProductStatusController : BaseController
    {
        public ActionResult AccessDenied() => ActionDenied(Request);

        //private readonly ProductStatusAuthorizeService productStatusAuthorizeService;

        //public ThirdPartyProductStatusController()
        //{
        //    productStatusAuthorizeService = new ProductStatusAuthorizeService();
        //}

        //[HttpPost]
        //[AntiXSRFFilter]
        //public ActionResult MultipleUpdate(MultipleUpdateViewModel viewModel)
        //{
        //    var jsonRequestBehavior = JsonRequestBehavior.DenyGet;

        //    if (!viewModel.IsValid())
        //    {
        //        return Json(new Result(400, Resources.BadRequest), jsonRequestBehavior);
        //    }

        //    if (!productStatusAuthorizeService.HasPermission(AuthenticateAction.AuthenticationType.Host, viewModel.ProductId)
        //        || !AuthorizationService.IsBelong())
        //    {
        //        return ActionDenied(Request);
        //    }

        //    var results = productStatusSettingService.MultipleUpdate(
        //            viewModel.ProductId,
        //            viewModel.CustIds,
        //            viewModel.CustNames,
        //            CurrentSession.UserId,
        //            CurrentContext.Ip4Address,
        //            viewModel.ProductStatus);

        //    return Json(results, jsonRequestBehavior);
        //}

        //private IEnumerable<KeyValuePair<int, string>> GetProductList(int roleId, int[] productIds)
        //{
        //    IServiceBase permissionService = new ServiceBase();

        //    var checkIsInternalAdminResult =
        //        permissionService.CheckCustomerIsInternalAdmin(
        //            new CheckCustomerIsInternalAdminCriteria { CustID = CurrentSession.CustId });

        //    var isAuthorized = permissionService.CheckCustomerHasSpecialSitePermission(
        //        new CheckCustomerHasSpecialSiteFunctionPermissionCriteria { CustID = CurrentSession.CustId });

        //    var products = ProductsConfiguration.Instance.ProductCollection
        //        .Where(p => !IsHiddenProduct(p) && p.ProductId != 0 && p.ProductId != 1);

        //    products = products.Where(p => !p.IsSpecial
        //        || !AdminDoesNotHavePermissionOnSpecialProduct(checkIsInternalAdminResult, isAuthorized));

        //    if (roleId.IsNotSuper())
        //    {
        //        var includedProducts = ToProductDictionary(productIds);

        //        products = products.Where(p => includedProducts.ContainsKey(p.ProductId));
        //    }

        //    return products.Select(p => new KeyValuePair<int, string>(p.ProductId, GetProductNameFromRemotingRef(p.ProductId)));
        //}

        //private static IDictionary<int, bool> ToProductDictionary(int[] productIds)
        //{
        //    IDictionary<int, bool> productDictionary = new Dictionary<int, bool>();

        //    if (productIds?.Length > 0)
        //    {
        //        foreach (var item in productIds)
        //        {
        //            productDictionary[item] = true;
        //        }
        //    }

        //    return productDictionary;
        //}
    }
}
