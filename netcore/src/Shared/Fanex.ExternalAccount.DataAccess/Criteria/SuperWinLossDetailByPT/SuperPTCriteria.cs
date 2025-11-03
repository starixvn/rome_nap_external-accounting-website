namespace Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT
{
    using Fanex.ExternalAccount.Utilities;
    using Fanex.Data.Repository;

    public class SuperPTCriteria : CriteriaBase
    {
        public SuperPTCriteria(string supers)
        {
            Supers = supers;
            SettingStatus = Config.SettingStatus;
        }

        public string Supers { get; }

        public byte SettingStatus { get; }

        public override string GetSettingKey() => "EAS_GetPTProductBySuper";
        
        public override bool IsValid() => true;
    }
}
