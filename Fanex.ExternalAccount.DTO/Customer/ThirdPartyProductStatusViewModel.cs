using System.Collections.Generic;
using ThirdPartyProducts.StatusSetting.Model;

namespace Fanex.ExternalAccount.DTO.Customer
{
    public class ThirdPartyProductStatusViewModel
    {
        public int CustId { get; set; }

        public string CustName { get; set; }

        public int RoleId { get; set; }

        public bool HasCheckAll { get; set; }

        public bool GroupBySport { get; set; }

        public string RoleName
        {
            get
            {
                switch (this.RoleId)
                {
                    case 1:
                        return "Member";
                    case 2:
                        return "Agent";
                    case 3:
                        return "Master";
                    case 4:
                        return "Super";
                    default:
                        return string.Empty;
                }
            }
        }

        public List<ProductStatus> ProductStatusList { get; set; }
    }
}
