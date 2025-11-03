namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.MarkAccountType
{
    using System;
    using Fanex.Data.Repository;

    public class SearchMarkAccountCriteria : CriteriaBase
    {
        public string GroupName { get; set; }

        public int GroupId { get; set; }

        public string UserName { get; set; }

        public int UserId { get; set; }

        public string RoleId { get; set; }

        public string TypeId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalRows { get; set; }

        protected readonly int[] ValidPageSize = { 50, 100, 500 };

        public override bool IsValid() => Array.Exists(ValidPageSize, item => item == PageSize);

        public override string GetSettingKey()
            => "EAS_AccountType_Sel";
    }
}
