namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.Menu
{
    using Fanex.Data.Repository;

    public class GetAllMenusCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "EAS_MenuSel";

        public override bool IsValid() => true;
    }
}