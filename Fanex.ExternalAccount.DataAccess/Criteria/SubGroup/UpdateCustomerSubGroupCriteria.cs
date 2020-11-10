namespace Fanex.ExternalAccount.DataAccess.Criteria.SubGroup
{
    using Fanex.NAP.Framework.Data;

    public class UpdateCustomerSubGroupCriteria : CriteriaBase
    {
        public string CustList { get; set; }

        public string GroupName { get; set; }
        
        public bool IsCheck { get; set; }

        public string IP { get; set; }

        public int UserId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
            => "EAS_CustomerUpdSubGroup";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(GroupName) && !string.IsNullOrWhiteSpace(CustList);
    }
}
