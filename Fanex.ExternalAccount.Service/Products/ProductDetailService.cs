namespace Fanex.ExternalAccount.Service.Products
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Caching;
    using Fanex.ExternalAccount.DataAccess.Products;
    using Fanex.ExternalAccount.DTO.Products;
    using Fanex.NAP.Framework.Data;

    public interface IProductDetailService
    {
        IEnumerable<ProductDetailEntity> GetProductDetails();

        string BuildProductDetailsXML(string validProductIds, string userSelectedResourceIds, bool isBelongToSpecialGroups);
    }

    public class ProductDetailService : IProductDetailService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICache Cache = CacheService.CreateInstance(CacheType.MemoryCache);

        public ProductDetailService(IDynamicRepository dynamicRepository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<ProductDetailEntity> GetProductDetails()
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

        public string BuildProductDetailsXML(string validProductIds, string userSelectedResourceIds, bool isBelongToSpecialGroups)
        {
            var productIdsHash = ToDictionaryString(validProductIds);

            var validProductDetails = GetProductDetails()
                .Where(p => productIdsHash.ContainsKey(p.ProductId.ToString()));

            if (isBelongToSpecialGroups)
            {
                var resourceIdsHash = ToDictionaryString(userSelectedResourceIds);

                validProductDetails = validProductDetails.Where(p => resourceIdsHash.ContainsKey(p.ResourceId));
            }

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
    }
}
