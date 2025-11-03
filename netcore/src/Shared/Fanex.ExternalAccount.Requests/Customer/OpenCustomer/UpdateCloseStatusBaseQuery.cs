namespace Fanex.ExternalAccount.Requests.Customer.OpenCustomer
{
    using Fanex.ExternalAccount.Utilities;

    public class UpdateCloseStatusBaseQuery
    {
        public int UserId { get; set; }

        public int CustId { get; set; }

        public bool IsClosed { get; set; }

        public string Language { get; set; }

        public string Ip { get; set; }

        public int SystemId => Config.SystemId;
    }
}