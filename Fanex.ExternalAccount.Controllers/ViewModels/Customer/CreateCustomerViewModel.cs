using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Fanex.PTEngine.BusinessObjects;

namespace Fanex.ExternalAccount.Controllers.ViewModels.Customer
{
    public class CreateCustomerViewModel
    {
        public CreateCustomerViewModel(AppConfiguration appConfig, string cndUri, string assetsVersion, string secretKey, int uplineId = 0, byte uplineRoleId = 0, string suggestUsername = "")
        {
            CdnUri = cndUri;
            AssetsVersion = assetsVersion;
            SecretKey = secretKey;
            AppConfig = JsonConvert.SerializeObject(appConfig, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            Params = JsonConvert.SerializeObject(new { uplineId, uplineRoleId, suggestUsername }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            switch (uplineRoleId)
            {
                case (byte)CustomerRole.Super:
                    PageTitle = $"Add New Master";
                    break;

                case (byte)CustomerRole.Master:
                    PageTitle = $"Add New Agent";
                    break;

                case (byte)CustomerRole.Agent:
                    PageTitle = $"Add New Member";
                    break;

                default:
                    PageTitle = "Add New Super";
                    break;
            }
        }

        public string AppConfig { get; }

        public string CdnUri { get; }

        public string AssetsVersion { get; }

        public string Params { get; }

        public string SecretKey { get; }

        public string PageTitle { get; }
    }
}