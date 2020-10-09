namespace Fanex.ExternalAccount.ProblemAccount.Models
{
    using System;
    using Fanex.ExternalAccount.ProblemAccount.Extensions;
    using Fanex.ExternalAccount.ProblemAccount.Resources;

    public class ProblemAccountEntity
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public byte Status { get; set; }

        public string StatusName
        {
            get
            {
                switch (Status)
                {
                    case 1:
                        return "Open";
                    case 2:
                        return "Suspended";
                    case 3:
                        return "Closed";
                    case 4:
                        return "Disabled";
                    default:
                        return string.Empty;
                }
            }
        }

        public int NumberOfDownline { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedDateStr
            => CreatedDate.ToDateTimeInMinuteString();

        public bool IsReviewed { get; set; }

        public string ReviewStatus
            => IsReviewed ? ViewLang.Reviewed : ViewLang.NotReviewed;

        public int ClosedReason { get; set; }
    }
}
