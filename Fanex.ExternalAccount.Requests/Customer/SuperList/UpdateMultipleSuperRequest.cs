using System.Collections.Generic;

namespace Fanex.ExternalAccount.Requests.Customer.SuperList
{
    public class UpdateMultipleSuperRequest
    {
        public string FirstName { get; set; }
        public bool IsChangeFirstName { get; set; }
        public string LastName { get; set; }
        public bool IsChangeLastName { get; set; }
        public string Phone { get; set; }
        public bool IsChangePhone { get; set; }
        public string MobilePhone { get; set; }
        public bool IsChangeMobilePhone { get; set; }
        public string Password { get; set; }
        public string SecurityCode { get; set; }
        public bool IsForceChange { get; set; }
        public bool IsUpdateInfo { get; set; }
        public bool IsUpdatePassword { get; set; }
        public bool IsUpdateSecurityCode { get; set; }
        public string SuperIdNameList { get; set; }
    }
}
