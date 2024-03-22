using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    public class CreateCustomerRequest
    {
        public int UplineId { get; set; }

        public byte UplineRoleId { get; set; }

        public string UplineUsername { get; set; }

        public string SuggestUsername { get; set; }

        public bool NoPermissionEditCredit { get; set; }

        internal string ToJson() => JsonConvert.SerializeObject(new { UplineId, UplineRoleId, NoPermissionEditCredit, SuggestUsername }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}