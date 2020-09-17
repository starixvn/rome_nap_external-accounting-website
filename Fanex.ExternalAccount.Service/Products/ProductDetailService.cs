namespace Fanex.ExternalAccount.Service.Products
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Caching;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DataAccess.Products;
    using Fanex.ExternalAccount.DTO.Products;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.NAP.Framework.Data;

    public interface IProductDetailService
    {
        IEnumerable<ProductDetailEntity> GetProductDetails(string groupName);

        string BuildProductDetailsXML(string validProductIds, string userSelectedResourceIds, string groupName);

        string RemoveProductIds(string validProductIds, string userSelectedResourceIds, string groupName);
    }

    public class ProductDetailService : IProductDetailService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICache Cache = CacheService.CreateInstance(CacheType.MemoryCache);

        public ProductDetailService(IDynamicRepository dynamicRepository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<ProductDetailEntity> GetProductDetails(string groupName)
        {
            var canView = CanViewProductDetails(groupName);

            return GetProductDetailsWithCache().Where(p => canView.ContainsKey(p.ProductId));
        }

        private IEnumerable<ProductDetailEntity> GetProductDetailsWithCache()
        {
            const string key = "Fanex.NAP.External.Site.ProductDetails";
            var result = Cache.Get<IEnumerable<ProductDetailEntity>>(key);

            if (result == null)
            {
                result = dynamicRepository.Fetch<ProductDetailEntity>(new GetProductsDetailCriteria());
                Cache.Set(key, result, DateTime.Now.AddDays(1));
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

            return canView;
        }

        public string BuildProductDetailsXML(string validProductIds, string userSelectedResourceIds, string groupName)
        {
            var validProductDetails = new List<ProductDetailEntity>();
            var productIdsHash = ToDictionaryString(validProductIds);
            var resourceIdsHash = ToDictionaryString(userSelectedResourceIds);

            GetProductDetailsWithCache()
                .Where(p => productIdsHash.ContainsKey(p.ProductId.ToString()))
                .GroupBy(p => p.ProductId)
                .ToList()
                .ForEach(group =>
                {
                    if (group.Key == ProductConstants.PragmaticPlay)
                    {
                        validProductDetails.AddRange(
                            Config.IsOfferJackpotProductDetailsGroup(groupName) ? group.Where(p => resourceIdsHash.ContainsKey(p.ResourceId)) : group);
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
            IDictionary<string, bool> results = new Dictionary<string, bool>();

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

        public string RemoveProductIds(string validProductIds, string userSelectedResourceIds, string groupName)
        {
            var removedProductIds = ToListString(validProductIds).Select(id => int.Parse(id)).ToList();
            var resourceIds = ToListString(userSelectedResourceIds);

            GetProductDetailsWithCache()
                .Where(p => removedProductIds.Contains(p.ProductId))
                .GroupBy(p => p.ProductId)
                .ToList()
                .ForEach(group =>
                {
                    if (group.Key == ProductConstants.PragmaticPlay && Config.IsOfferJackpotProductDetailsGroup(groupName))
                    {
                        var intersectResourceIds = group.Select(p => p.ResourceId).Intersect(resourceIds);

                        if (!intersectResourceIds.Any())
                        {
                            removedProductIds.RemoveAll(id => id == group.Key);
                        }
                    }
                });

            return string.Join(",", removedProductIds);
        }

        private static IEnumerable<string> ToListString(string value)
            => string.IsNullOrWhiteSpace(value)
            ? Enumerable.Empty<string>()
            : value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    }
}
