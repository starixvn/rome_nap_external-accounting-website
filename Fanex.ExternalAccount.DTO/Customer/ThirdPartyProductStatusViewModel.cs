namespace Fanex.ExternalAccount.DTO.Customer
{
    using System.Collections.Generic;
    using Core.Shared;
    using ThirdPartyProducts.StatusSetting.Model;

    public class ThirdPartyProductStatusViewModel
    {
        public int CustId { get; set; }

        public string CustName { get; set; }

        public int RoleId { get; set; }

        public bool HasCheckAll { get; set; }

        public bool GroupBySport { get; set; }

        public string RoleName => RoleId.DisplayRoleName();

        public List<ProductStatus> ProductStatusList { get; set; }
    }
}
