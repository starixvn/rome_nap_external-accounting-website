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
    using Fanex.ExternalAccount.Utilities;
    using Fanex.NAP.Framework.Data;

    public interface IProductDetailService
    {
        IEnumerable<ProductDetailDto> GetProductDetails(IEnumerable<ProductDto> products, string groupName);

        IEnumerable<ProductDetailEntity> GetProductDetails(string groupName);

        string BuildProductDetailsXML(IEnumerable<int> validProductIds, string userSelectedResourceIds, string groupName);

        IEnumerable<int> RemoveProductIds(IEnumerable<int> validProductIds, string userSelectedResourceIds, string groupName);
    }

    public class ProductDetailService : IProductDetailService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheService Cache = new CacheService(new MemoryCache(new MemoryCacheOptions()));

        public ProductDetailService(IDynamicRepository dynamicRepository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<ProductDetailDto> GetProductDetails(IEnumerable<ProductDto> products, string groupName)
        {
            var productHash = new Dictionary<int, bool>();
            products.ToList().ForEach(product =>
            {
                productHash[product.ProductId] = true;

                if (product.IsParent)
                {
                    product.Products.ToList().ForEach(subProduct =>
                    {
                        productHash[subProduct.ProductId] = true;
                    });
                }
            });

            var productDetails = new List<ProductDetailDto>();
            GetProductDetails(groupName)
                .GroupBy(p => p.ProductId)
                .ToList()
                .ForEach(group =>
                {
                    if (productHash.ContainsKey(group.Key))
                    {
                        productDetails.AddRange(group.Select(p => new ProductDetailDto
                        {
                            RefId = p.RefId,
                            ResourceId = p.ResourceId,
                            ProductId = p.ProductId
                        }));
                    }
                });

            return productDetails;
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
                result = dynamicRepository.Fetch<ProductDetailEntity>(new GetProductsDetailCriteria());
                Cache.Set(key, result);
            }

            return result;
        }

        private static IDictionary<int, bool> CanViewProductDetails(string groupName)
        {
            var canView = new Dictionary<int, bool>();

            if (Config.IsOfferJackpotProductDetailsGroup(groupName))
            {
                canView[ProductConstants.PragmaticPlay] = true;
            }

            if (Config.IsOfferSexyGamingProductDetailsGroup(groupName))
            {
                canView[ProductConstants.SexyGaming] = true;
            }

            if (Config.IsOfferSpadeGamingProductDetailsGroup(groupName))
            {
                canView[ProductConstants.SpadeGaming] = true;
            }

            if (Config.IsOfferAllbetProductDetailsGroup(groupName))
            {
                canView[ProductConstants.AllBet] = true;
            }
            
            return canView;
        }

        public string BuildProductDetailsXML(IEnumerable<int> validProductIds, string userSelectedResourceIds, string groupName)
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
                        validProductDetails.AddRange(
                            Config.IsOfferProductDetail(groupName) ? group.Where(p => resourceIdsHash.ContainsKey(p.ResourceId)) : group);
                    }
                });

            return validProductDetails.Any()
                ? BuildProductDetailsXML(validProductDetails)
                : string.Empty;
        }

        private static string BuildProductDetailsXML(IEnumerable<ProductDetailEntity> productDetails)
        {
            var xml = new StringBuilder("<ROOT>");
            productDetails.ToList().ForEach(product => xml.AppendFormat(
                "<ProductDetail ProductId=\"{0}\" Sporttype=\"{1}\" Bettype=\"{2}\"/>",
                product.ProductId,
                product.SportType,
                product.BetType));
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
                    .ForEach(text =>
                    {
                        results[text] = true;
                    });
            }

            return results;
        }

        public IEnumerable<int> RemoveProductIds(IEnumerable<int> validProductIds, string userSelectedResourceIds, string groupName)
        {
            var removedProductIds = validProductIds.ToList();
            var resourceIds = ToEnumerable(userSelectedResourceIds);

            GetProductDetailsWithCache()
                .Where(p => removedProductIds.Contains(p.ProductId))
                .GroupBy(p => p.ProductId)
                .ToList()
                .ForEach(group =>
                {
                    if ((group.Key == ProductConstants.PragmaticPlay && Config.IsOfferJackpotProductDetailsGroup(groupName)) ||
                    (group.Key == ProductConstants.SexyGaming && Config.IsOfferSexyGamingProductDetailsGroup(groupName)) ||
                    (group.Key == ProductConstants.SpadeGaming && Config.IsOfferSpadeGamingProductDetailsGroup(groupName)) ||
                    (group.Key == ProductConstants.AllBet && Config.IsOfferAllbetProductDetailsGroup(groupName)))
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
