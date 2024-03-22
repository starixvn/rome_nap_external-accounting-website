using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Fanex.ExternalAccount.Controllers.ViewModels.Customer
{
    public class CustomerBaseViewModel
    {
        public CustomerBaseViewModel(AppConfiguration appConfig, string cndUri, string assetsVersion, string paramters)
        {
            CdnUri = cndUri;
            AssetsVersion = assetsVersion;
            Params = paramters;
            AppConfig = JsonConvert.SerializeObject(appConfig, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public string AppConfig { get; }

        public string CdnUri { get; }

        public string AssetsVersion { get; }

        public string Params { get; }
    }
}