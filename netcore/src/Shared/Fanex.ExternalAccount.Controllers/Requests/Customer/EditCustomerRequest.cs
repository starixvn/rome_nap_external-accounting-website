using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    public class EditCustomerRequest
    {
        public int CustId { get; set; }

        public int RoleId { get; set; }

        public int UplineRoleId { get; set; }

        public int ProductGroupId { get; set; }

        public bool NoPermissionEditCredit { get; set; }

        public int AgentId { get; set; }

        public int ServiceType { get; set; }

        internal string ToJson() => JsonConvert.SerializeObject(new { CustId, RoleId, UplineRoleId, NoPermissionEditCredit, ProductGroupId, AgentId, ServiceType }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}