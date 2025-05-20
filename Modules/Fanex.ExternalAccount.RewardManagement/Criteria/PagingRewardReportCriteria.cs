namespace Fanex.ExternalAccount.RewardManagement.Criteria
{
    using Fanex.Data.Repository;
    
    public class PagingRewardReportCriteria : CriteriaBase
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }
        public override string GetSettingKey()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsValid()
        {
            throw new System.NotImplementedException();
        }
    }
}