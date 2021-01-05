namespace Fanex.ExternalAccount.HeadlineBonus.ViewModels
{
    using Fanex.ExternalAccount.Core.Context;

    public class CancelPendingViewModel
    {
        public string GroupName
            => ExtRequest.Instance.GetGroupName();

        public int CustId { get; set; }

        public long TransId { get; set; }

        public bool IsValid()
            => CustId > 0 && TransId > 0;
    }
}
