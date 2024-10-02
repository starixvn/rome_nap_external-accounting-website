using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    public class EditMultipleSuperBetSettingRequest : EditMultipleCustomerRequest
    {
        public int CustId { get; set; }

        public bool IsCopy { get; set; }

        internal override string ToJson() => JsonConvert.SerializeObject(new
        {
            CustId,
            CustIds,
            Usernames,
            RoleId,
            UplineRoleId,
            ProductGroupId,
            IsCopyBetSetting = IsCopy,
            GroupName
        }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}