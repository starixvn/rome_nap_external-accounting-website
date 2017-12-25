namespace Fanex.ExternalAccount.Core.Shared
{
    using System;
    using System.Diagnostics;

    public static class Role
    {
        public const int Member = 1;
        public const int Agent = 2;
        public const int Master = 3;
        public const int Super = 4;

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
        public static bool IsNotSuper(this int role) => !IsSuper(role);

        [DebuggerStepThrough]
        public static int DirectDownlineLevel(this int role)
        {
            if (role > Member)
            {
                return role - 1;
            }

            throw new InvalidOperationException("Member doest not have downline");
        }

        [DebuggerStepThrough]
        private static void EnsureRoleIsInRange(int role)
        {
            if (role < 1 || role > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(role));
            }
        }
    }
}