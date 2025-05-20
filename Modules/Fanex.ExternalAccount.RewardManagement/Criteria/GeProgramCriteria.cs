namespace Fanex.ExternalAccount.RewardManagement.Criteria
{
   // using Fanex.NAP.Framework.Data;
    using Fanex.Data.Repository;
    public class GeProgramCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Age_Rpt_GetProgramList";
        
        public override bool IsValid() => true;
    }
}