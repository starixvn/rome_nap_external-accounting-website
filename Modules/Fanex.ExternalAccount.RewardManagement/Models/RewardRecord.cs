namespace Fanex.ExternalAccount.RewardManagement.Models
{
    using Fanex.ExternalAccount.Core.Shared;
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
        public string MemberPaymentStatusMapped => MappingPaymentStatus(MemberPaymentStatus);
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
        public string AgentPaymentStatusMapped => MappingPaymentStatus(AgentPaymentStatus);
        public DateTime? AgentPaymentDate { get; set; }

        public string AgentPaymentDateString => AgentPaymentDate?.ToString(FormatPaymentDate) ?? "";

        
        public string MasterUsername { get; set; }
        public bool MasterPaymentStatus { get; set; }
        public string MasterPaymentStatusMapped => MappingPaymentStatus(MasterPaymentStatus);
        public DateTime? MasterPaymentDate { get; set; }

        public string MasterPaymentDateString => MasterPaymentDate?.ToString(FormatPaymentDate) ?? "";


        public string SuperUsername { get; set; }
        public bool SuperPaymentStatus { get; set; }

        public string SuperPaymentStatusMapped => MappingPaymentStatus(SuperPaymentStatus);
        public DateTime? SuperPaymentDate { get; set; }

        public string SuperPaymentDateString => SuperPaymentDate?.ToString(FormatPaymentDate) ?? "";
        
        private static string MappingPaymentStatus(bool status)
        {
            return status ? "Paid" : "Unpaid";
        }
        
        public bool IsDirectMember { get; set; }
    }
}