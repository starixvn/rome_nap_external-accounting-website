namespace Fanex.ExternalAccount.DTO.Customer
{
    using System;
    using Core.Shared;

    public class LineInfo
    {
        public int CustId { get; set; }

        public string Username { get; set; } = string.Empty;

        public int RoleId { get; set; }

        public int SuperId { get; set; }

        public string SuperName { get; set; } = string.Empty;

        public int MasterId { get; set; }

        public string MasterName { get; set; } = string.Empty;

        public int AgentId { get; set; }

        public string AgentName { get; set; } = string.Empty;

        public string MemberName => RoleId.IsMember() ? Username : string.Empty;

        public int GetUplineId()
        {
            switch (RoleId)
            {
                case Role.Master:
                    return SuperId;

                case Role.Agent:
                    return MasterId;

                case Role.Member:
                    return AgentId;

                default:
                    throw new InvalidOperationException(nameof(RoleId));
            }
        }

        public bool HasSecurityCode { get; set; }
    }
}