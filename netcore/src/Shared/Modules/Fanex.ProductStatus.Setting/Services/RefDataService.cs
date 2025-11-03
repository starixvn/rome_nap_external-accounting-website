namespace Fanex.ProductStatus.Setting.Services
{
    using System.Threading;
    using RemotingRef;

    public interface RefDataService
    {
        string GetBetTypeName(int betTypeId);

        string GetProductName(int productId);

        string GetSportTypeName(int sportTypeId);
    }

    internal class RefDataServiceImpl : RefDataService
    {
        private readonly IRemotingRefService remotingRefService;

        public RefDataServiceImpl(IRemotingRefService remotingRefService = null)
        {
            this.remotingRefService = remotingRefService ?? new RemotingRefService();
        }

        public string GetBetTypeName(int betTypeId)
            => remotingRefService.GetBetTypeName(betTypeId.ToString(), LanguageCode);

        public string GetSportTypeName(int sportTypeId)
           => remotingRefService.GetSportName(sportTypeId.ToString(), LanguageCode, null) ?? remotingRefService.GetResourceDataByRefIdAndResourceId(
                "DummySport",
                sportTypeId.ToString(),
                LanguageCode,
                string.Empty);

        public string GetProductName(int productId)
            => remotingRefService.GetResourceDataByRefIdAndResourceId(
                "Product",
                productId.ToString(),
                LanguageCode,
                string.Empty);

        private static string LanguageCode
            => Thread.CurrentThread.CurrentCulture.Name;
    }
}
