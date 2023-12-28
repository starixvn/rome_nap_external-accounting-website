using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.ExternalAccount.Controllers.ViewModels.Customer
{
    public class EditCustomerViewModel
    {
        public EditCustomerViewModel(AppConfiguration appConfig, string cndUri, string assetsVersion)
        {
            CdnUri = cndUri;
            AssetsVersion = assetsVersion;
            AppConfig = JsonConvert.SerializeObject(appConfig, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public string AppConfig { get; }

        public string CdnUri { get; }

        public string AssetsVersion { get; }
    }
}
