namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;


    public class GetLogginInfoCriteria : CriteriaBase
    {
        public GetLogginInfoCriteria(int custId, string groupName)
        {
            CustId = custId;
            GroupName = groupName;
        }

        public int CustId { get; }

        public string GroupName { get; }

        public override string GetSettingKey()
            => "EAS_CustomerSelLoginInfo";

        public override bool IsValid() => CustId > 0;;
    }
}
