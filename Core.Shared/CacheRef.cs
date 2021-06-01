namespace Fanex.ExternalAccount.Core.Shared
{
    using Fanex.RemotingRef;

    public static class CacheRef
    {
        private const string DefaultLanguage = "en-US";
        private static readonly IRemotingRefService remotingRefService = new RemotingRefService();

        public static string GetProductName(int productId, string defaultName = null)
            => GetResourceDataByRefIdAndResourceId("Product", productId.ToString(), defaultName);

        public static string GetProductDetailName(int resourceId, string defaultName = null)
            => GetResourceDataByRefIdAndResourceId("ProductDetail", resourceId.ToString(), defaultName);

        public static string GetResourceDataByRefIdAndResourceId(string refId, string resourceId, string defaultName = null)
            => remotingRefService.GetResourceDataByRefIdAndResourceId(refId, resourceId, DefaultLanguage, defaultName);

        public static string GetSportName(string sportTypeId)
            => remotingRefService.GetSportName(sportTypeId, DefaultLanguage);

        public static string GetBettype(string betTypeId)
            => remotingRefService.GetBetTypeName(betTypeId, DefaultLanguage);
    }
}
