namespace Fanex.ExternalAccount.SearchAndCopy.Models
{
    using Fanex.CustomerSetting.Module.Core.Web;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.SearchAndCopy.Requests;

    public class CloneSettingContext
    {
        public CloneSettingContext(CloneSettingRequest request, int existingCustId, ICurrentSession currentSession, ICurrentContext currentContext)
        {
            Request = request;
            ExistingCustId = existingCustId;
            CurrentSession = currentSession;
            CurrentContext = currentContext;
        }

        public CloneSettingRequest Request { get; }

        public int ExistingCustId { get; }

        public ICurrentContext CurrentContext { get; }

        public ICurrentSession CurrentSession { get; }

        public NG2Status NG2Status { get; set; }

        public CustomerInfo ExistingCustomerInfo { get; set; }

        public Result Result { get; set; }

        public int CustId { get; set; }
    }
}
