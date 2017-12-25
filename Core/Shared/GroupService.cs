namespace Fanex.ExternalAccount.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class GroupService
    {
        private const int ITGroupId = 1;
        private const string FinanceGroupName = "Finance";

        public static List<string> OfficeGroups { get; }
            = new List<string> { FinanceGroupName, "Internal", "IT" };

        public static bool IsOfficeGroup(string group)
            => OfficeGroups.Any(officeGroup => officeGroup.Equals(group, StringComparison.OrdinalIgnoreCase));

        public static bool IsITGroup(int groupId) => ITGroupId == groupId;

        public static bool IsFinanceGroup(string groupName)
            => FinanceGroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase);
    }
}