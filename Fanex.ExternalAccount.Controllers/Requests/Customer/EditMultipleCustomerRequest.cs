using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    public class EditMultipleCustomerRequest
    {
        public string CustIds { get; set; }

        public string Usernames { get; set; }

        public byte RoleId { get; set; }

        public bool IsBetSettingOnly { get; set; }

        public bool NoPermissionEditCredit { get; set; }

        internal string ToJson() => JsonConvert.SerializeObject(new { CustIds, Usernames, RoleId, NoPermissionEditCredit }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}