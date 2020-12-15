namespace Fanex.ExternalAccount.DTO.Products
{
    using Fanex.ExternalAccount.Core.Shared;

    public class ProductDetailDto
    {
        public string RefId { get; set; }

        public string ResourceId { get; set; }

        public string DisplayName
            => CacheRef.GetResourceDataByRefIdAndResourceId(RefId, ResourceId, string.Empty);

        public int ProductId { get; set; }

        public string ProductKey { get; set; }
    }
}
