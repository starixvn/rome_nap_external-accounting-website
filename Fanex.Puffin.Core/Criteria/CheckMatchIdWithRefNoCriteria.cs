using System;
using Fanex.NAP.Framework.Data;
using Fanex.Puffin.Core.Utils;

namespace Fanex.Puffin.Core.Criteria
{
    public class CheckMatchIdWithRefNoCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Core_CheckMatchIDwithRefno";
        }

        /// <summary>
        /// Check data's constrains
        /// </summary>
        /// <returns>True: valid; False: invalid</returns>
        public override bool IsValid()
        {
            return true;
        }

        public int MatchId { get; set; }

        public Int64 FromRefno { get; set; }

        public Int64 ToRefno { get; set; }

        public Helper.SportType Type { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }


    }
}
