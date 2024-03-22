namespace Fanex.ExternalAccount.Controllers.ViewModels.Customer
{
    public class CreateCustomerViewModel : CustomerBaseViewModel
    {
        public CreateCustomerViewModel(AppConfiguration appConfig, string cndUri, string assetsVersion, string parameters, string secretKey, string pageTitle)
            : base(appConfig, cndUri, assetsVersion, parameters)
        {
            SecretKey = secretKey;
            PageTitle = pageTitle;
        }

        public string SecretKey { get; }

        public string PageTitle { get; }
    }
}