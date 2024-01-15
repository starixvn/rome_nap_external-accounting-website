namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerList
{
    using Fanex.NAP.Framework.Data;
    using System;

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

        public override bool IsValid() => Status >= MinStatusValue && Status < MaxStatusValue
                && Array.Exists(ValidPageSize, item => item == PageSize);
    }
}
