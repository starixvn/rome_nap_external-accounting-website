namespace Fanex.ExternalAccount.Controllers.ViewModels.Customer
{
    public class EditCustomerViewModel : CustomerBaseViewModel
    {
        public EditCustomerViewModel(AppConfiguration appConfig, string cndUri, string assetsVersion, string parameters)
            : base(appConfig, cndUri, assetsVersion, parameters)
        {
        }
    }
}