namespace Fanex.ExternalAccount.DataAccess.Criteria.TotalBet
{
    public class GetTotalBetMCCriteria : GetTotalBetCriteria
    {
        public int BaseCurrency { get; set; }
        public override string GetSettingKey() => "EAS_TotalBet_1378Sel_ByCurrency";
    }
}
