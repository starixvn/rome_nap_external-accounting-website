using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    public class EditCustomerRequest
    {
        public int CustId { get; set; }

        public int RoleId { get; set; }

        public bool IsBetSettingOnly { get; set; }

        internal string ToJson() => JsonConvert.SerializeObject(new { CustId, RoleId }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}