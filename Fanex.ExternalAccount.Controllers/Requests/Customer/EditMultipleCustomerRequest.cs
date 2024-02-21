using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    public class EditMultipleCustomerRequest
    {
        public string CustIds { get; set; }

        public string Usernames { get; set; }

        public byte RoleId { get; set; }

        public byte UplineRoleId { get; set; }

        internal virtual string ToJson() => JsonConvert.SerializeObject(new { CustIds, Usernames, RoleId, UplineRoleId }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}