namespace Fanex.ExternalAccount.DataAccess.Criteria.SubGroup
{
    using Fanex.Data.Repository;

    public class UpdateCustomerSubGroupCriteria : INonQueryCommand
    {
        public string CustList { get; set; }

        public string GroupName { get; set; }

        public bool IsCheck { get; set; }

        public string IP { get; set; }

        public int UserId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey()
            => "EAS_CustomerUpdSubGroup";

        public bool IsValid()
            => !string.IsNullOrWhiteSpace(GroupName) && !string.IsNullOrWhiteSpace(CustList);
    }
}