using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Fanex.CustomerSetting.Module.Core.Criteria;
using Fanex.CustomerSetting.Module.Core.Services.Impl;
using Fanex.CustomerSetting.Module.Core.Services.Interface;
using Fanex.CustomerSetting.Security.Authenticate;
using Fanex.ExternalAccount.Core.Authorization.ProductStatus;
using Fanex.ExternalAccount.Core.Base;
using Fanex.ExternalAccount.Core.Reports.WinLossDetail;
using Fanex.ExternalAccount.Core.Shared;
using Fanex.ExternalAccount.Service.ProductStatusSetting;
using Fanex.ExternalAccount.ViewModel.ProductStatusSetting;
using Fanex.ProductStatus.Setting.AppResources;
using Fanex.ProductStatus.Setting.Models;

namespace Fanex.ExternalAccount.Controllers.Customer.Sportsbook
{
    public class ThirdPartyProductStatusController : BaseController
    {
        private readonly ProductStatusAuthorizeService productStatusAuthorizeService;
        private readonly ProductStatusSettingService productStatusSettingService;

        public ThirdPartyProductStatusController()
        {
            productStatusAuthorizeService = new ProductStatusAuthorizeService();
            productStatusSettingService = new ProductStatusSettingServiceImpl();
        }

        [HttpPost]
        public ActionResult MultipleView(MultipleViewModel viewModel)
        {
            viewModel.Products = GetProductList(viewModel.RoleId, viewModel.ProductIds);

            return View(viewModel);
        }

        [HttpPost]
        [AntiXSRFFilter]
        public ActionResult MultipleUpdate(MultipleUpdateViewModel viewModel)
        {
            var jsonRequestBehavior = JsonRequestBehavior.DenyGet;

            if (!viewModel.IsValid())
            {
                return Json(new Result(400, Resources.BadRequest), jsonRequestBehavior);
            }

            if (!productStatusAuthorizeService.HasPermission(AuthenticateAction.AuthenticationType.Host, viewModel.ProductId)
                || !AuthorizationService.IsBelong())
            {
                return ActionDenied(Request);
            }

            var results = productStatusSettingService.MultipleUpdate(
                    viewModel.ProductId,
                    viewModel.CustIds,
                    viewModel.CustNames,
                    CurrentSession.UserId,
                    CurrentContext.Ip4Address,
                    viewModel.ProductStatus);

            return Json(results, jsonRequestBehavior);
        }

        private IEnumerable<KeyValuePair<int, string>> GetProductList(int roleId, int[] productIds)
        {
            IServiceBase permissionService = new ServiceBase();

            var checkIsInternalAdminResult =
                permissionService.CheckCustomerIsInternalAdmin(
                    new CheckCustomerIsInternalAdminCriteria { CustID = CurrentSession.CustId });

            var isAuthorized = permissionService.CheckCustomerHasSpecialSitePermission(
                new CheckCustomerHasSpecialSiteFunctionPermissionCriteria { CustID = CurrentSession.CustId });

            var products = ProductsConfiguration.Instance.ProductCollection
                .Where(p => !IsHiddenProduct(p) && p.ProductId != 0 && p.ProductId != 1);

            products = products.Where(p => !p.IsSpecial
                || !AdminDoesNotHavePermissionOnSpecialProduct(checkIsInternalAdminResult, isAuthorized));

            if (roleId.IsNotSuper())
            {
                var includedProducts = ToProductDictionary(productIds);

                products = products.Where(p => includedProducts.ContainsKey(p.ProductId));
            }

            return products.Select(p => new KeyValuePair<int, string>(p.ProductId, GetProductNameFromRemotingRef(p.ProductId)));
        }

        private static IDictionary<int, bool> ToProductDictionary(int[] productIds)
        {
            IDictionary<int, bool> productDictionary = new Dictionary<int, bool>();

            if (productIds?.Length > 0)
            {
                foreach (var item in productIds)
                {
                    productDictionary[item] = true;
                }
            }

            return productDictionary;
        }
    }
}
