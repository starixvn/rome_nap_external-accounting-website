namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerStatus
{
    using Fanex.Data.Repository;

    public class GetMultipleProtectedStatusCriteria : CriteriaBase
    {
        public GetMultipleProtectedStatusCriteria(string groupName, string custIds, byte roleId, int userId)
        {
            GroupName = groupName;
            CustList = custIds;
            RoleId = roleId;
            UserId = userId;
        }

        public string GroupName { get; }

        public string CustList { get; }

        public byte RoleId { get; }

        public int UserId { get; }

        public override string GetSettingKey()
            => "EAS_CustomerSelProtectionModeList";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(GroupName)
            && !string.IsNullOrWhiteSpace(CustList);
    }
}
