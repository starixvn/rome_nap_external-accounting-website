namespace Fanex.ExternalAccount.HeadlineList.Criteria
{
    using System.Collections;
    using Fanex.Data.Repository;
    
    public class GetHeadlinesCriteria : CriteriaBase
    {
        public int Status { get; set; }
        public string Username { get; set; }
        public string GroupHeadline { get; set; }
        public string GroupName { get; set; }
        public int UserID { get; set; }

        public GetHeadlinesCriteria(string username, string groupHeadline, int status, int userId, string groupName)
        {
            Status = status;
            Username = username;
            GroupHeadline = groupHeadline;
            UserID = userId;
            GroupName = groupName;
        }
        
        public override string GetSettingKey()
        {
            return "EAS_CustomerSelByGroupHeadline";
        }

        // public override Hashtable GetSpParams()
        // {
        //     return new Hashtable
        //     {
        //         { "username", Username },
        //         { "groupheadline", GroupHeadline },
        //         { "status", Status },
        //         { "groupname", GroupName },
        //         { "userid", UserID },
        //     };
        // }
        
        public override bool IsValid()
        {
            return UserID > 0 && !string.IsNullOrWhiteSpace(GroupName);
        }
    }
}