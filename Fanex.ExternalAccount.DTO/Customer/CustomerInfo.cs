using System;

namespace Fanex.ExternalAccount.DTO.Customer
{
    public class CustomerInfoSearchResult
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public int RoleId { get; set; }

        public int SuperId { get; set; }

        public string SuperName { get; set; }

        public int MasterId { get; set; }

        public string MasterName { get; set; }

        public int AgentId { get; set; }

        public string AgentName { get; set; }

        public string MemberName => RoleId == 1 ? Username : string.Empty;

        public int GetCustomerUplineId()
        {
            switch (RoleId)
            {
                case 3:
                    return SuperId;

                case 2:
                    return MasterId;

                case 1:
                    return AgentId;

                default:
                    throw new InvalidOperationException(nameof(RoleId));
            }
        }

        public bool HasSecurityCode { get; set; }
    }
}