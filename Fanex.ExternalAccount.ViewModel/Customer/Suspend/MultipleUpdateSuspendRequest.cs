namespace Fanex.ExternalAccount.ViewModel.Customer.Suspend
{
    public class MultipleUpdateSuspendRequest
    {
        public int CustId { get; set; }

        public string UserName { get; set; }

        public bool Suspended { get; set; }
    }
}
