using Fanex.Data.Repository;

namespace Fanex.ExternalAccount.DataAccess.Criteria.TotalBet
{
    public class GetLeaguesCriteria : CriteriaBase
    {
        public GetLeaguesCriteria(bool isTest)
        {
            IsTest = isTest;
        }

        public bool IsTest { get; set; }

        public override string GetSettingKey() => "EAS_TotalBet_LeagueSel";

        public override bool IsValid() => true;
    }
}
