namespace Fanex.ExternalAccount.ProblemAccount.Models
{
    using System;
    using Fanex.ExternalAccount.DTO.Customer;
    using Fanex.ExternalAccount.ProblemAccount.Constants;
    using Fanex.ExternalAccount.ProblemAccount.Extensions;
    using Fanex.ExternalAccount.ProblemAccount.Resources;

    public class ProblemAccountEntity
    {
        public ProblemAccountEntity(
            int custId,
            string username,
            string firstName,
            string lastName,
            int numberOfDownline,
            byte status,
            DateTime createdDate,
            bool isReviewed,
            byte closedReason,
            byte locked,
            int agentId)
        {
            CustId = custId;
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Status = status;
            NumberOfDownline = numberOfDownline;
            CreatedDate = createdDate;
            IsReviewed = isReviewed;
            ClosedReason = closedReason;
            Locked = locked;
            AgentId = agentId;
        }

        public int CustId { get; }

        public string Username { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public byte Status { get; }

        public string StatusName
            => AccountStatusConst.GetAccountStatusName(Status);

        public int NumberOfDownline { get; }

        public DateTime CreatedDate { get; }

        public string CreatedDateStr
            => CreatedDate.ToDateTimeInMinuteString();

        public bool IsReviewed { get; }

        public string ReviewStatus
            => IsReviewed ? ViewLang.Reviewed : ViewLang.NotReviewed;

        public byte ClosedReason { get; }

        public byte Locked { get; }

        public int AgentId { get; }

        public string LockedClassName
            => new CustomerLockedAccess(Locked).ClassName;
    }
}
