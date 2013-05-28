
using Fanex.NAP.Framework.Data;

namespace Fanex.Puffin.Admin.Criteria
{
    public class AccountingUserCheckOnlineStatusCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Acc_Recheckin";
        }

        public override bool IsValid()
        {
            return true;
        }

        public int On_UserID { get; set; }
        public bool IsKickedOut { get; set; }
    }
}
