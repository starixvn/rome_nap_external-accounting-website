namespace Fanex.ExternalAccount.Service.Products
{
    using System.Collections.Generic;
    using System.Linq;
    using DTO.Products;
    using Fanex.CustomerSetting.Module.Core.Criteria;
    using Fanex.CustomerSetting.Module.Core.Services.Impl;
    using Fanex.CustomerSetting.Module.Core.Services.Interface;
    using Fanex.ExternalAccount.DataAccess.Products;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.NAP.Framework.Data;

    public interface IProductService
    {
        IEnumerable<ProductDto> GetAllProducts();

        IEnumerable<ProductDto> GetProducts(int userId, int groupId);

        IEnumerable<int> ValidateProductIds(int userId, int groupId, IEnumerable<int> productIds);

        bool AdminDoesNotHavePermissionOnSpecialProduct(int userId);
    }

    public class ProductService : IProductService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IServiceBase permissionService;

        public ProductService(
            IDynamicRepository dynamicRepository = null,
            IServiceBase permissionService = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
            this.permissionService = permissionService ?? new ServiceBase();
        }

        public IEnumerable<ProductDto> GetAllProducts()
        {
            var products = GetRawProducts();

            return GroupProducts(products);
        }

        public IEnumerable<ProductDto> GetProducts(int userId, int groupId)
        {
            var products = GetRawProducts(groupId);
            var filteredProducts = FilterProducts(userId, products);

            return GroupProducts(filteredProducts);
        }

        private IEnumerable<ProductEntity> GetRawProducts(int? groupId = null)
            => dynamicRepository.Fetch<ProductEntity>(new GetProductsCriteria(groupId));

        private IEnumerable<ProductEntity> FilterProducts(int userId, IEnumerable<ProductEntity> products)
        {
            var specialProductIds = Config.SpecialProductIds();
            var adminDoesNotHavePermissionOnSpecialProduct = AdminDoesNotHavePermissionOnSpecialProduct(userId);

            foreach (var product in products)
            {
                if (specialProductIds.ContainsKey(product.ProductId) && adminDoesNotHavePermissionOnSpecialProduct)
                {
                    continue;
                }

                yield return product;
            }
        }

        private static IEnumerable<ProductDto> GroupProducts(IEnumerable<ProductEntity> products)
        {
            var productList = new List<ProductDto>();

            products.GroupBy(p => p.Type)
                .ToList()
                .ForEach(grp =>
                {
                    if (grp.Key == 0)
                    {
                        productList.AddRange(grp.Select(p => new ProductDto(p.ProductId, p.ProductName)));
                    }
                    else
                    {
                        productList.Add(new ProductDto(grp.Key, products: grp.Select(p => new ProductDto(p.ProductId, p.ProductName))));
                    }
                });

            return productList;
        }

        public bool AdminDoesNotHavePermissionOnSpecialProduct(int userId)
        {
            var checkIsInternalAdminResult =
                permissionService.CheckCustomerIsInternalAdmin(
                    new CheckCustomerIsInternalAdminCriteria { CustID = userId });

            var isAuthorized = permissionService.CheckCustomerHasSpecialSitePermission(
                new CheckCustomerHasSpecialSiteFunctionPermissionCriteria { CustID = userId });

            return checkIsInternalAdminResult.IsInternalAdmin
               && (!checkIsInternalAdminResult.IsAllowAccess || !isAuthorized);
        }

        public IEnumerable<int> ValidateProductIds(int userId, int groupId, IEnumerable<int> productIds)
        {
            if (!productIds.Any())
            {
                return Enumerable.Empty<int>();
            }

            var productIdsHash = ToDictionary(productIds);
            var products = GetRawProducts(groupId);

            return FilterProducts(userId, products)
                .Where(p => productIdsHash.ContainsKey(p.ProductId))
                .Select(p => p.ProductId);
        }

        private static IDictionary<int, bool> ToDictionary(IEnumerable<int> arrayInt)
        {
            var dictionary = new Dictionary<int, bool>();

            foreach (var item in arrayInt)
            {
                dictionary[item] = true;
            }

            return dictionary;
        }
    }
}
