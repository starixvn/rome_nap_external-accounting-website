

using Fanex.NAP.Framework.Data;

namespace Fanex.Puffin.Website.Areas.UserLogin.Criteria
{
    public class UserPrivilegeCriteria: CriteriaBase
    {
        public int UserId { get; set; }
        public int SiteId { get; set; }
        public string Environment { get; set; }


        

        public override string GetSettingKey()
        {
            return "UserPrivilege";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}


