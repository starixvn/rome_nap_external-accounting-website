namespace Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT
{
    using Fanex.NAP.Framework.Data;

    public class SuperPTCriteria : CriteriaBase
    {
        public SuperPTCriteria(string supers)
        {
            Supers = supers;
        }

        public string Supers { get; }

        public override string GetSettingKey() => "EAS_GetPTProductBySuper";
    }
}
