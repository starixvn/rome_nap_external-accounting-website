using Fanex.NAP.Framework.Data;

namespace Fanex.Puffin.Core.Criteria
{
    public class SetUsrMessageCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Core_Dot_Msg_SetUsrMessage";
        }

        public int MessageId { get; set; }

        public int TargetId { get; set; }

        public string TargetName { get; set; }

        public bool IsRead {
            get { return false; }
        }

        public string BettransId { get; set; }
    }
}
