namespace Fanex.ExternalAccount.DTO.Products
{
    using Fanex.RemotingRef;

    public class ProductDto
    {
        private readonly IRemotingRefService remotingRefService;

        protected ProductDto(): this(null)
        {
        }

        protected ProductDto(IRemotingRefService remotingRefService)
        {
            this.remotingRefService = remotingRefService ?? new RemotingRefService();
        }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductText 
            => remotingRefService.GetResourceDataByRefIdAndResourceId("Product", ProductId.ToString(), "en-US", ProductName);
    }
}