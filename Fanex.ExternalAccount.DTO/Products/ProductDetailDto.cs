namespace Fanex.ExternalAccount.DTO.Products
{
    using Fanex.RemotingRef;

    public class ProductDetailDto
    {
        private readonly IRemotingRefService remotingRefService;

        public ProductDetailDto() : this(null)
        {
        }

        protected ProductDetailDto(IRemotingRefService remotingRefService)
        {
            this.remotingRefService = remotingRefService ?? new RemotingRefService();
        }

        public string RefId { get; set; }

        public string ResourceId { get; set; }

        public string DisplayName
            => remotingRefService.GetResourceDataByRefIdAndResourceId(RefId, ResourceId, "en-US", string.Empty);

        public string ProductKey { get; set; }
    }
}
