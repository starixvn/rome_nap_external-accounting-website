namespace Fanex.ExternalAccount.Requests.Customer.OpenCustomer
{
    using CustomerSetting.Module.Core.Web;

    public class UpdateSingleClosedStatusQuery
    {
        public int UserId => new CurrentSession().UserId;

        public int CustId { get; set; }

        public bool IsClosed { get; set; }

        public string Language { get; set; } = "E";

        public string Ip => CurrentContext.Instance.Ip4Address;
    }
}