namespace Fanex.ExternalAccount.Service.Products
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Caching;
    using Fanex.Caching.Memory;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DataAccess.Products;
    using Fanex.ExternalAccount.DTO.Products;
    using Fanex.ExternalAccount.Service.Resources;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.Data.Repository;

    public interface IProductDetailService
    {
        string GetProductDetailBelongSabaOtherSports(string groupName);

        IEnumerable<ProductDetailDto> GetProductDetails(IEnumerable<ProductDto> products, string groupName);

        IEnumerable<ProductDetailEntity> GetProductDetails(string groupName);

        string BuildProductDetailsXML(IEnumerable<int> validProductIds, string userSelectedResourceIds,
            string groupName);

        IEnumerable<int> RemoveProductIds(IEnumerable<int> validProductIds, string userSelectedResourceIds,
            string groupName);
    }

    public class ProductDetailService : IProductDetailService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheService Cache = new CacheService(new MemoryCache(new MemoryCacheOptions()));

        public ProductDetailService(IDynamicRepository dynamicRepository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public string GetProductDetailBelongSabaOtherSports(string groupName)
        {
            const string sabaOthers = "24";
            var productDetails = GetProductDetails(groupName)
                .Where(i => i.ResourceId == sabaOthers)
                .OrderBy(i => i.Order)
                .Select(i =>
                    CacheRef.GetResourceDataByRefIdAndResourceId(i.RefId, i.ChildResourceId.ToString(), string.Empty));

            return string.Format(Lang.SabaOtherSportsLegend, string.Join(", ", productDetails),
                CacheRef.GetResourceDataByRefIdAndResourceId("ProductDetail", sabaOthers, string.Empty));
        }


        public IEnumerable<ProductDetailDto> GetProductDetails(IEnumerable<ProductDto> products, string groupName)
        {
            var productHash = new Dictionary<int, bool>();
            products.ToList().ForEach(product =>
            {
                productHash[product.ProductId] = true;

                if (product.IsParent)
                {
                    product.Products.ToList().ForEach(subProduct => { productHash[subProduct.ProductId] = true; });
                }
            });

            return GetProductDetails(groupName)
                 .Where(p => productHash.ContainsKey(p.ProductId) && productHash[p.ProductId])
                .GroupBy(p => p.ResourceId)
                .Select(p =>
                {
                    var product = p.FirstOrDefault();

                    return new ProductDetailDto
                    {
                        RefId = product.RefId,
                        ResourceId = product.ResourceId,
                        ProductId = product.ProductId
                    };
                });
        }

        public IEnumerable<ProductDetailEntity> GetProductDetails(string groupName)
        {
            var canView = CanViewProductDetails(groupName);

            return GetProductDetailsWithCache().Where(p => canView.ContainsKey(p.ProductId));
        }

        private IEnumerable<ProductDetailEntity> GetProductDetailsWithCache()
        {
            const string key = "Fanex.NAP.External.Site.ProductDetails";
            Cache.TryGet<IEnumerable<ProductDetailEntity>>(key, out var result);

            if (result == null)
            {
                result = dynamicRepository.Fetch<ProductDetailEntity>(new GetProductsDetailCriteria())
                    .OrderBy(product => product.ProductId).ThenBy(product => product.Order);
                Cache.Set(key, result);
            }

            return result;
        }

        private static IDictionary<int, bool> CanViewProductDetails(string groupName)
        {
            var canView = new Dictionary<int, bool>();

            if (Config.IsOfferProductDetailGroup(groupName))
            {
                foreach (var productId in Config.ProductOfferProductDetails())
                {
                    canView[productId] = true;
                }
            }

            canView[ProductConstants.Sportsbook] = true;

            return canView;
        }

        public string BuildProductDetailsXML(IEnumerable<int> validProductIds, string userSelectedResourceIds,
            string groupName)
        {
            var validProductDetails = new List<ProductDetailEntity>();
            var productIdsHash = validProductIds.ToDictionary(kvp => kvp, kvp => true);
            var resourceIdsHash = ToDictionaryString(userSelectedResourceIds);

            GetProductDetailsWithCache()
                .Where(p => productIdsHash.ContainsKey(p.ProductId))
                .GroupBy(p => p.ProductId)
                .ToList()
                .ForEach(group =>
                {
                    if (ProductConstants.ProductListHasProductDetail.Any(i => i == group.Key))
                    {
                        var isSportsbook = group.Key == ProductConstants.Sportsbook;
                        var productDetails = Config.IsOfferProductDetailGroup(groupName)
                            ? group.Where(p => resourceIdsHash.ContainsKey(p.ResourceId))
                            : group;
                        validProductDetails.AddRange(isSportsbook
                            ? productDetails.Where(p => resourceIdsHash.ContainsKey(p.ResourceId))
                            : productDetails);
                    }
                });

            return validProductDetails.Any()
                ? BuildProductDetailsXML(validProductDetails)
                : string.Empty;
        }

        private static string BuildProductDetailsXML(IEnumerable<ProductDetailEntity> productDetails)
        {
            var xml = new StringBuilder("<ROOT>");
            foreach (var product in productDetails)
            {
                xml.Append(product.ToString());
            }

            xml.Append("</ROOT>");
            return xml.ToString();
        }

        private static IDictionary<string, bool> ToDictionaryString(string value)
        {
            var results = new Dictionary<string, bool>();

            if (!string.IsNullOrWhiteSpace(value))
            {
                value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList()
                    .ForEach(text => { results[text] = true; });
            }

            return results;
        }

        public IEnumerable<int> RemoveProductIds(IEnumerable<int> validProductIds, string userSelectedResourceIds,
            string groupName)
        {
            var removedProductIds = validProductIds.ToList();
            var resourceIds = ToEnumerable(userSelectedResourceIds);

            GetProductDetailsWithCache()
                .Where(p => removedProductIds.Contains(p.ProductId))
                .GroupBy(p => p.ProductId)
                .ToList()
                .ForEach(group =>
                {
                    if (group.Key == ProductConstants.Sportsbook ||
                        Config.ProductOfferProductDetails().Contains(group.Key) &&
                        Config.IsOfferProductDetailGroup(groupName))
                    {
                        var intersectResourceIds = group.Select(p => p.ResourceId).Intersect(resourceIds);

                        if (!intersectResourceIds.Any())
                        {
                            removedProductIds.RemoveAll(id => id == group.Key);
                        }
                    }
                });

            return removedProductIds;
        }

        private static string[] ToEnumerable(string value)
            => string.IsNullOrWhiteSpace(value)
                ? Array.Empty<string>()
                : value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    }
}