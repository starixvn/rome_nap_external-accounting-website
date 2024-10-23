namespace Fanex.ExternalAccount.SearchAndCopy.Criteria
{
    using Fanex.NAP.Framework.Data;

    public class ValidateCloneSettingComCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public decimal Credit { get; set; }

        public int ExistingCustId { get; set; }

        public string IP { get; set; }

        [SpParam(Ignored = true)]
        public int SiteDBGroupId { get; set; } 

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Username);

        public override string GetSettingKey() => (SiteDBGroupId == 0 ? string.Empty : $"DBSite{SiteDBGroupId}_" ) + "EAS_CustomerInsMember_CloneSetting_Com_Validate";
    }

    public class RegisterCustomerCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public short SiteDbGroupId { get; set; }

        public string Site { get; set; }

        public int? Currency { get; set; }

        public byte? RoleId { get; set; }

        public byte? UplineId{ get; set; }

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Username);

        public override string GetSettingKey() => "Acc_CustomerIns_Register";
    }

    public class GetGroupParentNameByGroupNameCriteria : CriteriaBase
    {
        public string GroupAdmin { get; set; }

        public override bool IsValid() => !string.IsNullOrWhiteSpace(GroupAdmin);

        public override string GetSettingKey() => "EAS_GroupAdmin_Sel";
    }

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

        public string GroupAdmin { get; set; }

        public string GroupName { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public int Creator { get; set; }

        public string IP { get; set; }

        public byte SettingStatus { get; set; }

        [SpParam(Ignored = true)]
        public int SiteDBGroupId { get; set; }

        public override bool IsValid() =>
#pragma warning disable S1067 // Expressions should not be too complex
            Creator > 0
            && ExistingCustId > 0
            && !string.IsNullOrWhiteSpace(Username)
            && !string.IsNullOrWhiteSpace(UserPwd)
            && Credit >= 0;
#pragma warning restore S1067 // Expressions should not be too complex

        public override string GetSettingKey() => (SiteDBGroupId == 0 ? string.Empty : $"DBSite{SiteDBGroupId}_") + "EAS_CustomerInsMember_CloneSetting_Com";
    }

    public class RollbackCustomerInsertCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public override bool IsValid() => CustId > 0;

        public override string GetSettingKey() => "Acc_CustomerIns_Rollback";
    }
}
