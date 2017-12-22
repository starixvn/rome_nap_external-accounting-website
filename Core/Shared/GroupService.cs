namespace Fanex.ExternalAccount.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class GroupService
    {
        private static string ITGroupName = "IT";
        private static string FinanceGroupName = "Finance";

        public static List<string> OfficeGroups { get; }
            = new List<string> { FinanceGroupName, "Internal", ITGroupName };

        public static bool IsOfficeGroup(string group)
            => OfficeGroups.Any(officeGroup => officeGroup.Equals(group, StringComparison.OrdinalIgnoreCase));

        public static bool IsITGroup(string groupName)
            => ITGroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase);

        public static bool IsFinanceGroup(string groupName)
            => FinanceGroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase);
    }
}