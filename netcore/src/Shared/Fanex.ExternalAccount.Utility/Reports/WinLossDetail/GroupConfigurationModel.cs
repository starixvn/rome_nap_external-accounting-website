namespace Fanex.ExternalAccount.Utilities.Reports.WinLossDetail
{
    using RemotingRef;

    public class GroupConfigurationModel
    {
        private readonly IRemotingRefService remotingRefService;

        public GroupConfigurationModel() : this(null)
        {
        }

        public GroupConfigurationModel(IRemotingRefService remotingRefService)
        {
            this.remotingRefService = remotingRefService ?? new RemotingRefService();
        }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductText
            => remotingRefService.GetResourceDataByRefIdAndResourceId("Product", ProductId.ToString(), "en-US", ProductName);

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string Currency { get; set; }

        public string FormatedCurrency => string.IsNullOrEmpty(Currency) ? string.Empty : Currency.Replace(",", ", ");

        public string CurrencyCode { get; set; }

        public string FormatedCurrencyCode => string.IsNullOrEmpty(CurrencyCode) ? string.Empty : CurrencyCode.Replace(",", ", ");

        public string Site { get; set; }

        public string FormatedSite => string.IsNullOrEmpty(Site) ? string.Empty : Site.Replace(",", ", ");

        public bool IsShow { get; set; }

        public bool IsHide => !IsShow;
    }
}

