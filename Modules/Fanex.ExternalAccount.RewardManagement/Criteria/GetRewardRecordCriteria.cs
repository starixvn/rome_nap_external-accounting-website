namespace Fanex.ExternalAccount.RewardManagement.Criteria
{
    using System;

    public class GetRewardRecordCriteria : PagingRewardReportCriteria
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string ProgramIds { get; set; }

        public string UserName { get; set; }

        public string GroupName { get; set; }

        public byte? PaymentStatus { get; set; }

        public override string GetSettingKey() => "EAS_Rpt_RewardDetail_Com";

        public override bool IsValid() =>
            !string.IsNullOrWhiteSpace(ProgramIds) &&
            !string.IsNullOrWhiteSpace(GroupName) &&
            FromDate <= ToDate;
    }
}