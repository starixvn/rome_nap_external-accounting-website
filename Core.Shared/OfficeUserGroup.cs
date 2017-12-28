namespace Fanex.ExternalAccount.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class OfficeUserGroup
    {
        private const int ITGroupId = 1;
        private const string FinanceGroupName = "Finance";

        public static List<string> OfficeGroups { get; } = new List<string> { FinanceGroupName, "Internal", "IT" };

        public static bool IsOfficeUserGroup(string groupName)
            => OfficeGroups.Any(officeGroup => officeGroup.Equals(groupName, StringComparison.OrdinalIgnoreCase));

        public static bool IsITUserGroup(int groupId) => ITGroupId == groupId;

        public static bool IsFinanceUserGroup(string groupName)
            => FinanceGroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase);
    }
}