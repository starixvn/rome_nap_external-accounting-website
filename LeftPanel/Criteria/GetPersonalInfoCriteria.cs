namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;


    public class GetPersonalInfoCriteria : CriteriaBase
    {
        public GetPersonalInfoCriteria(string username, string groupName, int userId)
        {
            Username = username;
            GroupName = groupName;
            UserId = userId;
        }

        public string Username { get;  }
        public string GroupName { get;  }
        public int UserId { get;  }

        public override string GetSettingKey()
            => "EAS_CustomerSelAllInfo";

        public override bool IsValid() => true;
    }
}
