using Fanex.ExternalAccount.Controllers.Requests.Customer;

namespace Fanex.ExternalAccount.Controllers.ViewModels.Customer
{
    public class IndirectEditMemberViewModel : CustomerBaseViewModel
    {
        public IndirectEditMemberViewModel(AppConfiguration appConfig, string cndUri, string assetsVersion, string parameters)
             : base(appConfig, cndUri, assetsVersion, parameters)
        {
        }
    }
}
