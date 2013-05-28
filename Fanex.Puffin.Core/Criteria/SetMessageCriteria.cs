using Fanex.NAP.Framework.Data;

namespace Fanex.Puffin.Core.Criteria
{
    public class SetMessageCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Core_Dot_Msg_SetMessage";
        }

        public bool IsPublic {
            get { return false; }
        }

        public int Msgtype {
            get { return 1; }
        }

        public bool IsSticky {
            get { return false; }
        }

        public int Creator { get; set; }

        public string CreatorName { get; set; }

        public int MatchId { get; set; }

        public bool IsBatch {
            get { return true; }
        }

        public bool IsTest {
            get { return false; }
        }

        public string Content_en { get; set; }

        public string Content_ch { get; set; }

        public string Content_cs { get; set; }

        public string Content_it { get; set; }

        public string Content_jp { get; set; }

        public string Content_th { get; set; }

        public string Content_ko { get; set; }

        public string Content_es { get; set; }

        public override bool IsValid()
        {
            return true;
        }
    }
}
