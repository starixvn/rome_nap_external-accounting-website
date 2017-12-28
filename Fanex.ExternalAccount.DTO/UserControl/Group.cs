namespace Fanex.ExternalAccount.DTO.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Group
    {
        private const int ITGroupId = 1;
        private const string FinanceGroupName = "Finance";
        private static readonly List<string> OfficeGroups = new List<string> {FinanceGroupName, "Internal", "IT"};

        public int GroupID { get; set; }

        public int NoUsers { get; set; }

        public string GroupName { get; set; }

        public string Description { get; set; }

        public string CreatedDate { get; set; }

        public string Creator { get; set; }

        public bool IsOffice => IsOfficeGroup(GroupName);

        public bool IsNotOffice => !IsOffice;

        public bool IsIT => IsITGroup(GroupID);

        public bool IsNotIT => !IsIT;

        public bool IsFinance => IsFinanceGroup(GroupName);

        public bool IsNotFinance => !IsFinance;

        public static bool IsOfficeGroup(string groupName)
            => OfficeGroups.Any(officeGroup => officeGroup.Equals(groupName, StringComparison.OrdinalIgnoreCase));

        public static bool IsITGroup(int groupId) => ITGroupId == groupId;

        public static bool IsFinanceGroup(string groupName)
            => FinanceGroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase);
    }
}