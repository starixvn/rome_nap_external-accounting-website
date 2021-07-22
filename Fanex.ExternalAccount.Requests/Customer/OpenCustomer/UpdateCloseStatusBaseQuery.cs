namespace Fanex.ExternalAccount.Requests.Customer.OpenCustomer
{
    using Fanex.CustomerSetting.Module.Core.Web;
    using Fanex.ExternalAccount.Utilities;

    public class UpdateCloseStatusBaseQuery
    {
        public int UserId { get; set; }

        public int CustId { get; set; }

        public bool IsClosed { get; set; }

        public string Language { get; set; }

        public string Ip => CurrentContext.Instance.Ip4Address;

        public int SystemId => Config.SystemId;
    }
}