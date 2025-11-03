namespace Fanex.ExternalAccount.DTO.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Group
    {        
        private static readonly List<string> OfficeGroups = new List<string> { "Finance", "IT" };

        public const int ITGroupId = 1;

        public int GroupID { get; set; }

        public int NoUsers { get; set; }

        public string GroupName { get; set; }

        public string Description { get; set; }

        public string CreatedDate { get; set; }

        public string Creator { get; set; }

        public int ParentGroupId { get; set; }

        public string ParentGroupName { get; set; }

        public bool IsIT => IsITGroup(GroupID);

        public static bool IsOfficeGroup(string groupName)
            => OfficeGroups.Any(officeGroup => officeGroup.Equals(groupName, StringComparison.OrdinalIgnoreCase));

        public static bool IsITGroup(int groupId) => ITGroupId == groupId;   
    }
}