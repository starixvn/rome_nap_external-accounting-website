namespace Fanex.ExternalAccount.RewardManagement.Models
{
    using System;

    public class RewardRecord
    {
        private const string FormatPaymentDate = "MM/dd/yyyy";
        public DateTime CreatedDate { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName => CacheRef.GetResourceDataByRefIdAndResourceId("RewardProgram", ProgramId.ToString(), "en-US");
        
        public string MemberUsername { get; set; }
        public decimal MemberAmount { get; set; }
        public string MemberCurrency { get; set; }
        public bool MemberPaymentStatus { get; set; }
        public string MemberPaymentStatusMapped =>  MappingPaymentStatus(MemberPaymentStatus);
        public DateTime? MemberPaymentDate { get; set; }

        public string MemberPaymentDateString => MemberPaymentDate?.ToString(FormatPaymentDate) ?? "";
        public string MemberFeedBack { get; set; }
        public string MemberFeedBackMapped
        {
            get
            {
                switch (MemberFeedBack)
                {
                    case "0":
                        return "Received";
                    case "1":
                        return "Not Received";
                    default:
                        return "";
                }
            }
        }
        
        public string AgentUsername { get; set; }
        public bool AgentPaymentStatus { get; set; }
        public DateTime? AgentPaymentDate { get; set; }
        
        public string AgentPaymentStatusMapped => Recommend.IsDirectMemberOfSuper() || Recommend.IsDirectMemberOfMaster() ? "" : MappingPaymentStatus(AgentPaymentStatus);
        public string AgentPaymentDateString => Recommend.IsDirectMemberOfSuper() || Recommend.IsDirectMemberOfMaster() ? "" : AgentPaymentDate?.ToString(FormatPaymentDate) ?? "";
        
        public string MasterUsername { get; set; }
        public bool MasterPaymentStatus { get; set; }
        public DateTime? MasterPaymentDate { get; set; }
        
        public string MasterPaymentStatusMapped => Recommend.IsDirectMemberOfSuper() ? "" : MappingPaymentStatus(MasterPaymentStatus);
        public string MasterPaymentDateString => Recommend.IsDirectMemberOfSuper() ? "": MasterPaymentDate?.ToString(FormatPaymentDate) ?? "";
        
        public string SuperUsername { get; set; }
        public bool SuperPaymentStatus { get; set; }
        public DateTime? SuperPaymentDate { get; set; }
        
        public string SuperPaymentStatusMapped => MappingPaymentStatus(SuperPaymentStatus);
        public string SuperPaymentDateString => SuperPaymentDate?.ToString(FormatPaymentDate) ?? "";
        
        private static string MappingPaymentStatus(bool status)
        {
            return status ? "Paid" : "Unpaid";
        }
        
        public int Recommend { get; set; }
    }
}