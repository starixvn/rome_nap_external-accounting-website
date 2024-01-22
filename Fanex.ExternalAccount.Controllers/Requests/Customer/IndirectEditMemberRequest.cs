using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    public class IndirectEditMemberRequest
    {
        public int CustId { get; set; }

        public byte RoleId { get; set; }

        public byte UplineRoleId { get; set; }

        internal string ToJson()
            => JsonConvert.SerializeObject(new { CustId, RoleId, UplineRoleId }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}
