namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetSortingOrderCriteria : CriteriaBase
    {
        public string GroupName { get; }

        public GetSortingOrderCriteria(string groupName)
        {
            this.GroupName = groupName;
        }

        public override string GetSettingKey() => "EAS_Rpt_WinLossDaily_SortingOrder";

        public override bool IsValid() => true;
    }
}
