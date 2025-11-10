using System;
using System.Diagnostics;

namespace Fanex.ExternalAccount.ReportsAndStatements.Utilities
{
    public static class Role
    {
        public const int Member = 1;
        public const int Agent = 2;
        public const int Master = 3;
        public const int Super = 4;
        public const int Group = 5;

        public const int DirectMemberOfSuper = -4;
        public const int DirectMemberOfMaster = -3;

        [DebuggerStepThrough]
        public static string DisplayRoleName(this int role)
        {
            EnsureRoleIsInRange(role);
            switch (role)
            {
                case Super:
                    return "Super";
                case Master:
                    return "Master";
                case Agent:
                    return "Agent";
                default:
                    return "Member";
            }
        }

        [DebuggerStepThrough]
        public static string DisplayRoleName(this int role, int recommend)
        {
            EnsureRoleIsInRange(role);
            switch (role)
            {
                case Super:
                    return "Super";
                case Master:
                    return "Master";
                case Agent:
                    return "Agent";
                default:
                    return recommend.IsDirectMemberOfSuper() ? "Super direct member" : recommend.IsDirectMemberOfMaster() ? "Master direct member" : "Member";
            }
        }

        [DebuggerStepThrough]
        public static bool IsMember(this int role)
        {
            EnsureRoleIsInRange(role);
            return role == Member;
        }

        [DebuggerStepThrough]
        public static bool IsNotMember(this int role) => !IsMember(role);

        [DebuggerStepThrough]
        public static bool IsAgent(this int role)
        {
            EnsureRoleIsInRange(role);
            return role == Agent;
        }

        [DebuggerStepThrough]
        public static bool IsNotAgent(this int role) => !IsAgent(role);

        [DebuggerStepThrough]
        public static bool IsMaster(this int role)
        {
            EnsureRoleIsInRange(role);
            return role == Master;
        }

        [DebuggerStepThrough]
        public static bool IsNotMaster(this int role) => !IsMaster(role);

        [DebuggerStepThrough]
        public static bool IsSuper(this int role)
        {
            EnsureRoleIsInRange(role);
            return role == Super;
        }

        [DebuggerStepThrough]
        public static bool IsHeadline(this int role)
        {
            return role == Group;
        }

        [DebuggerStepThrough]
        public static bool IsNotSuper(this int role) => !IsSuper(role);

        [DebuggerStepThrough]
        public static bool IsDirectMemberOfSuper(this int recommend)
        {
            return recommend == DirectMemberOfSuper;
        }

        [DebuggerStepThrough]
        public static bool IsDirectMemberOfMaster(this int recommend)
        {
            return recommend == DirectMemberOfMaster;
        }

        [DebuggerStepThrough]
        public static int DirectDownlineLevel(this int role)
        {
            EnsureRoleIsInRange(role);
            return role - 1;
        }

        [DebuggerStepThrough]
        public static int DirectUplineLevel(this int role)
        {
            EnsureRoleIsInRange(role);
            return role + 1;
        }

        [DebuggerStepThrough]
        private static void EnsureRoleIsInRange(int role)
        {
            if (role < Member || role > Group)
            {
                throw new ArgumentOutOfRangeException(nameof(role));
            }
        }
    }
}
