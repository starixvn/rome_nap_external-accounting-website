using Fanex.NAP.Framework.Data;

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
    }
}
