namespace Fanex.ExternalAccount.DataAccess.Criteria.ViewMaxPayoutSettingInformation
{
    using Fanex.Data.Repository;

    public class GetSabaSportsCriteria : CriteriaBase
    {
        public string Site { get; }

        public GetSabaSportsCriteria(string site)
        {
            Site = site;
        }

        public override string GetSettingKey() => "Acc_SabaVirtualSportsBetSettingRef_Sel";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Site);
    }
}





