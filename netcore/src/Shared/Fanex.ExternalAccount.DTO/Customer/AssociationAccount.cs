using System;
using Fanex.ExternalAccount.Core.Shared;

namespace Fanex.ExternalAccount.DTO.Customer
{
    public class AssociationAccountModel
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public int RoleId { get; set; }

        public int AgentId { get; set; }

        public string LevelName
        {
            get
            {
                return Role.DisplayRoleName(RoleId, AgentId);
            }
        }

        public int Status { get; set; }

        public string StatusName { get; set; }

        public DateTime Date { get; set; }

        public string DateString => Date.ToString("MM/dd/yyyy HH:mm:ss tt");

        public bool Suspended { get; set; }

        public bool Enabled { get; set; }

        public bool Closed { get; set; }
    }
}