namespace Fanex.ExternalAccount.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class GroupService
    {
        public static List<string> OfficeGroups { get; }
            = new List<string> {"Finance", "Internal", "IT"};

        public static bool IsOfficeGroup(string group)
            => OfficeGroups.Any(officeGroup => officeGroup.Equals(group, StringComparison.OrdinalIgnoreCase));
    }
}