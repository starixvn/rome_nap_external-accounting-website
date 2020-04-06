namespace Fanex.ExternalAccount.SearchAndCopy.Criteria
{
    using Fanex.NAP.Framework.Data;

    public class CloneSettingComCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public string UserPwd { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string MobilePhone { get; set; }

        public string Fax { get; set; }

        public decimal Credit { get; set; }

        public int ExistingCustId { get; set; }

        public int CustId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public int Creator { get; set; }

        public string IP { get; set; }

        public override bool IsValid() =>
            Creator > 0
            && ExistingCustId > 0
            && !string.IsNullOrWhiteSpace(Username)
            && !string.IsNullOrWhiteSpace(UserPwd)
            && Credit >= 0;

        public override string GetSettingKey() => "EAS_CustomerInsMember_CloneSetting_Com";
    }
}
