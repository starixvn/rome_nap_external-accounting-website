namespace Fanex.ExternalAccount.DataAccess.Criteria.SubGroup
{
    using Fanex.NAP.Framework.Data;

    public class GetSubGroupCustomerCriteria : CriteriaBase
    {
        public string GroupName { get; set; }

        public string SubGroup { get; set; }

        public string ListGroups { get; set; }

        public string Username { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalRows { get; set; }

        public override string GetSettingKey()
            => "EAS_CustomerSelBySubGroup";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(SubGroup);
    }
}
