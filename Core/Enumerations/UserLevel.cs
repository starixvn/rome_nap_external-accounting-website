namespace Fanex.ExternalAccount.Core.Enumerations
{
    using System;

    [Serializable]
    public class UserLevel : Enumeration
    {
        public static readonly UserLevel Super = new UserLevel(4, "Super");
        
        public static readonly UserLevel Master = new UserLevel(3, "Master");
        
        public static readonly UserLevel Agent = new UserLevel(2, "Agent");
        
        public static readonly UserLevel Member = new UserLevel(1, "Member");

        public UserLevel()
        {
        }

        private UserLevel(int value, string displayName)
            : base(value, displayName)
        {
        }

        public bool IsAgentLevel() => this == Agent;

        public bool IsNotAgentLevel() => !IsAgentLevel();

        public bool IsMasterLevel() => this == Master;

        public bool IsNotMasterLevel() => !IsMasterLevel();

        public bool IsSuperLevel() => this == Super;

        public bool IsMasterOrSuperLevel() => IsMasterLevel() || IsSuperLevel();

        public bool IsNotSuperLevel() => !IsSuperLevel();

        public bool IsMemberLevel() => this == Member;

        public bool IsDirectUplineOf(int downlineLevelValue)
            => GapBetweenTwoLevels(Value, downlineLevelValue) == 1;

        public bool IsDirectUplineOf(Enumeration downlineLevel)
            => (downlineLevel != null) && IsDirectUplineOf(downlineLevel.Value);

        public bool IsUplineOf(int downlineLevelValue)
            => GapBetweenTwoLevels(Value, downlineLevelValue) >= 1;

        public bool IsUplineOf(Enumeration downlineLevel)
            => (downlineLevel != null) && IsUplineOf(downlineLevel.Value);

        public bool IsEqualOrUplineOf(Enumeration custLevel)
           => this.Value == custLevel.Value || IsUplineOf(custLevel.Value);

        public UserLevel GetDirectDownLineLevel() => FromValue<UserLevel>((int)(Value - 1));

        public bool IsDirectDownlineOf(Enumeration uplineLevel)
            => (uplineLevel != null) && GapBetweenTwoLevels(uplineLevel.Value, Value) == 1;

        public bool Is(Enumeration level)
            => (level != null) && this == level;

        public bool IsNot(Enumeration level) => !Is(level);

        private static int GapBetweenTwoLevels(int uplineLevelValue, int downlineLevelValue)
            => uplineLevelValue - downlineLevelValue;
    }
}
