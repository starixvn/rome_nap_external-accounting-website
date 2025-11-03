namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerList
{
    using System;
    using Fanex.Data.Repository;

    public class SearchCustomerListBaseCriteria : CriteriaBase
    {
        public string UserName { get; set; }

        public string Currencies { get; set; }

        public short Status { get; set; }

        public int UserId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalRows { get; set; }

        protected const int MinStatusValue = 0;

        protected const int MaxStatusValue = 5;

        protected readonly int[] ValidPageSize = { 50, 100, 500 };

        public override bool IsValid() => Status >= MinStatusValue && Status <= MaxStatusValue
                && (PageSize == int.MaxValue - 1 || Array.Exists(ValidPageSize, item => item == PageSize));

        public override string GetSettingKey()
        {
            throw new NotImplementedException();
        }
    }
}
