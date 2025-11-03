namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssociationAccountCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public int UserId { get; set; }

        [SpParam(Ignored = true)]
        public string CustomerName { get; set; }

        public string GroupName { get; set; }

        public string AssociationList { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalRows { get; set; }

        public int TotalAccount { get; set; }

        public override string GetSettingKey()
            => "EAS_Customer_AssociationInfo";

        public override bool IsValid()
            => !string.IsNullOrEmpty(GroupName) && !string.IsNullOrEmpty(AssociationList);
    }
}
