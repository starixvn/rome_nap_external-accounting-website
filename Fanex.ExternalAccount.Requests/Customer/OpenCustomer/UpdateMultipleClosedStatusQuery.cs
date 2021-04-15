namespace Fanex.ExternalAccount.Requests.Customer.OpenCustomer
{
    using Fanex.CustomerSetting.Module.Core.Web;

    public class UpdateMultipleClosedStatusQuery
    {
        public int UserId => new CurrentSession().UserId;

        public int CustId { get; set; }

        public string UserName { get; set; }

        public bool IsClosed { get; set; }

        public string Language { get; set; } = "E";

        public string Ip => CurrentContext.Instance.Ip4Address;
    }
}