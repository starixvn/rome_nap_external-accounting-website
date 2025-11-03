namespace Fanex.ExternalAccount.SearchAndCopy.Models
{
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.SearchAndCopy.Requests;

    public class CloneSettingContext
    {
        public CloneSettingContext(CloneSettingRequest request, int existingCustId, int currency, string site, ICurrentSession currentSession, ICurrentContext currentContext)
        {
            Request = request;
            ExistingCustId = existingCustId;
            CurrentSession = currentSession;
            CurrentContext = currentContext;
            Currency = currency;
            Site = site;
        }

        public CloneSettingRequest Request { get; }

        public int ExistingCustId { get; }

        public ICurrentContext CurrentContext { get; }

        public ICurrentSession CurrentSession { get; }

        public CustomerInfo ExistingCustomerInfo { get; set; }

        public Result Result { get; set; }

        public int CustId { get; set; }

        public int Currency { get; }

        public string Site { get; }
    }
}
