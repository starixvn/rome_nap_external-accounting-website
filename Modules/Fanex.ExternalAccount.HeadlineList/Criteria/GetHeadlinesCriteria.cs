using System;
using System.Collections;
using Fanex.CustomerSetting.Module.Core.Web;
using Fanex.NAP.Framework.Data;

namespace Fanex.ExternalAccount.HeadlineList.Criteria
{
    public class GetHeadlinesCriteria : CriteriaBase
    {
        public int PageIndex { get; }

        public int PageSize { get; }

        public int TotalRows { get; set; }
        
        public int UserID { get; set; }
        public override string GetSettingKey()
        {
            return "EAS_CustomerSelByGroupHeadline";
        }

        public override Hashtable GetSpParams()
        {
            return new Hashtable
            {
                { "username", "" },
                { "groupheadline", "" },
                { "status", 0 },
                { "groupname", "ACCUUS" },
                { "userid", UserID },
            };
        }
        
        public override bool IsValid()
        {
            return UserID >0;
        }
    }
}