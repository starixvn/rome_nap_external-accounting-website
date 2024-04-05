using Fanex.NAP.Framework.Data;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.EditSuperList
{
    public class UpdateMultipleSuperInfoCriteria : CriteriaBase
    {
        public string CustIDList { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string MobilePhone { get; set; }

        public string IP { get; set; }

        public int UserId { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
         => "EAS_CustomerUpdSuperInfo_Multiple";
    }
}
