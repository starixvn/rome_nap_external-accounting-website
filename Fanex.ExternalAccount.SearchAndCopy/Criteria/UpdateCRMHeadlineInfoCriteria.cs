namespace Fanex.ExternalAccount.SearchAndCopy.Criteria
{
    using Fanex.NAP.Framework.Data;

    public class UpdateCRMHeadlineInfoCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public string UserName { get; set; }

        public int Case { get; set; }

        public int Old_Headline { get; set; }

        public int New_Headline { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey() => "Acc_CRM_HeadlineUpd";

        public override bool IsValid() => true;

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string LoginIP { get; set; }
    }
}
