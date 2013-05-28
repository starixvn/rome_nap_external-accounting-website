using Fanex.NAP.Framework.Data;

namespace Fanex.Puffin.Core.Criteria
{
    public class TestCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Test_BetTypeSel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
