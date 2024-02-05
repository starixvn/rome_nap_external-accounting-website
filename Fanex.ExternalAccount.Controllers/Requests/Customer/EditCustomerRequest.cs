using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    public class EditCustomerRequest
    {
        public int CustId { get; set; }

        public int RoleId { get; set; }

        public bool IsBetSettingOnly { get; set; }
        
        public bool NoPermissionEditCredit { get; set; } = false;

        internal string ToJson() => JsonConvert.SerializeObject(new { CustId, RoleId , NoPermissionEditCredit }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}