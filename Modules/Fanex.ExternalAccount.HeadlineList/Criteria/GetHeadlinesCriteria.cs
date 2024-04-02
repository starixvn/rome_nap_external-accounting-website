using System.Collections;
using Fanex.NAP.Framework.Data;

namespace Fanex.ExternalAccount.HeadlineList.Criteria
{
    public class GetHeadlinesCriteria : CriteriaBase
    {
        public int PageIndex { get; }

        public int PageSize { get; }

        public int TotalRows { get; set; }
        public int Status { get; set; }
        public string Username { get; set; }
        public string GroupHeadline { get; set; }
        public string GroupName { get; set; }

        public GetHeadlinesCriteria(string username, string groupHeadline, int status, int userId, string groupName)
        {
            Status = status;
            Username = username;
            GroupHeadline = groupHeadline;
            UserID = userId;
            GroupName = groupName;
        }
        
        public int UserID { get; set; }
        public override string GetSettingKey()
        {
            return "EAS_CustomerSelByGroupHeadline";
        }

        public override Hashtable GetSpParams()
        {
            return new Hashtable
            {
                { "username", Username },
                { "groupheadline", GroupHeadline },
                { "status", Status },
                { "groupname", GroupName },
                { "userid", UserID },
            };
        }
        
        public override bool IsValid()
        {
            return UserID >0;
        }
    }
}