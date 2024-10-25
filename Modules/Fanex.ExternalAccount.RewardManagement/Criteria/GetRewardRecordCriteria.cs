namespace Fanex.ExternalAccount.RewardManagement.Criteria
{
    using System;
    using Fanex.ExternalAccount.RewardManagement.Requests;
    using Fanex.NAP.Framework.Data;

    public class GetRewardRecordCriteria : CriteriaBase
    {
        public DateTime FromDate { get; }

        public DateTime ToDate { get; }
        
        public string ProgramIds { get; set; }

        public string UserName { get; }
        
        public string GroupName { get; }

        public byte? PaymentStatus { get; }

        public int PageIndex { get; set; } = 0;
        
        public int PageSize { get; set; } = 0;

        public int TotalRecords { get; set; }
        
        public override string GetSettingKey() => "EAS_Rpt_RewardDetail_Com";

        public GetRewardRecordCriteria(RewardReportRequest request)
        {
            FromDate = request.FromDate;
            ToDate = request.ToDate;
            UserName = request.Username;
            ProgramIds = request.ProgramIds;
            PaymentStatus = request.MemberPaymentStatus;
            GroupName = request.GroupName;
            PageSize = request.PageSize;
            PageIndex = request.Page;
        }
    }
}